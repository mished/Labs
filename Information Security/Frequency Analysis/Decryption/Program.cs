using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decryption {
       

    class Program {
        private static readonly string encText = @"48 84 13 33    94 13 48 42 33 46 82,    84 13 82 48    94 82 46 84 33 42    13 88 82 84 16 46 16    25 82    50 17 48    13    42 61 37 78 50 51    16    82 42 13 82 84 16 46 16    50 48 17 34    13    76 82 25 82 16    72 82 46 48 69 17 82 28 82,    28 84 48    51 75 48    75 84 33 46 16    46 33 84 33 17,    75 33 37 82 13 17 34    16    38 48 37 17 16 46 33.    82 17    13 58 94 25 33 69 58 13 33 46    76 82 75 48 46 33 17 16 34,    16    34    76 82 25 33 69 58 13 33 46    48 50 51    13 94 48,    38 42 82    17 16    48 94 42 78    13    50 16 37 48.    13    76 37 16 28 82 37 64 17 48    17 48 17 33 13 16 94 42 17 82 28 82    72 58 46 82    94 82 72 37 33 17 82    13 94 48,    38 42 82    84 82 13 48 46 82 94 78     76 82 13 16 84 33 42 78    51 75 48    51 94 82 76 64 16 50    16    38 42 82    69 37 34 42    17 58 17 48    69 84 37 33 13 94 42 13 51 61 21 16 48:    28 82 37 82 84 33,    75 33 37 25 16 48    16    88 82 46 82 84 17 58 48    94 42 37 33 17 58,    94 82 25 37 82 13 16 21 33,    94 25 37 58 42 58 48    13    69 48 50 17 58 88    28 46 51 72 16 17 33 88,    72 82 37 82 69 84 34 21 16 48    50 82 37 34    25 82 37 33 72 46 16,    82 37 51 84 16 34    13 82 91 17 58,    16 17 94 42 37 51 50 48 17 42 58    13 37 33 38 48 13 33 17 16 34    16    50 51 69 58 25 16,    76 46 48 17 16 42 48 46 78 17 58 88    75 48 17 21 16 17,    17 48 76 82 84 13 16 75 17 58 48    69 13 48 69 84 58    16    76 46 33 17 48 42 58,    25 37 33 94 25 16,    25 82 42 82 37 58 50 16    76 82 46 78 69 51 61 42 94 34    17 48 13 48 37 17 58 48,    25 82 28 84 33    76 16 64 51 42    94 13 82 16    50 48 37 69 25 16 48 25 33 37 42 16 17 58,    37 33 94 42 48 17 16 34    16    50 16 17 48 37 33 46 58    94 82    13 94 48 50 16    16 88    94 82 25 37 82 13 48 17 17 58 50 16    69 33 50 48 38 33 42 48 46 78 17 58 50 16    94 13 82 91 94 42 13 33 50 16,    94 48 37 48 72 37 34 17 58 88    33 17 28 48 46 82 13,    38 48 91    88 46 48 72    -    88 13 33 46 33    16    76 37 48 13 82 69 17 48 94 48 17 16 48    28 82 94 76 82 84 33,    37 33 69 84 33 38 51    17 33 28 37 33 84    13    64 25 82 46 33 88,    19 16 28 51 37 58    76 42 16 98    16    98 33 37 48 91,    88 37 33 17 34 21 16 48 94 34    13    94 33 50 82 50    94 48 37 84 98 48    76 16 37 33 50 16 84,    42 48 17 78    72 58 25 33,    17 33    25 82 42 82 37 82 50    76 82 25 82 16 42 94 34    69 48 50 46 34,    16    37 58 72 58,    17 33    25 82 42 82 37 82 91    94 42 82 16 42 72 58 25,    76 51 94 42 58 17 16    13 94 48 50 16 46 82 94 42 16 13 82 28 82    72 82 28 33.    82 17    51 13 16 84 48 46    13 48 21 16    17 48 82 76 16 94 51 48 50 58 48,    42 33 25 16 48,    25 33 25    51 46 16 98 58,    82 94 13 48 21 48 17 17 58 48    28 33 69 82 13 58 50 16    37 82 75 25 33 50 16,    16    25 16 42 33,    25 82 42 82 37 58 91    51 50 16 37 33 48 42    76 37 16    69 13 51 25 33 88    38 48 46 82 13 48 38 48 94 25 82 28 82    28 82 46 82 94 33.\";
        
        public static string GetClosestChar(Dictionary<string, double> alphbFreq, double value) {
            string closestChar = null;
            double minDiff = Double.MaxValue;
            foreach(var ch in alphbFreq) {
                var diff = Math.Abs(ch.Value - value);
                if(diff <= minDiff) {
                    minDiff = diff;
                    closestChar = ch.Key;
                }
            }
            return closestChar;
        }

        static void Main(string[] args) {
            var freq = new Dictionary<string, int>();
            var alphabetFreq = new Dictionary<string, double>() { { "О", 0.09 },  { "Е", 0.072 }, { "Ё", 0.072 }, { "А", 0.062 },
                                                                  { "И", 0.062 }, { "Т", 0.053 }, { "Н", 0.053 }, { "С", 0.045 },
                                                                  { "Р", 0.040 }, { "В", 0.038 }, { "Л", 0.035 }, { "К", 0.028 },
                                                                  { "М", 0.026 }, { "Д", 0.025 }, { "П", 0.023 }, { "У", 0.021 },
                                                                  { "Я", 0.018 }, { "Ы", 0.016 }, { "З", 0.016 }, { "Ь", 0.014 },
                                                                  { "Ъ", 0.014 }, { "Б", 0.014 }, { "Г", 0.013 }, { "Ч", 0.012 },
                                                                  { "Й", 0.010 }, { "Х", 0.009 }, { "Ж", 0.007 }, { "Ю", 0.006 },
                                                                  { "Ш", 0.006 }, { "Ц", 0.004 }, { "Щ", 0.003 }, { "Э", 0.003 },
                                                                  { "Ф", 0.002 }
                                                                };
            var text = encText.ToCharArray();
            
            //parse encrypted text
            var i = 0;
            var chCount = 0;
            while(text[i] != '\\') {
                string ch;
                if (Char.IsNumber(text[i])) {
                    ch = new string(text, i, 2);
                    int val;
                    if (freq.TryGetValue(ch, out val)) {
                        freq[ch]++;
                    } else freq.Add(ch, 1);
                    i++;
                    chCount++;
                }
                i++;
            }

            //get frequences
            var freqValues = new Dictionary<string, double>();
            foreach (var x in freq.OrderByDescending(x => x.Value)) {
                freqValues.Add(x.Key, (double)x.Value / chCount);
            }

            //get decoded chars
            var decAlphabet = new Dictionary<string, string>();
            foreach(var ch in freqValues) {
                var encodedCh = ch.Key;
                var decodedCh = GetClosestChar(alphabetFreq, ch.Value);

                decAlphabet.Add(encodedCh, decodedCh);
            }

            //decode text
            string decodedText = encText;
            foreach(var ch in decAlphabet) {
                decodedText = decodedText.Replace(ch.Key, ch.Value);
            }

            Console.WriteLine(decodedText.Replace(" ", ""));
        }
    }
}
