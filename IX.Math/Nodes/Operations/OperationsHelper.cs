// <copyright file="OperationsHelper.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using IX.Math.Nodes.Parameters;

namespace IX.Math.Nodes.Operations
{
    internal static class OperationsHelper
    {
        internal static void ParameterMustBeFloat(NumericParameterNode parameter)
        {
            if (parameter.RequireFloat != null)
            {
                if (!parameter.RequireFloat.Value)
                {
                    throw new InvalidOperationException(string.Format(Resources.ParameterMustBeFloatButAlreadyRequiredToBeInteger, parameter.ParameterName));
                }
            }
            else
            {
                parameter.RequireFloat = true;
            }
        }

        internal static void ParameterMustBeInteger(NumericParameterNode parameter)
        {
            if (parameter.RequireFloat != null)
            {
                if (parameter.RequireFloat.Value)
                {
                    throw new InvalidOperationException(string.Format(Resources.ParameterMustBeIntegerButAlreadyRequiredToBeFloat, parameter.ParameterName));
                }
            }
            else
            {
                parameter.RequireFloat = false;
            }
        }
    }
}