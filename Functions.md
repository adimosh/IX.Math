# IX.Math functions extensibility

## Basic definition

Each function definition is represented by a new class in a scannable assembly.

The newly-created class should inherit from one of the base classes that define functions
usable by the engine: [NonaryFunctionNodeBase](src/IX.Math/Nodes/NonaryFunctionNodeBase.cs),
[UnaryFunctionNodeBase](src/IX.Math/Nodes/UnaryFunctionNodeBase.cs),
[BinaryFunctionNodeBase](src/IX.Math/Nodes/BinaryFunctionNodeBase.cs)
or [TernaryFunctionNodeBase](src/IX.Math/Nodes/TernaryFunctionNodeBase.cs).

For now, only unary, binary, ternary and nonary (no parameters) functions can be created,
but a generalized implementation is planned. This design is currently used for simplifying
the node graph processing while the library is still not mature enough.

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

## Constructor

Each base class supports a number of parameters, and has a constructor that requires
that exact number of arguments. It is recommended that there be a constructor with
the same argument arrangement in the extensibility class.

For the sake of simplification, we will take a class derived from
[BinaryFunctionNodeBase](src/IX.Math/Nodes/BinaryFunctionNodeBase.cs) as a concrete
example.

The two parameter nodes come from the engine, interpreted as separate expressions,
and are automatically assigned when the class is instantiated. The exact return type
of each node can be consulted by checking out their ```ReturnType``` property.

A virtual method, ```EnsureCompatibleParameters```, is called next. Its role,
if overridden, is to ensure that the nodes received as parameters, are compatible,
and, should any of them be a parameter node, to also ensure that they are forced
to be what they need to be (if this is the case).

At this time, the class can signal to the engine that whatever it's trying to do
does not make sense from a logical perspective. If an exception of type
[ExpressionNotValidLogicallyException](src/IX.Math/ExpressionNotValidLogicallyException.cs)
(representing an expression that does not make sense, or does not make sense in that
particular context) or [FunctionCallNotValidLogicallyException](src/IX.Math/FunctionCallNotValidLogicallyException.cs)
(representing a function call that is done in a way that does not make sense, such
as invoking with a wrong number of parameters, or a parameter that cannot be coerced
even if it should be) is thrown, the engine stops evaluating this branch and either
looks for an alternative branch, or, if one is not found, stops evaluating altogether.

Suppose that we have two have two numbers for our function. If that is the case,
our method would look something like this:

```csharp
protected override void EnsureCompatibleParameters(
    ref NodeBase firstParameter,
    ref NodeBase secondParameter)
{
    if (firstParameter is ParameterNode up1)
    {
        firstParameter = up1.DetermineNumeric();
    }

    if (secondParameter is ParameterNode up2)
    {
        secondParameter = up2.DetermineNumeric();
    }

    if (firstParameter.ReturnType != SupportedValueType.Numeric)
    {
        throw new ExpressionNotValidLogicallyException();
    }

    if (secondParameter.ReturnType != SupportedValueType.Numeric)
    {
        throw new ExpressionNotValidLogicallyException();
    }
}
```

```Simplify``` is then called on the nodes in order to obtain the smallest possible
node graph.

## Return type

The return type of any node is critical, as it ensures compatibility with the nodes
that they work with further down the tree.

The ```ReturnType``` must be a value of the enumeration [SupportedValueType](src/IX.Math/SupportedValueType.cs).
The returned value can be dynamic (depending on the input parameters).

Overriding this property is mandatory.

As a simple example:

```csharp
public override SupportedValueType ReturnType => SupportedValueType.Numeric;
```

## Deep cloning

