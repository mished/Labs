using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDecryptor
{
    public class Decryptor
    {
        private int keyLength;

        public Decryptor(int keyLength)
        {
            this.keyLength = keyLength;
        }
        public string GetSource()
        {
           return Properties.Resources.Source;
        }
                
    }
}
