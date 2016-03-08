using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace DIP.Linq {
    public static class DipExtensions {

        public static Bitmap Scale(this Bitmap image, double coefficient) {
            return new Scaling(image, coefficient).Image;
        }

        public static Bitmap Equalize(this Bitmap image) {
            return new Equalization(image).Image;
        }

        public static Bitmap Grayscale(this Bitmap image) {
            return Shared.Grayscale(image);
        }

        public static Bitmap Normalize(this Bitmap image) {
            return new Normalization(image).Image;
        }
    }
}
