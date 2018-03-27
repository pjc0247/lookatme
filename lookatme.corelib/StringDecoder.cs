using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lookatme.corelib
{
    public class StringDecoder
    {
        public static string Encode(string input)
        {
            var buffer = input.ToCharArray();
            for (int i = 0; i < input.Length; i++)
                buffer[i] = (char)(buffer[i] ^ 26);
            return new string(buffer);
        }
        public static string Decode(string input)
        {
            var buffer = input.ToCharArray();
            for (int i = 0; i < input.Length; i++)
                buffer[i] = (char)(buffer[i] ^ 26);
            return new string(buffer);
        }
    }   
}
