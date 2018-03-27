using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace lookatme
{
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

                var operand = (MethodReference)il.Operand;
                var offset = i;
                var parameters = new List<VariableDefinition>();

                if (operand.Resolve().IsIL == false || operand.Resolve().Body == null) continue;

                foreach (var v in operand.Resolve().Body.Variables)
                    method.Body.Variables.Add(v);

                if (il.OpCode == OpCodes.Calli || il.OpCode == OpCodes.Callvirt)
                    parameters.Add(new VariableDefinition(module.ImportReference(operand.DeclaringType)));
                foreach (var v in operand.Resolve().Parameters)
                {
                    parameters.Add(new VariableDefinition(module.ImportReference(v.ParameterType)));
                    method.Body.Variables.Add(parameters.Last());
                }

                var nopTarget = p.Create(OpCodes.Nop);
                p.Replace(il, nopTarget);

                // STACK => LOCAL
                var instrcutionsToCopy = operand.Resolve().Body.Instructions;
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
                        if (inst.OpCode == OpCodes.Ldarg_1) idx = 1;
                        if (inst.OpCode == OpCodes.Ldarg_2) idx = 2;
                        if (inst.OpCode == OpCodes.Ldarg_3) idx = 3;
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
