// <copyright file="StandardParameterRegistry.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Globalization;
using System.Linq;
using IX.StandardExtensions.Efficiency;

namespace IX.Math.Registration
{
    internal class StandardParameterRegistry : IParameterRegistry
    {
        private readonly ConcurrentDictionary<string, ParameterContext> parameterContexts;

        public StandardParameterRegistry()
        {
            this.parameterContexts = new ConcurrentDictionary<string, ParameterContext>();
        }

        public bool Populated => this.parameterContexts.Count > 0;

        public ParameterContext AdvertiseParameter(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            return this.parameterContexts.GetOrAdd(name, (nameL1) => new ParameterContext(nameL1));
        }

        public ParameterContext CloneFrom(ParameterContext previousContext)
        {
            if (previousContext == null)
            {
                throw new ArgumentNullException(nameof(previousContext));
            }

            var name = previousContext.Name;
            if (this.parameterContexts.TryGetValue(name, out ParameterContext existingValue))
            {
                if (existingValue.Equals(previousContext))
                {
                    return existingValue;
                }

                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.ParameterAlreadyAdvertised, name));
            }

            ParameterContext newContext = previousContext.DeepClone();

            this.parameterContexts.TryAdd(name, newContext);

            return newContext;
        }

        public ParameterContext[] Dump() => this.parameterContexts.ToArray().Select(p => p.Value).OrderBy(p => p.Order).ToArray();

        public bool Exists(string name) => this.parameterContexts.ContainsKey(name);
    }
}