using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lookatme
{
    class Program
    {
        static void Main(string[] args)
        {
            Obfuscator.Run(
                "..\\..\\..\\sampleapp\\bin\\Debug\\sampleapp.exe",
                "output.exe");
        }
    }
}
