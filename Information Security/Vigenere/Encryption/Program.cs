using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Encryption
{
    class Program
    {
        static void Main(string[] args)
        {
            var encripter = new MyEnc("MySuperKey", "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            var encripted = encripter.Encrypt("Hello, World!");
            var encriptedString = String.Join("", encripted);
            Console.WriteLine(encriptedString);

            var decripted = encripter.Decrypt(encriptedString);
            Console.WriteLine(String.Join("", decripted));
        }
    }
}
