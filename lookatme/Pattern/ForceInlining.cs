using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace lookatme
{
    /* 함수 콜을 강제적으로 인라이닝한다.
     *   함수 콜을 하지 않기 떄문에 함수 이름으로부터 힌트를 얻을 수 없고
     *   한개의 메소드가 매우 길어져서 읽기 힘들게 된다.
     * 
     * flow
     *   1. 만약 instance call이면 ldarg0 역할을 할 지역변수를 만든다.
     *   2. 호출 전에는 파라미터가 스택에 저장되어 있으므로 해당 파라미터를 지역변수에 모두 담는다.
     *   3. 인스트럭션을 순회하면서 ret은 jmp로 변경한다,
     *   4. ldarg는 1번에서 저장한 지역변수에서 가져오도록 변경한다.
     */
    class ForceInlining : PatternBase
    {
        public override void EachMethod(MethodDefinition method)
        {
            // This breaks callstack, So never inlining methods with ExceptionHandlers.
            if (method.Body.HasExceptionHandlers) return;
            if (method.IsIL == false) return;

            method.Body.InitLocals = true;

            var p = method.Body.GetILProcessor();
            var instructions = method.Body.Instructions;
            for (int i = 0; i < instructions.Count; i++)
            {
                var il = instructions[i];

                if (il.IsCall() == false) continue;

                var operand = ((MethodReference)il.Operand).Resolve();
                var offset = i;
                var parameters = new List<VariableDefinition>();

                if (operand.IsIL == false || operand.Body == null) continue;

                // 스택을 지역 변수에 저장해야 하므로 파라미터만큼 지역변수를 만든다.
                foreach (var v in operand.Body.Variables)
                    method.Body.Variables.Add(v);

                // Instance call needs `this(arg0)`.
                if (operand.IsStatic == false)
                    parameters.Add(new VariableDefinition(module.ImportReference(operand.DeclaringType)));
                foreach (var v in operand.Parameters)
                {
                    parameters.Add(new VariableDefinition(module.ImportReference(v.ParameterType)));
                    method.Body.Variables.Add(parameters.Last());
                }

                var nopTarget = p.Create(OpCodes.Nop);
                p.Replace(il, nopTarget);

                // STACK => LOCAL
                var instrcutionsToCopy = operand.Body.Instructions;
                foreach (var param in parameters)
                {
                    instructions.Insert(i, p.Create(OpCodes.Stloc, param));

                    offset++;
                    i++;
                }
                i--;
                // MODIFY INSTRUCTIONS
                foreach (var inst in instrcutionsToCopy)
                {
                    // Ret must be changed to `simple jump`
                    if (inst.OpCode == OpCodes.Ret)
                        instructions.Insert(offset, p.Create(OpCodes.Br_S, nopTarget));

                    // Ldarg must be changed to `ldloc`.
                    else if (inst.IsLdarg())
                    {
                        var idx = 0;
                        if (inst.OpCode == OpCodes.Ldarg_0) idx = 0;
                        else if (inst.OpCode == OpCodes.Ldarg_1) idx = 1;
                        else if (inst.OpCode == OpCodes.Ldarg_2) idx = 2;
                        else if (inst.OpCode == OpCodes.Ldarg_3) idx = 3;
                        else idx = (byte)inst.Operand;

                        instructions.Insert(offset, p.Create(OpCodes.Ldloc, parameters[idx]));
                    }

                    // Just copy
                    else
                        instructions.Insert(offset, inst.Clone(module));

                    offset++;
                    i++;
                }
                i--;
            }
        }
    }
}
