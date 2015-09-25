using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Encryption
{
    public class MyEnc
    {        
        private string alphabet;
        private string lowerAlph;
        private string upperAlph;
        private string key;
        private int keyPos;
        
        public MyEnc(string key, string alphabet) {
            this.key = key;
            this.alphabet = alphabet;
            lowerAlph = alphabet.ToLower();
            upperAlph = alphabet.ToUpper();
        }

        public IEnumerable<char> Encrypt(IEnumerable<char> text) {
            
            foreach(var ch in text) {
                var alphabet = Char.IsUpper(ch) ? upperAlph : lowerAlph;

                if (!Char.IsLetter(ch)) {
                    yield return ch;
                    continue;
                }

                var encIndex = alphabet.IndexOf(ch) + alphabet.IndexOf(GetKeyChar());
                encIndex = encIndex >= alphabet.Length ? encIndex - alphabet.Length : encIndex;
                yield return alphabet[encIndex];
            }

            keyPos = 0;
        }

        public IEnumerable<char> Decrypt(IEnumerable<char> text) {
            foreach (var ch in text) {
                var alphabet = Char.IsUpper(ch) ? upperAlph : lowerAlph;

                if (!Char.IsLetter(ch)) {
                    yield return ch;
                    continue;
                }

                var decIndex = alphabet.IndexOf(ch) - alphabet.IndexOf(GetKeyChar());
                decIndex = decIndex < 0 ? decIndex + alphabet.Length : decIndex; 
                yield return alphabet[decIndex];
            }

            keyPos = 0;
        }

        private char GetKeyChar() {
            if(keyPos > key.Length - 1) {
                keyPos = 0;
            }
            return key[keyPos++];
        }                
    }
}
