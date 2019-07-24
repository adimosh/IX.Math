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

Contributing can be done by anyone, at any time and in any form, as long as the
contributor has read the [contributing guidelines](https://adimosh.github.io/contributingguidelines)
beforehand and tries their best to abide by them.

### Code health checks

| Build | Status |
|:-----:|:------:|
| Master branch | [![Build Status](https://ixiancorp.visualstudio.com/IX.Framework/_apis/build/status/Master%20CI/IX.Math%20master%20CI?branchName=master)](https://ixiancorp.visualstudio.com/IX.Framework/_build/latest?definitionId=6&branchName=master) |
| Continuous integration | [![Build Status](https://ixiancorp.visualstudio.com/IX.Framework/_apis/build/status/Development%20CI/IX.Math%20continuous%20integration?branchName=dev)](https://ixiancorp.visualstudio.com/IX.Framework/_build/latest?definitionId=5&branchName=dev) |

## Usage

Please check out the [usage guide](Usage.md) for further information on how to use
IX.Math.

## Extensibility

Please check out the [extensibility page](Extensibility.md) for further information on
how IX.Math can be extended.

## Licenses and structure

Please be aware that this project is a sub-project of [IX.Framework](https://github.com/adimosh/IX.Framework). All credits and license information should be taken from there.