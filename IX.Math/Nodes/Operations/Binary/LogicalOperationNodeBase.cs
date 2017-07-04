// <copyright file="LogicalOperationNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using IX.Math.Nodes.Parameters;

namespace IX.Math.Nodes.Operations.Binary
{
    internal abstract class LogicalOperationNodeBase : BinaryOperationNodeBase
    {
        protected LogicalOperationNodeBase(NodeBase left, NodeBase right)
            : base(left, right)
        {
        }

        public override SupportedValueType ReturnType => this.Left.ReturnType;

        protected override void EnsureCompatibleOperands(ref NodeBase left, ref NodeBase right)
        {
            if (left is UndefinedParameterNode uLeft)
            {
                if (right is UndefinedParameterNode uRightInternal)
                {
                    left = uLeft.IfDeterminedNumericAlsoDetermineInteger();
                    right = uRightInternal.IfDeterminedNumericAlsoDetermineInteger();
                }
                else
                {
                    switch (right.ReturnType)
                    {
                        case SupportedValueType.Numeric:
                            left = uLeft.DetermineNumeric().ParameterMustBeInteger();
                            break;

                        case SupportedValueType.Boolean:
                            left = uLeft.DetermineBool();
                            break;

                        case SupportedValueType.Unknown:
                            break;

                        default:
                            throw new ExpressionNotValidLogicallyException();
                    }
                }
            }

            if (right is UndefinedParameterNode uRight)
            {
                switch (left.ReturnType)
                {
                    case SupportedValueType.Numeric:
                        right = uRight.DetermineNumeric().ParameterMustBeInteger();
                        break;

                    case SupportedValueType.Boolean:
                        right = uRight.DetermineBool();
                        break;

                    case SupportedValueType.Unknown:
                        break;

                    default:
                        throw new ExpressionNotValidLogicallyException();
                }
            }

            if (!((left.ReturnType == SupportedValueType.Numeric && right.ReturnType == SupportedValueType.Numeric) || (left.ReturnType == SupportedValueType.Boolean && right.ReturnType == SupportedValueType.Boolean)))
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }
    }
}