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
    static class IlHelper
    {
        public static Instruction Clone(this Instruction inst, ModuleDefinition module)
        {
            if (inst.Operand is TypeReference)
                inst.Operand = module.ImportReference((TypeReference)inst.Operand);
            else if (inst.Operand is FieldReference)
                inst.Operand = module.ImportReference((FieldReference)inst.Operand);

            if (inst.OpCode == OpCodes.Newobj ||
                inst.OpCode == OpCodes.Callvirt || inst.OpCode == OpCodes.Calli || inst.OpCode == OpCodes.Call)
            {
                inst.Operand = module.ImportReference((MethodReference)inst.Operand);
            }

            return inst;
        }

        public static bool IsCall(this Instruction inst)
        {
            return inst.OpCode == OpCodes.Call || inst.OpCode == OpCodes.Calli || inst.OpCode == OpCodes.Callvirt;
        }
        public static bool IsLdarg(this Instruction inst)
        {
            return inst.OpCode == OpCodes.Ldarg
                || inst.OpCode == OpCodes.Ldarga || inst.OpCode == OpCodes.Ldarga_S
                || inst.OpCode == OpCodes.Ldarg_0 || inst.OpCode == OpCodes.Ldarg_1
                || inst.OpCode == OpCodes.Ldarg_2 || inst.OpCode == OpCodes.Ldarg_3
                || inst.OpCode == OpCodes.Ldarg_S;
        }
        public static bool IsLdloc(this Instruction inst)
        {
            return inst.OpCode == OpCodes.Ldloc
                || inst.OpCode == OpCodes.Ldloca || inst.OpCode == OpCodes.Ldloca_S
                || inst.OpCode == OpCodes.Ldloc_0 || inst.OpCode == OpCodes.Ldloc_1
                || inst.OpCode == OpCodes.Ldloc_2 || inst.OpCode == OpCodes.Ldloc_3
                || inst.OpCode == OpCodes.Ldloc_S;
        }
        public static bool IsStloc(this Instruction inst)
        {
            return inst.OpCode == OpCodes.Stloc
                || inst.OpCode == OpCodes.Stloc_0 || inst.OpCode == OpCodes.Stloc_1 || inst.OpCode == OpCodes.Stloc_2
                || inst.OpCode == OpCodes.Stloc_3 || inst.OpCode == OpCodes.Stloc_S;
        }
    }
}
