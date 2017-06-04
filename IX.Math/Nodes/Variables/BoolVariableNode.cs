//using System;
//using System.Collections.Generic;
//using System.Linq.Expressions;
//using System.Text;

//namespace IX.Math.Nodes.Variables
//{
//    /// <summary>
//    /// A boolean variable node.
//    /// </summary>
//    /// <seealso cref="IX.Math.Nodes.VariableNodeBase" />
//    public class BoolVariableNode : VariableNodeBase
//    {
//        /// <summary>
//        /// Initializes a new instance of the <see cref="BoolVariableNode"/> class.
//        /// </summary>
//        /// <param name="referenceNode">The reference node.</param>
//        public BoolVariableNode(NodeBase referenceNode)
//            : base(referenceNode)
//        { }

//        /// <summary>
//        /// Gets the return type of this node.
//        /// </summary>
//        /// <value>The node return type.</value>
//        public override SupportedValueType ReturnType => SupportedValueType.Boolean;

//        //protected override Expression GenerateExpressionInternal() => Expression.Assign
//    }
//}