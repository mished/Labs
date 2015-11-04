using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDecryptor.Properties;

namespace VDecryptor
{    
    class Program
    {
        static void Main(string[] args)
        {
            var decryptor = new Decryptor(5,Resources.Source);
            //var col = decryptor.BuildColumns();
            decryptor.DO();
            Console.ReadKey();
        }
    }
}
