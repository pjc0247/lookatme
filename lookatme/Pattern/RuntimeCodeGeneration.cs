using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Newtonsoft.Json;

namespace lookatme
{
    class RuntimeCodeGeneration : PatternBase
    {
        private class StoredInstruction
        {
            public OpCode opcode;
            public object operand;
        }
        private class StoredFieldReference
        {
            public string name;
        }
        private class StoredMethodReference
        {
            public string name;
            public StoredTypeReference returnType;
        }
        private class StoredVariableReference
        {
            public StoredTypeReference type;
        }
        private class StoredTypeReference
        {
            public string ns;
            public string name;
        }
        private class MethodData
        {
            public StoredInstruction[] instructions;
        }

        public override void EachType(TypeDefinition type)
        {
            foreach (var method in type.Methods)
            {
                var sinst = new List<StoredInstruction>();
                foreach (var inst in method.Body.Instructions)
                {
                    if (inst.Operand is Instruction)
                        continue;

                    var _op = inst.Operand;
                    if (_op is TypeReference)
                    {
                        var op = (TypeReference)_op;
                        _op = new StoredTypeReference()
                        {
                            name = op.Name,
                            ns = op.Namespace
                        };
                    }
                    else if (_op is FieldReference)
                    {
                        var op = (FieldReference)_op;
                        _op = new StoredFieldReference()
                        {
                            name = op.Name
                        };
                    }
                    else if (_op is MethodReference)
                    {
                        var op = (MethodReference)_op;
                        _op = new StoredMethodReference()
                        {
                            name = op.Name
                        };
                    }
                    else if (_op is VariableReference)
                    {
                        var op = (VariableReference)_op;
                        _op = new StoredVariableReference()
                        {
                        };
                    }

                    sinst.Add(new StoredInstruction()
                    {
                        opcode = inst.OpCode,
                        operand = _op
                    });

                    if (inst.Operand != null)
                    Console.WriteLine(inst.Operand.GetType());
                }

                var methodData = new MethodData()
                {
                    instructions = sinst.ToArray()
                };

                
                Console.WriteLine(JsonConvert.SerializeObject(methodData, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }));
            }
        }
    }
}
