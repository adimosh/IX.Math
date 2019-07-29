# IX.Math Extensibility

## Introduction

Extensibility in IX.Math is achievable through multiple channels:

- The mathematics definition
- Functions
- Constant extractors
- Pass-through extractors
- Parsing formatters
- Type formatters

## The mathematics definition

The [MathDefinition](src/IX.Math/MathDefinition.cs)
class allows one to define the exact parameters of the mathematical model that the
engine should follow. An instance of this class can be passed to the
[ExpressionParsingService](src/IX.Math/ExpressionParsingService.cs)
class and, consequently, to the [CachedExpressionParsingService](src/IX.Math/CachedExpressionParsingService.cs)
class, and will enable operation on a different set of conventions.

The default is the standard modern Euclidean mathematics conventions
for computer programming, which are expressed in the [ExpressionParsingService](src/IX.Math/ExpressionParsingService.cs)
class, at the first constructor.

When defining operators, one need not worry if an operator is a substring of another
operator. For example, let's say we define the regular addition &quot;+&quot; operator
as _&quot;add&quot;_ and the regular multiplication &quot;*&quot; operator as
_&quot;polyadd&quot;_. We can therefore have the following operation:

```(1 add 3) polyadd 8```

Such an expression will be correctly identified as:

```(1+3)*8```

...and will result in a constant 32 as a result. This is because the engine parses the
input expression first and replaces operators that run the risk of being substrings of
larger operators with symbols that are verified to not be found in the expression.

## Functions

In order to extend the set of functions that the IX.Math library supports, a new class
should be created for each function that can be invoked.

The documentation for extending functions can be found at
[the functions extensibility page](Functions.md).

## Constant Extractors

Constant extractors work on any unidentified symbols in the expression and have the
ability to define symbols otherwise not recognized by the mathematics engine.

In order to create a constant extractor, one needs to create a class in a scannable
assembly, that implements the [IConstantsExtractor](src/IX.Math/Extraction/IConstantsExtractor.cs)
interface, and is decorated with the [ConstantsExtractorAttribute](src/IX.Math/Extensibility/ConstantsExtractorAttribute.cs).

The overridable method ```ExtractAllConstants``` will do all the work on extracting
the constants. It takes the original expression, a dictionary of constants, a
dictionary of reverse lookup constants, and the math definition as parameters.

The extraction is done in multiple steps.

1. First, the extractor needs to identify the constants it can extract
2. The constant should then be looked up in the reverse lookup dictionary (to check
whether or not it has already been identified before - if yes, jump to step 6)
3. If the constant has not been identified before, a constant node should be created
4. That constant node should be given a name that is guaranteed to not be found in the
original or processed expression
5. Both the name and the node should be added to the constants dictionary, and the
constant value and its name should be added to the reverse lookup table
6. The expression should have the extracted value replaced by its name
7. The method should return the new expression

A few things to note:

- It is the extractor's responsibility to ensure that the constant is an actual constant,
and not part of a literal
- Returning ```null``` (```Nothing``` in Visual Basic) or an empty string will cause
complete graph invalidation, so please refrain from such practices (throw exceptions
if really necessary instead)
- The extractor can extract as many constants as it wishes at the same time, provided
that it follows the above steps for each one
- The extractor method call is never guaranteed to be thread-safe, even across
differing expressions, and should never be assumed to be such; it is the responsibility
of the extractor to ensure that its internal state is consistent and thread-safe
- The extractors are guaranteed to be called in sequence, so there is no risk of
overlap with a different extractor on the same expression

## Pass-through extractors

The pass-through extractors will be called when the expression is first evaluated.
If the method called on it returns true, then the expression is kept as a literal
string, otherwise it is interpreted.

This facility might not seem important if a standard expression parsing service is
used, however it can bring significant advantages if a cached expression parsing service
is used. In such a case, the expression is not only evaluated to be a pass-through
expression, but it is also cached as a pass-through expression, thus ensuring that
whenever a method call for its interpretation happens, the shortest route is always
taken (that of the cache).

In order to create a pass-through extractor, one needs to create a class in a scannable
assembly, that implements the [IConstantPassThroughExtractor](src/IX.Math/Extraction/IConstantPassThroughExtractor.cs)
interface, and is decorated with the [ConstantsPassThroughExtractorAttribute](src/IX.Math/Extensibility/ConstantsPassThroughExtractorAttribute.cs).

The only method that needs implemented in the extractor is the ```Evaluate``` method.
The method receives the original expression as a parameter, and has a ```bool```
return type. If the return value is ```true```, the expression is treated as a
pass-through expression, and will always be returned as a string literal.

## Parsing formatters

_Under construction_

Parsing formatters are not yet available to the general public, pending upgrading of the
```[string]``` data type to be convertible to other types via special functions.

## Type formatters

_Under construction_

Type formatters are, in a sense, the opposite of parsing formatters, as they aid when
a non-string is transformed into a string.

In order to create a type formatter, one needs to create a class in a scannable
assembly, that implements the [ITypeFormatter](src/IX.Math/Formatters/ITypeFormatter.cs)
interface, and is decorated with the [TypeFormatterAttribute](src/IX.Math/Extensibility/TypeFormatterAttribute.cs).

A type formatter will be invoked whenever a value of a specific type needs to be
converted to a string, wherever that would be the case in the expression. As an
example, were one to interpret the expression:

```
"Mary has " + numericParameter1
```

...a type formatter can be defined as:

```csharp
[TypeFormatter(FromType = SupportedValueType.Numeric)]
public class LittleLambFormatter : ITypeFormatter
{
    public string Format<TInput>(TInput input)
    {
        switch (input)
        {
            case long il:
                return il < 0 ?
                    $"some lambs she borrowed from her uncle Schrödinger" :
                    (il == 0 ? "no lambs" : $"{il} little lambs.");
            case double dl:
                return $"a binary lamb that holds the value {dl}";
        }
    }
}
```

This will have the following results:

```csharp
numericParameter1 = 17; // Mary has 17 little lambs
numericParameter1 = 0; // Mary has no lambs
numericParameter1 = -17; // Mary has some lambs she borrowed from her uncle Schrödinger
numericParameter1 = 72D; // Mary has a binary lamb that holds the value 72.0
```

Note: only the first type formatter will be accepted for any given internal type.