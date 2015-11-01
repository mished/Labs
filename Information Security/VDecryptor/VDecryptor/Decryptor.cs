using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDecryptor.Properties;

namespace VDecryptor
{
    public class Decryptor
    {
        private int keyLength;
        private string source;
        private int colLnth;
        private const double matchIndex= 0.0662;
        private string alphabet;

        public Decryptor(int keyLength,string source)
        {
            this.keyLength = keyLength;
            this.source = source.Trim('\n','\r');
            colLnth = (int)Math.Ceiling((decimal)source.Length);
            alphabet = Resources.EnglishAlphabet;
        }        
        private IEnumerable<int> GenerateIndexes(int column,int colLnth)
        {
            for(int i=0;i<colLnth;i++)
            {
                yield return column ;
                column += 5;
            }
        }
        private char[][] BuildColumns()
        {            
            var result = new char[keyLength][];
            for (int i = 0; i < keyLength; i++)
            {
                var column = source.Where((x,index)=>GenerateIndexes(i,colLnth).Contains(index));
                result[i]=column.ToArray();
            }

            return result;
        } 
        private int Calc(int a)
        {
            return a * (a - 1);
        }
        private double[] GetMatchIndexes(char[][] columns)
        {
            var indexes =new double[columns.Rank];
            for(int i=0;i<columns.Rank;i++)
            {
                var denominator = Calc(columns[i].Length);
                var numerator=columns[i].Aggregate(0,(x,y)=>x+Calc(alphabet.IndexOf(y)+1));

                indexes[i] = numerator / denominator;
            }
            return indexes;
        }
        private IEnumerable<char> Shift(char[] source,int count)
        {
            foreach(var ch in source)
            {
                yield return alphabet[alphabet.IndexOf(ch) + count];
            }
        }
    }
}
