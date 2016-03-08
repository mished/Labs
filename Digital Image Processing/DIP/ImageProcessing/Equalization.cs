using System;
using System.Drawing;
using System.Linq;

namespace DIP {

    public class Equalization : IDisposable {

        private Bitmap original;
        private Bitmap current;

        private Equalization() { }

        public Equalization(Bitmap image) {
            original = image;
        }

        public Bitmap Equalize() {
            current = new Bitmap(original.Width, original.Height);
            var oldLevels = original.GetGrayscaleBytes();
            var newLevels = GetNewBrightnessLevels(oldLevels.Get1dArray());
            for (var i = 0; i < original.Width; i++) {
                for (var j = 0; j < original.Height; j++) {
                    var oldLevel = oldLevels[i, j];
                    var newLevel = newLevels[oldLevel];
                    current.SetPixel(i, j, Color.FromArgb(255, newLevel, newLevel, newLevel));
                }
            }

            return (Bitmap)current.Clone();
        }

        private byte[] GetNewBrightnessLevels(byte[] oldLevels) {
            const int colorLevels = 255;
            var count = oldLevels.Count();
            var frq = new int[colorLevels];

            oldLevels.ToList()
                     .ForEach(x => frq[x] += 1);

            var pmf = frq.AsParallel()
                         .AsOrdered()
                         .Select(x => (float)x / count);

            return pmf.Select((x, i) => (byte)Math.Round(colorLevels * pmf.Take(i + 1).Sum()))
                      .ToArray();
        }

        public void Dispose() {
            current.Dispose();
        }

    }
}
