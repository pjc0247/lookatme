using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace lookatme
{
    class EncodeStringLiteral : PatternBase
    {
        public override void EachMethod(MethodDefinition method)
        {
            var p = method.Body.GetILProcessor();

            var instructions = method.Body.Instructions;
            for (int i = 0; i < instructions.Count; i++)
            {
                var il = instructions[i];

                if (il.Operand is string)
                {
                    instructions.RemoveAt(i);
                    instructions.Insert(i, p.Create(OpCodes.Ldstr, corelib.StringDecoder.Encode((string)il.Operand)));
                    instructions.Insert(i + 1, p.Create(OpCodes.Call, Resolver.GetMethodReference(module, nameof(corelib.StringDecoder), nameof(corelib.StringDecoder.Decode))));
                    i++;
                }
            }
        }
    }
}
