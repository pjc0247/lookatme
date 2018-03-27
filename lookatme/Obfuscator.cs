using System;
using System.Reflection;
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
    public class Obfuscator
    {
        public static void Run(string inputPath, string outputPath)
        {
            try
            {
                var module = ModuleDefinition.ReadModule(inputPath);

                foreach (var rf in module.AssemblyReferences)
                    Console.WriteLine(rf);

                Injector.Inject(module, typeof(lookatme.corelib.AssemblyTag).Assembly);

                foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
                {
                    if (type.BaseType != (typeof(PatternBase))) continue;

                    var inst = Activator.CreateInstance(type);
                    typeof(PatternBase).GetMethod(nameof(PatternBase.Execute)).Invoke(inst, new object[] { module });
                }

                module.Write(outputPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
