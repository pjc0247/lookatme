using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sampleapp
{
    class Program
    {
        private static int balue = 1;

        static int Foo(int a, int b)
        {
            return a + b;
        }

        static void Main(string[] args)
        {
            Console.WriteLine(balue);
            Console.Write(Foo(5, 10));
            Console.WriteLine("Hello World");

            Console.Read();
        }
    }
}
