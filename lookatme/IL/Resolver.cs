using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Mdb;
using Mono.CompilerServices.SymbolWriter;

namespace lookatme
{
    class Resolver
    {
        public static MethodReference GetMethodReference(
            ModuleDefinition _module, string typeName, string methodName, Type[] types = null)
        {
            var modules = new ModuleDefinition[] { _module };
            foreach (var module in modules)
            {
                var candidate = module.Types
                    .Where(x => x.Name == typeName)
                    .FirstOrDefault();

                if (candidate == null)
                    continue;

                var methodCandidates = candidate.Methods
                    .Where(x => x.Name == methodName);

                foreach (var method in methodCandidates)
                {
                    if (types == null)
                        return module.ImportReference(method);
                    if (types.Length != method.Parameters.Count)
                        continue;

                    bool flag = true;
                    for (int i = 0; i < types.Length; i++)
                    {
                        if (types[i].FullName != method.Parameters[i].ParameterType.FullName)
                        {
                            flag = false;
                            break;
                        }
                    }

                    if (flag)
                        return module.ImportReference(method);
                }
            }

            return null;
        }
    }
}
