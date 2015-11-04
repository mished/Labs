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
        private const double matchIndex = 0.0662;
        private string alphabet;
        private double[] maxShifts;

        public Decryptor(int keyLength, IEnumerable<char> source)
        {
            this.keyLength = keyLength;
            this.source = source.ToString();
            colLnth = (int)Math.Ceiling((decimal)this.source.Length / keyLength);
            alphabet = Resources.EnglishAlphabet.ToLower();
            maxShifts = new double[keyLength];
        }
        private IEnumerable<int> GenerateIndexes(int column, int colLnth)
        {
            int j = 0;
            for (int i = 0; i < colLnth; i++)
            {
                yield return j + column;
                j += 5;
            }
        }
        private char[][] BuildColumns()
        {
            var result = new char[keyLength][];
            for (int i = 0; i < keyLength; i++)
            {
                var column = source.Where((x, index) => GenerateIndexes(i, colLnth).Contains(index));
                result[i] = column.ToArray();
            }

            return result;
        }
        private int Calc(int a)
        {
            return a * (a - 1);
        }
        private double[] GetMatchIndexes(char[][] columns)
        {
            var indexes = new double[keyLength];
            var contrGroup = columns[0].GroupBy(x => x)
                                      .ToDictionary(y => y.Key, x => x.Count());
            for (int i = 1; i < keyLength; i++)
            {
                double denominator = columns[i].Length * columns[0].Length;
                var group = columns[i].GroupBy(x => x)
                                      .ToDictionary(y => y.Key, x => x.Count());

                double numerator = group.Aggregate(0, (x, y) => x + GetValue(y, contrGroup));

                indexes[i] = numerator / denominator;
            }
            return indexes;
        }
        private int GetValue(KeyValuePair<char, int> pair, Dictionary<char, int> dictionary)
        {
            int a = 0;
            var success = dictionary.TryGetValue(pair.Key, out a);
            if (success)
                return a * pair.Value;

            return 0;
        }
        private void Shift(char[][] source)
        {
            for (int i = 1; i < keyLength; i++)
                for (int j = 0; j < source[i].Length; j++)
                {
                    var index = alphabet.IndexOf(source[i][j])+1;
                    index = index > 25 ? index - 25 : index;
                    source[i][j] = alphabet[index];
                }

        }
        private void Shift(char[] source)
        {
            for (int i = 0; i < source.Length; i++)
            {
                var index = alphabet.IndexOf(source[i]) + 1;
                index = index > 25 ? index - 25 : index;
                source[i] = alphabet[index];
            }
        }
        private int[] GetShifts(char[][] source)
        {
            var shifts = new int[keyLength];
            var maxIndexes = new double[keyLength];
            for (int i = 0; i < 26; i++)
            {
                var indexes = GetMatchIndexes(source);
                for (int j = 1; j < keyLength; j++)
                {
                    maxIndexes[j] = maxIndexes[j] > indexes[j] ? maxIndexes[j] : indexes[j];
                    shifts[j] = maxIndexes[j] > indexes[j] ? shifts[j] : i;
                }
                Shift(source);
            }
            return shifts;
        }
        private int ShiftDifferent(char fst, char another)
        {
            var fstidx = alphabet.IndexOf(fst);
            var anotheredx = alphabet.IndexOf(another);

            return Math.Abs(fst - another);
        }
        private char Shift(char ch,int count)
        {
            var index = alphabet.IndexOf(ch) + count;
            index = index > 25 ? index - 25 : index;
            return alphabet[index];
        }
        private void ShiftByFirst(int[] shifts,char[][] source)
        {
            for(int i=1;i<shifts.Length;i++)
                shifts[i] = ShiftDifferent(source[0][0], Shift(source[i][0], shifts[i]));            
        }
        public void DO()
        {
            var columns = BuildColumns();
            var shifts = GetShifts(columns);
            ShiftByFirst(shifts, columns);
            foreach (var shift in shifts)
            {
                Console.WriteLine($"Shift: {shift}");
                Console.WriteLine("--------------------------------");
            }           
        }        
    }
}
