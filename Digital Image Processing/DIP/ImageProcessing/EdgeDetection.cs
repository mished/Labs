using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DIP {

    public static class EdgeDetection {

        private static sbyte[][] masks = new sbyte[][] {
            new sbyte[] { 1, 1, 1, 1, -2, 1, -1, -1, -1 },
            new sbyte[] { 1, 1, 1, -1, -2, 1, -1, -1, 1 },
            new sbyte[] { -1, 1, 1, -1, -2, 1, -1, 1, 1 },
            new sbyte[] { -1, -1, 1, -1, -2, 1, 1, 1, 1 },
            new sbyte[] { -1, -1, -1, 1, -2, 1, 1, 1, 1 },
            new sbyte[] { 1, -1, -1, 1, -2, -1, 1, 1, 1 },
            new sbyte[] { 1, 1, -1, 1, -2, -1, 1, 1, -1 },
            new sbyte[] { 1, 1, 1, -1, -2, -1, 1, -1, -1 }
        };

        private const int limit = 100;

        public static Bitmap DetectEdges(Bitmap original, ExchangeMask maskType) {
            var mask = masks[(int)maskType];
            var current = new Bitmap(original.Width, original.Height);
            var gsBytes = original.GetGrayscaleBytes();
            for (var i = 0; i < current.Width; i++)
                for (var j = 0; j < current.Height; j++) {
                    var newVal = GetSiblings(gsBytes, i, j)
                        .Zip(mask, (a, b) => a * b)
                        .Sum();
                    newVal = (newVal > limit) ? 255 : 0;
                    current.SetPixel(i, j, Color.FromArgb(255, newVal, newVal, newVal));
                }
            return current;
        }

        private static IEnumerable<byte> GetSiblings(byte[,] image, int i, int j) {
            return Shared.GetSiblingCoordinates(i, j)
                .Select(x => GetPointSafe(image, x[0], x[1]));
        }

        private static byte GetPointSafe(byte[,] image, int i, int j) {
            var width = image.GetLength(0);
            var height = image.GetLength(1);
            if (i >= 0 && i < width && j >= 0 && j < height)
                return image[i, j];
            return 0;
        }

    }

    public enum ExchangeMask {
        North = 0,
        Northeast,
        East,
        Southeast,
        South,
        Southwest,
        West,
        Northwest
    }
}
