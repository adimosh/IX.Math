# IX.Math

## Introduction

IX.Math is a .NET library that reads mathematical formulas as text and creates delegates and delegate placeholders for solving them at runtime.

The library is capable of interpreting any mathematical expression that makes sense from a logical perspective, complete with a few mathematical functions. It supports integer numbers (as [long](https://msdn.microsoft.com/en-us/library/system.int64.aspx) in standard numeric formats), floating-point numbers (as [double](https://msdn.microsoft.com/en-us/library/system.double.aspx), strings (as [string](https://msdn.microsoft.com/en-us/library/system.string.aspx)) and boolean (as [bool](https://msdn.microsoft.com/en-us/library/system.boolean.aspx)) values, and can compute the most common mathematic operations, as well as certain mathematics functions.

## How to get

This project is primarily available through NuGet.

The current version can be accessed by using NuGet commands:

```powershell
Install-Package IX.Math
```

Releases: [![IX.Math NuGet](https://img.shields.io/nuget/v/IX.Math.svg)](https://www.nuget.org/packages/IX.Math/)

## Contributing

### Guidelines

Contributing can be done by anyone, at any time and in any form, as long as the contributor
has read the [contributing guidelines](https://adimosh.github.io/contributingguidelines)
beforehand and tries their best to abide by them.

### Code health checks

| Build | Status |
|:-----:|:------:|
| Master branch | [![Build Status](https://ixiancorp.visualstudio.com/IX.Framework/_apis/build/status/Master CI/IX.Math%20master%20CI?branchName=master)](https://ixiancorp.visualstudio.com/IX.Framework/_build/latest?definitionId=6&branchName=master) |
| Continuous integration | [![Build Status](https://ixiancorp.visualstudio.com/IX.Framework/_apis/build/status/Development CI/IX.Math%20continuous%20integration?branchName=dev)](https://ixiancorp.visualstudio.com/IX.Framework/_build/latest?definitionId=5&branchName=dev) |

## Usage

### Standard operation

This library is based on two implementations of the interface IExpressionParsingService:
- ExpressionParsingService - a parsing service that just spits out delegates on demand
- CachedExpressionParsingService - a parsing service that also caches its expressions

There is one method that is implemented in both: Interpret. This method takes in one string (and an optional [CancellationToken](https://msdn.microsoft.com/en-us/library/system.threading.cancellationtoken.aspx)) and generates a ComputedExpression object which can afterwards be used to calculate the result of the expression.

A computed expression can be parameterless (for instance ```5+6```) or parametered (for instance ```2+x```).

The ComputedExpression features two overloads of the Compute method:
- The first overload takes parameters as objects
- The second overload takes an IDataFinder, which is an interface that the library user is supposed to implement and that will fetch items by name

Each of these methods results in a possible result, or, if the expression doesn't make sense from a mathematics perspective, or if the parameters fed to it are of the wrong type, will return the original expression as a string.

### Available mathematical functions

The functions that can be invoked are:

Nonary functions:

| Function | Purpose |
|:--------:|:-------:|
| ```[numeric] rand()``` | Random number |
| ```[numeric] random()``` | Random number |
| ```[numeric-int] randomint()``` | Random integer number |

Unary functions:

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

Binary functions:

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

Ternary functions:

| Function | Purpose |
|:--------:|:-------:|
| ```[string] repl([string], [string], [string])``` | Replacing of a substring from a source string with another substring |
| ```[string] replace([string], [string], [string])``` | Replacing of a substring from a source string with another substring |
| ```[string] substr([string], [numeric-int], [numeric-int])``` | A substring between a set of indices |
| ```[string] substring([string], [numeric-int], [numeric-int])``` | A substring between a set of indices |

## Extensibility

### Functions extensibility

In order to extend the set of functions that the IX.Math library supports, a new class should be created for each function that can be invoked. For now, only unary, binary and nonary (no parameters) functions can be created, but a generalized implementation will be created soon.

Each such class should inherit from BinaryFunctionNodeBase, UnaryFunctionNodeBase and NonaryFunctionNodeBase, must be decorated with the CallableMathematicsFunctionAttribute, and their containing assembly must be registered with the IExpressionParsingService's RegisterFunctionsAssembly method.

### Extractors

There are two types of extractors available: constant extractors and pass-through extractors.

The pass-through extractors will be called when the expression is first evaluated. If the method called on it returns true, then the expression is kept as a literal string, otherwise it is interpreted.

Constant extractors work on any unidentified symbols in the expression and have the ability to define symbols otherwise not recognized by the mathematics engine.

## Licenses and structure

Please be aware that this project is a sub-project of [IX.Framework](https://github.com/adimosh/IX.Framework). All credits and license information should be taken from there.