The ```DeepClone``` method is a method that must be overridden. It is called by the
engine whenever an interpretation context is cloned (such as when an expression that has
parameters is computed with a set of actual parameter values.

The method must return another (**different**) instance of the same logical node, with
everything cloned. The method receives an instance of the [NodeCloningContext](src/IX.Math/Nodes/NodeCloningContext.cs)
class, which contains pertinent cloning context information, such as the parameter
registry, which the node would otherwise not have access to during cloning.

In our example, the implementation would look somewhat similar to:

```csharp
public override NodeBase DeepClone(NodeCloningContext context) => new ScrtFunctionNode(
    this.FirstParameter.DeepClone(context),
    this.SecondParameter.DeepClone(context));
```

## Simplification

A really important part of the IX.Math library is the ability to simplify graphs.

Graph simplification means reducing the expression graph to fewer nodes whenever that is
possible. The expression graph is compiled into executable code, so making it as small as
possible is a major performance goal. Even though the JIT can make optimizations on its
own, the nature of dynamically-compiled expressions makes it difficult to use many
of the built-in optimization scenarios used by JIT.

The ```Simplify``` method is used for this purpose. The method takes no arguments, and
is expected to return an instance of a node. If no simplification is possible, then
reflexive return (return of ```this```) is expected.

**Warning!!!** Returning ```null``` (```Nothing``` in Visual Basic) will fully
invalidate the node graph on all branches of possibility. Such a situation is not
desirable in any case, so please refrain from such a return.

For our example, the simplify method would look like this:

```csharp
public override NodeBase Simplify() => this;
```

However, as an example, let's look at how the ```log([numeric], [numeric])``` function
simplifies:

```csharp
public override NodeBase Simplify() =>
    this.FirstParameter is NumericNode firstParam && this.SecondParameter is NumericNode secondParam
        ? new NumericNode(
            global::System.Math.Log(
                firstParam.ExtractFloat(),
                secondParam.ExtractFloat()))
        : (NodeBase)this;
```

The type ```NumericNode``` represents a numeric constant. The ternary conditional
operator would choose the ```false``` branch (and therefore return reflexively) in
all scenarios except one: the scenario in which the graph is reduced to calling the
logarithm function with two constants.

In such a scenario, there is no point in emitting code that does the calculation.
Actually calling ```System.Math.Log(x, y)``` whenever the expression is computed
is both time-consuming and non-optimizable by the JIT. Since we're here, and since
building the graph will obviously consume a great deal of our time, we might as well
simplify this group of three nodes to just one, which is a constant with the value
equivalent to the logarithm with the two desired operands. In turn, this constant
expression will be evaluated further down the tree, resulting in possibly even more
simplification.

Currently-planned features (checked means in progress):

- [ ] polynomial simplification
- [ ] multi-level node simplification
- [ ] usual expressions simplification
 
## Expression generation

Two types of expressions must be generated: a value expression and a string expression.

The value expression, generated in an override of the ```GenerateExpression()```
method, represents the actual value of the calculation, whereas the string expression,
generated in an override of the ```GenerateStringExpression()``` method, represents
the string representation of the calculated value.

In certain circumstances, the string representation is required, and might be different
than just calling an ```object.ToString()``` method. In such cases, the string
expression is used. An example of this is when concatenating a string with something
else via the + operator.

The principle of expression generation is to have a ```System.Linq.Expressions.Expression```
to return at the end of the method call, which can fit into a larger expression graph.
Again, the kind of expression that is generated depends on the nature of the function
you're trying to implement. You can think of it as returning an expression that
represents what the compiled code of your function would do.

Optionally, this can even mean an actual method call. Each base class offers some helper
methods that can generate an expression for you. For an example, please look at the
[BinaryFunctionNodeBase](src/IX.Math/Nodes/BinaryFunctionNodeBase.cs)'s
```GenerateStaticBinaryFunctionCall``` method.

For example, let's assume that our function's inner workings are represented in a static
method called ```MyClass.DoScrt```. We would then have the following code:

```csharp
protected override Expression GenerateExpressionInternal() =>
    this.GenerateStaticBinaryFunctionCall(
        typeof(MyNamespace.MyClass),
        nameof(MyNamespace.MyClass.DoScrt));
```

For further information about expression trees, the following documentation is available
and should be read prior to taking on the task of creating a function:

- [Expression Trees](https://docs.microsoft.com/en-us/dotnet/csharp/expression-trees)
- [Expression Trees (C#)](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/expression-trees/)
- [How to: Use Expression Trees to Build Dynamic Queries (C#)](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/expression-trees/how-to-use-expression-trees-to-build-dynamic-queries)
- [Understanding Expression and Expression Trees](https://www.dotnettricks.com/learn/linq/understanding-expression-and-expression-trees)

## Tolerance pass-through

`GenerateExpressionInternal` has an overload which takes in a class of type `Tolerance`
as a parameter. It is intended as a way to use and control pass-through of tolerance, when
asked for by the caller.

Basically, when dealing with a function that can do something with tolerance, the value
of the tolerance specified in the calling context of the expression that is currently being
interpreted can be used within this method.

Also, pass-through is controller, which ensures that, if you have parameters for the
function, they may or may not receive tolerance information, depending on the needs of
the particular implementation.

If tolerane isn't sent to parameters, any sub-expression that depends on it will not get
access to it. Calculation will be done in the default way, meaning that exact matches
will be performed. This, however, does not include sub-expressions that can get their
tolerance information elsewhere, such as already-computed expressions which had access to
tolerance.