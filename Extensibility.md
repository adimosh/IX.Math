# IX.Math Extensibility

## Introduction

Extensibility in IX.Math is achievable through multiple channels:

- The mathematics definition
- Functions
- Extractors
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

### Basic definition

The newly-created class should inherit from one of the base classes that define functions
usable by the engine: [NonaryFunctionNodeBase](src/IX.Math/Nodes/NonaryFunctionNodeBase.cs),
[UnaryFunctionNodeBase](src/IX.Math/Nodes/UnaryFunctionNodeBase.cs),
[BinaryFunctionNodeBase](src/IX.Math/Nodes/BinaryFunctionNodeBase.cs)
or [TernaryFunctionNodeBase](src/IX.Math/Nodes/TernaryFunctionNodeBase.cs).

For now, only unary, binary, ternary and nonary (no parameters) functions can be created,
but a generalized implementation is planned.

Each class must be decorated with the [CallableMathematicsFunctionAttribute](src/IX.Math/Extensibility/CallableMathematicsFunctionAttribute.cs),
and their containing assembly must be registered with the IExpressionParsingService's
RegisterFunctionsAssembly method. The attribute is valid only on classes (struct support
is considered, but not currently planned as it will most likely involve a completely
separate extensibility system) and is not inheritable (meaning it has to be explicitly
added to all classes that represent callable functions). The attribute's constructor
takes at least one name, which represents the function name.

The reason for this design is to allow multiple points of extensibility even when talking
about functions, by allowing developers to have as many derived classes of the
aforementioned base classes, for either grouping, functionality or volatility reasons,
without encumbering the engine with anything the developer does not want to make
available.

Assuming that we have a function named as _&quot;scrt&quot;_ that is supposed to be
a binary function, we can therefore define a class as:

```csharp
[CallableMathematicsFunction("scrt")]
public class ScrtFunctionNode : BinaryFunctionNodeBase
{
    // ... class contents
}
```

Such a construct will enable the following expression to be recognized:

```1+scrt(7,12)```

The actual result (including its type) depends on what the class actually does.

### Basic operation

Under construction

## Extractors

There are two types of extractors available: constant extractors and pass-through
extractors.

The pass-through extractors will be called when the expression is first evaluated.
If the method called on it returns true, then the expression is kept as a literal
string, otherwise it is interpreted.

Constant extractors work on any unidentified symbols in the expression and have the
ability to define symbols otherwise not recognized by the mathematics engine.

## Parsing formatters

Under construction

## Type formatters

Under construction