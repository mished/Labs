using System;
using System.Drawing;
using System.Linq;

namespace DIP {

    public class Normalization : IDisposable {

        private Bitmap original;
        private Bitmap current;

        private Normalization() { }

        public Normalization(Bitmap image) {
            original = image;
        }

        public Bitmap Normalize() {
            current = new Bitmap(original.Width, original.Height);
            var gsBytes = original.GetGrayscaleBytes();
            var gsBytesArr = gsBytes.Get1dArray();
            var oldMin = gsBytesArr.Min();
            var oldMax = gsBytesArr.Max();
            var newMin = 0;
            var newMax = 255;

            for (var i = 0; i < current.Width; i++) {
                for (var j = 0; j < current.Height; j++) {
                    var oldVal = gsBytes[i, j];
                    var newVal = (oldVal - oldMin) * (newMax - newMin) / (oldMax - oldMin) + newMin;
                    current.SetPixel(i, j, Color.FromArgb(255, newVal, newVal, newVal));
                }
            }

            return (Bitmap)current.Clone();
        }

        public void Dispose() {
            current.Dispose();
        }
    }
}
