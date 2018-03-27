using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace lookatme
{
    /// <summary>
    /// Combines two assembly into one
    /// </summary>
    public class Injector
    {
        public static void Inject(ModuleDefinition module, Assembly asm)
        {
            var moduleToInect = ModuleDefinition.ReadModule(asm.Location);

            foreach (var type in moduleToInect.Types)
            {
                if (module.Types.Any(x => x.FullName == type.FullName))
                    continue;

                module.Types.Add(CloneType(module, type));
            }
        }

        private static MethodDefinition CloneMethod(ModuleDefinition module, MethodDefinition method)
        {
            var clone = new MethodDefinition(method.Name, method.Attributes, method.ReturnType);
            clone.Body = new Mono.Cecil.Cil.MethodBody(method)
            {
                InitLocals = method.Body.InitLocals,
                LocalVarToken = method.Body.LocalVarToken,
                MaxStackSize = method.Body.MaxStackSize
            };

            clone.Body.Instructions.Clear();
            foreach (var inst in method.Body.Instructions)
            {
                if (inst.OpCode == OpCodes.Newobj || inst.IsCall())
                    inst.Operand = module.ImportReference((MethodReference)inst.Operand);

                clone.Body.Instructions.Add(inst);
            }
            foreach (var p in method.Parameters)
                clone.Parameters.Add(p);
            foreach (var eh in method.Body.ExceptionHandlers)
                clone.Body.ExceptionHandlers.Add(eh);
            foreach (var v in method.Body.Variables)
                clone.Body.Variables.Add(v);

            return clone;
        }
        private static TypeDefinition CloneType(ModuleDefinition module, TypeDefinition type)
        {
            var clone = new TypeDefinition(type.Namespace, type.Name, type.Attributes);

            foreach (var method in type.Methods)
                clone.Methods.Add(CloneMethod(module, method));

            clone.BaseType = type.BaseType;

            return clone;
        }
    }
}
