using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static System.Math;

namespace DIP {
    public class Scaling {
        private Bitmap original;
        private Bitmap current;
        private double coefficient;

        private Scaling() { }

        public Scaling(Bitmap image, double coefficient) {
            this.original = image;
            this.coefficient = coefficient;
        }

        public Bitmap ScaleImage() {
            current = new Bitmap((int)(original.Width * coefficient), (int)(original.Height * coefficient));
            Enumerable.Range(0, current.Width - 1)
                .ToList()
                .ForEach(x => Enumerable.Range(0, current.Height - 1)
                                .ToList()
                                .ForEach(y => current.SetPixel(x, y, CalcPixel(x, y))));
            return (Bitmap)current.Clone();
        }

        private Color CalcPixel(int x, int y) {
            var origX = x / coefficient;
            var origY = y / coefficient;
            var siblings = GetSiblings(origX, origY);
            if (siblings.Any(s => s.d == 0))
                return siblings.Where(s => s.d == 0).Select(s => original.GetPixel(s.x, s.y)).First();
            var w = siblings.Sum(s => 1 / s.d);
            return Color.FromArgb(
                siblings.Aggregate(0, (p, c) => p + (int)(original.GetPixel(c.x, c.y).R / (c.d * w))),
                siblings.Aggregate(0, (p, c) => p + (int)(original.GetPixel(c.x, c.y).G / (c.d * w))),
                siblings.Aggregate(0, (p, c) => p + (int)(original.GetPixel(c.x, c.y).B / (c.d * w)))
                );
        }

        private IEnumerable<Point> GetSiblings(double x, double y) {
            var tarX = (int)Round(x);
            var tarY = (int)Round(y);
            yield return GetPoint(tarX, tarY, x, y);
            if ((tarX - 1).IsInWidth(original.Width)) {
                yield return GetPoint(tarX - 1, tarY, x, y);
            }
            if ((tarX + 1).IsInWidth(original.Width)) {
                yield return GetPoint(tarX + 1, tarY, x, y);
            }
            if ((tarY - 1).IsInHeight(original.Height)) {
                yield return GetPoint(tarX, tarY - 1, x, y);
            }
            if ((tarY + 1).IsInHeight(original.Height)) {
                yield return GetPoint(tarX, tarY + 1, x, y);
            }
        }

        private Point GetPoint(int tarX, int tarY, double x, double y) {
            var delta = Abs(tarX - x) + Abs(tarY - y);
            return new Point(x: tarX, y: tarY, d: delta);
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

    public static class ScalingUtils {
        public static bool IsInWidth(this int x, int width) {
            return x >= 0 && x < width;
        }

        public static bool IsInHeight(this int y, int height) {
            return y >= 0 && y < height;
        }
    }
}
