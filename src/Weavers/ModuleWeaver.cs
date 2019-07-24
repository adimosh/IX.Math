using System;
using System.Collections.Generic;
using System.Linq;
using Fody;
using Mono.Cecil.Cil;

namespace Weavers
{
    public class ModuleWeaver : BaseModuleWeaver
    {
        /// <summary>
        /// Return a list of assembly names for scanning.
        /// Used as a list for <see cref="P:Fody.BaseModuleWeaver.FindType" />.
        /// </summary>
        public override IEnumerable<string> GetAssembliesForScanning() => Enumerable.Empty<string>();

        /// <summary>Called when the weaver is executed.</summary>
        public override void Execute()
        {
            foreach (var currentType in ModuleDefinition.GetTypes().Where(p => p.HasMethods))
            {
                foreach (var currentMethod in currentType.Methods)
                {
                    var processor = currentMethod.Body.GetILProcessor();
                    foreach (var currentInstruction in currentMethod.Body.Instructions)
                    {
                        if (currentInstruction.OpCode.Code == Code.Call)
                        {
                        }
                    }
                }
            }
        }
    }
}
