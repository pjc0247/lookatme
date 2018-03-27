using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;

namespace lookatme
{
    class PatternBase
    {
        protected ModuleDefinition module;

        public void Execute(ModuleDefinition _module)
        {
            module = _module;

            Apply(module);

            foreach (var type in module.Types)
            {
                EachType(type);

                foreach (var method in type.Methods)
                    EachMethod(method);
            }
        }

        public virtual void Apply(ModuleDefinition module)
        {
            /* Override */
        }

        public virtual void EachType(TypeDefinition type)
        {
            /* Override */
        }
        public virtual void EachMethod(MethodDefinition method)
        {
            /* Override */
        }
    }
}
