using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace lookatme
{
    /* 스트링 리터럴을 암호화한다.
     *   스트링 리터럴은 디어셈블리 시 바로 노출되므로
     *   암호화 해서 알기 어렵게 한다.
     *   
     * flow
     *   1. 암호화/복호화 코드를 타겟 어셈블리에 Inject한다. (See Injector.cs)
     *   2. 모든 Ldstr을 
     *         ldstr "ENCODED STRING"
     *         call  StringEncoder.Decode
     *      로 치환한다.
     */
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
