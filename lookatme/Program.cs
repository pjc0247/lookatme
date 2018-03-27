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
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var module = ModuleDefinition.ReadModule("..\\..\\..\\sampleapp\\bin\\Debug\\sampleapp.exe");

                foreach (var rf in module.AssemblyReferences)
                    Console.WriteLine(rf);

                Injector.Inject(module, typeof(lookatme.corelib.AssemblyTag).Assembly);

                foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
                {
                    if (type.BaseType != (typeof(PatternBase))) continue;

                    var inst = Activator.CreateInstance(type);
                    typeof(PatternBase).GetMethod(nameof(PatternBase.Execute)).Invoke(inst, new object[] { module });
                }

                module.Write("output.exe");
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
