using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace lookatme
{
    class RenameField : PatternBase
    {
        private bool IsRenamable(FieldDefinition field)
        {
            // Only private fields can be renamed currently
            return field.IsPrivate;
        }

        public override void EachType(TypeDefinition type)
        {
            var offset = 0;
            var mapping = new Dictionary<string, FieldDefinition>();

            foreach (var field in type.Fields)
            {
                if (IsRenamable(field) == false) continue;

                var newName = "a" + offset;
                var newFIeld = new FieldDefinition(newName, field.Attributes, module.ImportReference(field.FieldType));

                mapping[field.Name] = newFIeld;

                offset++;
            }

            type.Fields.Clear();
            foreach (var pair in mapping)
                type.Fields.Add(pair.Value);

            foreach (var method in type.Methods)
            {
                if (method.IsIL == false) continue;

                var p = method.Body.GetILProcessor();
                for (int i=0;i<method.Body.Instructions.Count;i++)
                {
                    var inst = method.Body.Instructions[i];

                    if (inst.OpCode == OpCodes.Stfld || inst.OpCode == OpCodes.Stsfld ||
                        inst.OpCode == OpCodes.Ldsfld || inst.OpCode == OpCodes.Ldsflda ||
                        inst.OpCode == OpCodes.Ldfld || inst.OpCode == OpCodes.Ldflda || inst.OpCode == OpCodes.Ldftn)
                    {
                        var operand = (FieldReference)inst.Operand;

                        if (mapping.ContainsKey(operand.Name) == false) continue;
                        //if (operand.DeclaringType != type) continue;

                        p.Replace(inst, p.Create(inst.OpCode, mapping[operand.Name]));
                    }
                }
            }
        }
    }
}
