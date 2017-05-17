# IX.Math

## Introduction

IX.Math is a .NET library that reads mathematical formulas as text and creates delegates and delegate placeholders for solving them at runtime.

The library is capable of interpreting any mathematical expression that makes sense from a logical perspective, complete with a few mathematical functions. It
supports integer numbers (as [long](https://msdn.microsoft.com/en-us/library/system.int64.aspx) in standard numeric formats), floating-point numbers (as
[double](https://msdn.microsoft.com/en-us/library/system.double.aspx), strings (as [string](https://msdn.microsoft.com/en-us/library/system.string.aspx)) and
boolean (as [bool](https://msdn.microsoft.com/en-us/library/system.boolean.aspx)) values, and can compute the most common mathemtic operations, as well as
certain mathematics functions.

## Code health
- Build status: [![Build status](https://ci.appveyor.com/api/projects/status/dq2codv2mo32le37?svg=true)](https://ci.appveyor.com/project/adimosh/ix-math)
- Master branch status: [![Build status](https://ci.appveyor.com/api/projects/status/dq2codv2mo32le37/branch/master?svg=true)](https://ci.appveyor.com/project/adimosh/ix-math/branch/master)

## Usage

This library is based on two implementations of the interface IExpressionParsingService:
- ExpressionParsingService - a parsing service that just spits out delegates on demand
- CachedExpressionParsingService = a parsing service that also caches its expressions

There is one method that is implemented in both: Interpret. This method takes in one string (and an optional
[CancellationToken](https://msdn.microsoft.com/en-us/library/system.threading.cancellationtoken.aspx)) and generates a ComputedExpression object which can afterwards
be used to calculate the result of the expression.

A computed expression can be parameterless (for instance 5+6) or parametered (for instance 2+x).

The ComputedExpression features two overloads of the Compute method:
- The first overload takes parameters as objects
- The second overload takes an IDataFinder, which is an interface that the library user is supposed to implement and that will fetch items by name

Each of these methods results in a possible result, or, if the expression doesn't make sense from a mathematics perspective, or if the parameters fed to it are
of the wrong type, will return the original expression as a string.

## How to get

This project is primarily available through NuGet.

The current version can be accessed by using NuGet commands:

```powershell
Install-Package IX.Math
```

Releases: [![IX.Math NuGet](https://img.shields.io/nuget/v/IX.Math.svg)](https://www.nuget.org/packages/IX.Math/)

## Documentation

Documentation is currently in progress.

## Contributing

Contributing can be done by anyone, at any time and in any form, as long as the contributor
has read the [contributing guidelines](https://adimosh.github.io/contributingguidelines)
beforehand and tries their best to abide by them.

## Developer guidelines

The project builds in Visual Studio 2017 and uses some of the language enhancements VS2017 brought. The project structure also follows the .NET Core CSPROJ standard.

Since the tooling is very difficult to work with and mostly unavailable, Visual Studio 2017 has been chosen as a suitable IDE with a good-enough project structure
for the purposes of this project. There are no plans to port this to earlier editions of Visual Studio.

Visual Studio Code should, to the extent of my knowledge, also work (at least for vanilla code changes), but I do not currently work with that IDE, instead focusing
on development with the familiar IDE that I use in commercial development at my daily job.

The project is and will be exclusive to the .NET Standard. For now, there is no point in adding further targets than the .NET Standard 1.1, which provides the
highest level of compatibility. Should any special build be required in the future, please point it out, as well as giving a reason/scenario in which things did not
work out with the current targets. Such questions and comments are always welcome, since I cannot commit to developing on all available platforms and operating systems
at the same time.

## Acknowledgements

This project uses the following libraries:

- .NET Framework Core, available from the [.NET Foundation](https://github.com/dotnet)
- StyleCop analyzer, available from [its GitHub page](https://github.com/DotNetAnalyzers/StyleCopAnalyzers)
- xunit.net, available from [its GitHub page](http://xunit.github.io/)

This project uses the following tools:

- [Visual Studio](https://www.visualstudio.com/) Community Edition [2017 RC](https://www.visualstudio.com/vs/visual-studio-2017-rc/)
- GhostDoc, available at [SubMain's website](http://submain.com/products/ghostdoc.aspx)
- Mads Kristensen's fabulous and numerous tools and extensions, which are too many to name and are available at [his GitHub page](https://github.com/madskristensen/)

There is also [EditorConfig](http://editorconfig.org/) support and an .editorconfig file included that works with Visual Studio 2017's baked-in support.

The project is hosted by [GitHub](https://github.com) and its build server is powered by [AppVeyor](https://www.appveyor.com/).