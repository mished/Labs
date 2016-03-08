using System.Drawing;

namespace DIP.Linq {
    public static class DipExtensions {

        public static Bitmap Scale(this Bitmap image, double coefficient) {
            return Scaling.Scale(image, coefficient);
        }

        public static Bitmap Equalize(this Bitmap image) {
            return Equalization.Equalize(image);
        }

        public static Bitmap Grayscale(this Bitmap image) {
            return Shared.Grayscale(image);
        }

        public static Bitmap Normalize(this Bitmap image) {
            return Normalization.Normalize(image);
        }
    }
}
