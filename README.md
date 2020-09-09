# IX.Math

## Introduction

IX.Math is a .NET library that reads mathematical formulas as text and creates delegates
and delegate placeholders for solving them at runtime.

The library is capable of interpreting any mathematical expression that makes sense from
a logical perspective, complete with a few mathematical functions. It supports integer
numbers (as [long](https://docs.microsoft.com/en-us/dotnet/api/system.int64) in standard
numeric formats), floating-point numbers (as
[double](https://docs.microsoft.com/en-us/dotnet/api/system.double)), strings (as
[string](https://docs.microsoft.com/en-us/dotnet/api/system.string)), binary data (as
[byte[]](https://docs.microsoft.com/en-us/dotnet/api/system.byte)), and boolean (as
[bool](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)) values, and can
compute the most common mathematic operations, as well as certain mathematics functions.

## What you can do with it

Let's assume that you have a text box in which a user can introduce a formula:

```
1+2
```

You need to calculate the value of what the user has typed in for later use. In order to
do that, since the user can input any formula they wish, you would need an interpreter
that works with a given set of rules and that can calculate what the user types in.

With IX.Math, you can calculate formulas with varying degrees of complexity:

```
1+2   // 3
(1+2)*5   // 15
sqrt(25)+max(2,17)+min(sqrt(45)*sin(12),floor(pi))   // 25
```

...and so on.

Even if your formula involves different data types, you can still do calculations:

```
25*strlen("abc")   // 75
substr("ABC",1,1)+"ecause of love"   // "Because of love"
"I have "+min(strlen("abc"),5)+" oranges."   // "I have 3 oranges"
```

From within your .NET software, you can go with more complex formulas that can even
contain external variables:

```
(x+2)*y
```

...where _x_ and _y_ can be provided either as parameters to a delegate that is created
for you, or as externally-held data through the use of a data finder class. Assuming
that `x=3` and `y=5`, the above formula would yield 25.

Even assuming that your data changes and you wish to plot variations accross time,
you can still use IX.Math. Let's assume that calling multiple times for the value of _x_
returns 1, then 2, then 3:

```
"The values accross time are: "+x+","+x+", and "+x   // "The values are 1, 2, and 3"
```

Logical and bitwise operations are supported:

```
x|y
x&y
x^y
```

Comparison and equation are also supported:

```
x=y
x>y
x<=y
x!=y
```

...and, if you wish to specify tolerance for these operations, there are a multitude of
ways to specify tolerance, including range and percentage.

You can even rename your own operators:

```
x add y
x subtract y
x multiply by the power of 8
x [ y
x $big$boss$ y   // We don't judge
```

...or create custom functions, interpret values in funny ways, and many many more!

## How to get

This project is primarily available through NuGet.

The current version can be accessed by using NuGet commands:

### Commands {.tabset}

#### .NET CLI

```powershell
dotnet add package IX.Math
```

#### PowerShell

```powershell
Install-Package IX.Math
```

#### Package reference

```xml
<PackageReference Include="IX.Math" Version="0.5.4" />
```

#### Paket

```powershell
paket add IX.Math
```

### Versions

| Release | Package |
|:-------:|:-------:|
| Stable | [![IX.Math NuGet](https://img.shields.io/nuget/v/IX.Math)](https://www.nuget.org/packages/IX.Math/) |
| Pre-release | [![IX.Math NuGet](https://img.shields.io/nuget/vpre/IX.Math)](https://www.nuget.org/packages/IX.Math/) |

![](https://img.shields.io/nuget/dt/IX.Math)

## Usage

The [usage guide](Usage.md) page holds information on how to use IX.Math in your software.

Additionally, consult the [extensibility page](Extensibility.md) if you need customized
behavior for IX.Math.

## Contributing

### Guidelines

Contributing can be done by anyone, at any time and in any form, as long as the
contributor has read the [contributing guidelines](https://adimosh.github.io/contributingguidelines)
beforehand and tries their best to abide by them.

### Code health checks

| Build | Status |
|:-----:|:------:|
| Master branch | [![Build Status](https://dev.azure.com/adimosh/IX.Framework/_apis/build/status/Master%20CI%20for%20IX.Math?branchName=master)](https://dev.azure.com/adimosh/IX.Framework/_build/latest?definitionId=5&branchName=master) |
| Continuous integration | [![Build Status](https://dev.azure.com/adimosh/IX.Framework/_apis/build/status/Dev%20CI%20for%20IX.Math?branchName=master)](https://dev.azure.com/adimosh/IX.Framework/_build/latest?definitionId=4&branchName=master) |

## Licenses and structure

Please be aware that this project is a sub-project of [IX.Framework](https://github.com/adimosh/IX.Framework). All credits and license information should be taken from there.