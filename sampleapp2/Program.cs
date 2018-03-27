using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lookatme.corelib;

namespace sampleapp2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(StringDecoder.Decode("Hello World"));

            Console.Read();
        }
    }
}
