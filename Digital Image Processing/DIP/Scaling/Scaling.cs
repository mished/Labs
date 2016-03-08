using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static System.Math;

namespace DIP {
    public static class Scaling {

        public static Bitmap Scale(Bitmap original, double coefficient) {
            var current = new Bitmap((int)(original.Width * coefficient), (int)(original.Height * coefficient));
            Enumerable.Range(0, current.Width).ToList()
                .ForEach(x => Enumerable.Range(0, current.Height).ToList()
                                .ForEach(y => current.SetPixel(x, y, CalcPixel(original, coefficient, x, y))));
            return current;
        }

        private static Color CalcPixel(Bitmap original, double coefficient, int x, int y) {
            var origX = x / coefficient;
            var origY = y / coefficient;
            var siblings = GetSiblings(original, origX, origY);
            if (siblings.Any(s => s.d == 0))
                return siblings.Where(s => s.d == 0)
                               .Select(s => original.GetPixel(s.x, s.y))
                               .First();
            var w = siblings.Sum(s => 1 / s.d);
            return Color.FromArgb(
                siblings.Aggregate(0, (p, c) => p + (int)(original.GetPixel(c.x, c.y).R / (c.d * w))),
                siblings.Aggregate(0, (p, c) => p + (int)(original.GetPixel(c.x, c.y).G / (c.d * w))),
                siblings.Aggregate(0, (p, c) => p + (int)(original.GetPixel(c.x, c.y).B / (c.d * w)))
                );
        }

        private static IEnumerable<Point> GetSiblings(Bitmap original, double x, double y) {
            var tarX = (int)Round(x);
            var tarY = (int)Round(y);
            if (PointIsInRange(original, tarX, tarY))
                yield return GetPoint(tarX, tarY, x, y);
            if (PointIsInRange(original, tarX - 1, tarY))
                yield return GetPoint(tarX - 1, tarY, x, y);
            if (PointIsInRange(original, tarX + 1, tarY))
                yield return GetPoint(tarX + 1, tarY, x, y);
            if (PointIsInRange(original, tarX, tarY - 1))
                yield return GetPoint(tarX, tarY - 1, x, y);
            if (PointIsInRange(original, tarX, tarY + 1))
                yield return GetPoint(tarX, tarY + 1, x, y);
        }

        private static Point GetPoint(int tarX, int tarY, double x, double y) {
            var delta = Abs(tarX - x) + Abs(tarY - y);
            return new Point(tarX, tarY, delta);
        }

        private static bool PointIsInRange(Bitmap original, int x, int y) {
            return x >= 0 && x < original.Width
                && y >= 0 && y < original.Height;
        }

        private struct Point {
            public int x;
            public int y;
            public double d;

            public Point(int x, int y, double d) {
                this.x = x;
                this.y = y;
                this.d = d;
            }
        }
    }
}
