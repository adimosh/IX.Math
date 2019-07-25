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