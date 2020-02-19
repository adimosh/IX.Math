# IX.Math usage guide

## Standard operation

This library is based on two implementations of the interface IExpressionParsingService:
- [ExpressionParsingService](src/IX.Math/ExpressionParsingService.cs) - a parsing service
that just spits out delegates on demand
- [CachedExpressionParsingService](src/IX.Math/CachedExpressionParsingService.cs) - a
parsing service that also caches its expressions

There is one method that is implemented in both: Interpret. This method takes in one
string (and an optional [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken))
and generates a ComputedExpression object which can afterwards be used to calculate the
result of the expression.

A computed expression can be parameterless (for instance ```5+6```) or parametered
(for instance ```2+x```).

The ComputedExpression features two overloads of the Compute method:
- The first overload takes parameters as objects
- The second overload takes an IDataFinder, which is an interface that the library user
is supposed to implement and that will fetch items by name

Each of these methods results in a possible result, or, if the expression doesn't make
sense from a mathematics perspective, or if the parameters fed to it are of the wrong
type, will return the original expression as a string.

## Supported types

The primitive types supported as input values are:

- ```long``` - the integer numeric type
- ```double``` - the floating-point numeric type
- ```byte[]``` - the binary type
- ```string``` - the string type
- ``` bool``` - the boolean type

These primitive types also double as the supported output types. The ```object```
returned by computing an expression is guaranteed to be one of these four types.

Please note that the ```object``` type is not supported, and attempting to marshal
in an ```object``` without offering the engine the chance to determine its exact
type will result in an internal error, and the returning of the original expression.

Apart from the primitive types, input values can consist of function pointers of the
delegate ```Func<T>```, where ```T``` is any of the above type. This is needed
when the input has to calculate a value on the fly, and/or the value can change
depending on the exact moment in which it is computed.

Currently-planned features (checked means in progress):

- [ ] special conversion for types ```byte```, ```char```, ```single```, ```int```,
```float```, ```char[]```, ```BitArray``` into their supported equivalents
- [ ] ```DateTime``` and ```TimeSpan``` types
- [ ] ```Span<T>``` and ```Memory<T>``` types, as well as their read-only equivalents

## Available internal types

There are four logical types used by the library:

- ```[numeric]``` - a numeric type used to calculate mathematical expressions; comes
in two flavors: ```[numeric-int]```, which is an obligatory integer version for when
a specified number has to be an integer (for example, at bit shift, you cannot shift
by 2.7318 bits, so the ```>>``` and ```<<``` operators have an obligatory integer
second operand), and ```[numeric-float]```, which is the obligatory floating point
version of the number; if only ```[numeric]``` is specified, this means that the engine
does not care whether or not the numeric value is an integer or a floating point number,
or, if used as a return type, indicates that the return value cannot be guaranteed to
be either of the two sub-types
- ```[string]``` - a string type; works pretty much as one would expect a string to
work
- ```[binary]``` - basically a blob of binary data, can be seen as the eqivalent of a
```byte[]``` in C#; while there isn't a whole lot of things to do with it, it still
can make parts of expressions, especially if used as recognition expression for binary
data and messages
- ```[boolean]``` - everyone's favorite ```true``` or ```false``` variable, the result
of pretty much all logical operators in the world

Currently-planned features (checked means in progress):

- [ ] special conversion functions ```string(...)```, ```numeric(...)```,
```binary(...)``` and ```boolean(...)```
- [ ] ```[datetime]``` type
- [ ] ```[money]``` type

## Available operators

_Under construction_

## Available mathematical functions

The functions that can be invoked are:

### Nonary functions

| Function | Purpose |
|:--------:|:-------:|
| ```[numeric] rand()``` | Random number |
| ```[numeric] random()``` | Random number |
| ```[numeric-int] randomint()``` | Random integer number |

### Unary functions

| Function | Purpose |
|:--------:|:-------:|
| ```[numeric] abs([numeric])``` | Absolute value of a number |
| ```[numeric] absolute([numeric])``` | Absolute value of a number |
| ```[numeric] acos([numeric])``` | Arccosine |
| ```[numeric] arccos([numeric])``` | Arccosine |
| ```[numeric] arccosine([numeric])``` | Arccosine |
| ```[numeric] asin([numeric])``` | Arcsine |
| ```[numeric] arcsin([numeric])``` | Arcsine |
| ```[numeric] arcsine([numeric])``` | Arcsine |
| ```[numeric] atan([numeric])``` | Arctangent |
| ```[numeric] actg([numeric])``` | Arctangent |
| ```[numeric] arctangent([numeric])``` | Arctangent |
| ```[numeric] ceil([numeric])``` | Ceiling of a rational number |
| ```[numeric] ceiling([numeric])``` | Ceiling of a rational number |
| ```[numeric] cos([numeric])``` | Cosine |
| ```[numeric] cosine([numeric])``` | Cosine |
| ```[numeric] lg([numeric])``` | Decimal logarithm |
| ```[numeric] exp([numeric])``` | Exponential |
| ```[numeric] exponential([numeric])``` | Exponential |
| ```[numeric] floor([numeric])``` | Floor of a rational number |
| ```[numeric] cosh([numeric])``` | Hyperbolic cosine |
| ```[numeric] sinh([numeric])``` | Hyperbolic sine |
| ```[numeric] tanh([numeric])``` | Hyperbolic tangent |
| ```[numeric] ln([numeric])``` | Natural logarithm |
| ```[numeric] rand([numeric])``` | Random number less than a specified number |
| ```[numeric] random([numeric])``` | Random number less than a specified number |
| ```[numeric-int] randomint([numeric-int])``` | Random integer number less than a specified number |
| ```[numeric] round([numeric])``` | The round value of a rational number |
| ```[numeric] sin([numeric])``` | Sine |
| ```[numeric] sine([numeric])``` | Sine |
| ```[numeric] sqrt([numeric])``` | Square root |
| ```[numeric] squareroot([numeric])``` | Square root |
| ```[numeric-int] strlen([string])``` | Length of a string |
| ```[numeric-int] length([string])``` | Length of a string |
| ```[numeric] tan([numeric])``` | Tangent |
| ```[numeric] tangent([numeric])``` | Tangent |
| ```[string] trim([string])``` | Trimming of a string of all whitespace from the margins |
| ```[numeric] trun([numeric])``` | The integral part of a rational number |
| ```[numeric] truncate([numeric])``` | The integral part of a rational number |

### Binary functions

| Function | Purpose |
|:--------:|:-------:|
| ```[numeric] log([numeric], [numeric])``` | Logarithm |
| ```[numeric] logarithm([numeric], [numeric])``` | Logarithm |
| ```[numeric] max([numeric], [numeric])``` | The maximum of two numbers |
| ```[numeric] maximum([numeric], [numeric])``` | The maximum of two numbers |
| ```[numeric] min([numeric], [numeric])``` | The minimum of two numbers |
| ```[numeric] minimum([numeric], [numeric])``` | The minimum of two numbers |
| ```[numeric] pow([numeric], [numeric])``` | Power |
| ```[numeric] power([numeric], [numeric])``` | Power |
| ```[numeric] rand([numeric], [numeric])``` | Random number between two specified numbers |
| ```[numeric] random([numeric], [number])``` | Random number between two specified numbers |
| ```[numeric-int] randomint([numeric-int], [numeric-int])``` | Random integer between two specified numbers |
| ```[string] substr([string], [numeric-int])``` | A substring beginning at a specified position |
| ```[string] substring([string], [numeric-int])``` | A substring beginning at a specified position |
| ```[string] trim([string], [string])``` | Trimming of a string of all specified characters from the margins |
| ```[string] trimbody([string], [string])``` | Trimming of a string of a specified substring from the entire body |

### Ternary functions

| Function | Purpose |
|:--------:|:-------:|
| ```[string] repl([string], [string], [string])``` | Replacing of a substring from a source string with another substring |
| ```[string] replace([string], [string], [string])``` | Replacing of a substring from a source string with another substring |
| ```[string] substr([string], [numeric-int], [numeric-int])``` | A substring between a set of indices |
| ```[string] substring([string], [numeric-int], [numeric-int])``` | A substring between a set of indices |
