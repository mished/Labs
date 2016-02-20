using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using System.Runtime.InteropServices;
using static System.Math;

namespace SIMD {

    public class Sobel {

        private sbyte[] Mh = {1, 2, 1, 0, 0, 0, -1, -2, -1};
        private sbyte[] Mv = {1, 0, -1, 2, 0, -2, 1, 0, -1};

        private Bitmap origin;
        private BitmapData bmData;
        private byte[] originRgb;
        private byte[] resultRgb;
        private int pixSize;

        public Bitmap Image => (Bitmap)origin.Clone();
        
        private Sobel() { }

        public Sobel(Bitmap origin, SobelFunc func) {
            this.origin = origin;

            var rect = new Rectangle(0, 0, origin.Width, origin.Height);
            bmData = origin.LockBits(rect, ImageLockMode.ReadWrite, origin.PixelFormat);
            var ptr = bmData.Scan0;
            var bytes = bmData.Stride * bmData.Height;
            originRgb = new byte[bytes];
            Marshal.Copy(ptr, originRgb, 0, bytes);

            if (func == SobelFunc.NoVect)
                ApplySobelOperator();
            else
                ApplySobelOperatorVect();

            Marshal.Copy(resultRgb, 0, ptr, bytes);
            origin.UnlockBits(bmData);
        }

        private void ApplySobelOperatorVect() {
            resultRgb = new byte[originRgb.Length];
            pixSize = bmData.Stride / bmData.Width;

            for (var i = 0; i < bmData.Height; i++) {
                for (var j = 0; j < bmData.Width - 3; j += 4) {
                    var dVect = CalcValueVect(i, j);
                    resultRgb[Get1dPosition(i, j)]     = (byte)dVect.X;
                    resultRgb[Get1dPosition(i, j + 1)] = (byte)dVect.Y;
                    resultRgb[Get1dPosition(i, j + 2)] = (byte)dVect.Z;
                    resultRgb[Get1dPosition(i, j + 3)] = (byte)dVect.W;
                }
            }
        }

        private Vector4 CalcValueVect(int i, int j) {

            var A = new Vector4();
            var B = new Vector4();
            var C = new Vector4();
            var D = new Vector4();
            var F = new Vector4();
            var G = new Vector4();
            var H = new Vector4();
            var I = new Vector4();

            A.X = GetPointSafe(i - 1, j - 3);
            B.X = GetPointSafe(i - 1, j);
            C.X = GetPointSafe(i - 1, j + 3);
            D.X = GetPointSafe(i, j - 1);    
            F.X = GetPointSafe(i, j + 1);
            G.X = GetPointSafe(i + 1, j - 3);
            H.X = GetPointSafe(i + 1, j);
            I.X = GetPointSafe(i + 1, j + 3);

            j += 1;

            A.Y = GetPointSafe(i - 1, j - 3);
            B.Y = GetPointSafe(i - 1, j);
            C.Y = GetPointSafe(i - 1, j + 3);
            D.Y = GetPointSafe(i, j - 1);
            F.Y = GetPointSafe(i, j + 1);
            G.Y = GetPointSafe(i + 1, j - 3);
            H.Y = GetPointSafe(i + 1, j);
            I.Y = GetPointSafe(i + 1, j + 3);

            j += 1;

            A.Z = GetPointSafe(i - 1, j - 3);
            B.Z = GetPointSafe(i - 1, j);
            C.Z = GetPointSafe(i - 1, j + 3);
            D.Z = GetPointSafe(i, j - 1);
            F.Z = GetPointSafe(i, j + 1);
            G.Z = GetPointSafe(i + 1, j - 3);
            H.Z = GetPointSafe(i + 1, j);
            I.Z = GetPointSafe(i + 1, j + 3);

            j += 1;

            A.W = GetPointSafe(i - 1, j - 3);
            B.W = GetPointSafe(i - 1, j);
            C.W = GetPointSafe(i - 1, j + 3);
            D.W = GetPointSafe(i, j - 1);
            F.W = GetPointSafe(i, j + 1);
            G.W = GetPointSafe(i + 1, j - 3);
            H.W = GetPointSafe(i + 1, j);
            I.W = GetPointSafe(i + 1, j + 3);

            var t1 = A - I;
            var t2 = C - G;
            var Hh = 2 * (D - F) + t1 - t2;
            var Hv = 2 * (B - H) + t1 + t2;
            Hh *= Hh;
            Hv *= Hv;
            var d = Vector4.SquareRoot(Hh + Hv) * 256 / 1140;

            return d;
        }

        private void ApplySobelOperator() {
            resultRgb = new byte[originRgb.Length];
            pixSize = bmData.Stride / bmData.Width;

            for (var i = 0; i < bmData.Height; i++) {
                for (var j = 0; j < bmData.Width; j++) {
                    resultRgb[Get1dPosition(i, j)] = CalcValue(i, j);
                }
            }
        }

        private byte CalcValue(int i, int j) {
            var A = GetPointSafe(i - 1, j - 3);
            var B = GetPointSafe(i - 1, j);
            var C = GetPointSafe(i - 1, j + 3);
            var D = GetPointSafe(i, j - 1);
            var E = GetPointSafe(i, j);
            var F = GetPointSafe(i, j + 1);
            var G = GetPointSafe(i + 1, j - 3);
            var H = GetPointSafe(i + 1, j);
            var I = GetPointSafe(i + 1, j + 3);
            //byte[] tmp = {A, B, C, D, E, F, G, H, I};
            //var dv = tmp.Zip(Mv, (x, y) => x * y).Sum();
            //var dh = tmp.Zip(Mh, (x, y) => x * y).Sum();
            //var d = Floor(Sqrt(Pow(dh, 2) + Pow(dv, 2)) * 256 / 1140);
            var t1 = A - I;
            var t2 = C - G;
            var Hh = 2 * (D - F) + t1 - t2;
            var Hv = 2 * (B - H) + t1 + t2;
            var d = Floor(Sqrt(Pow(Hh, 2) + Pow(Hv, 2)) * 256 / 1140);
            return (byte)d;
        }

        private byte GetPointSafe(int i, int j) {
            if (i >= 0 && i < bmData.Height && j >= 0 && j < bmData.Width) {
                return originRgb[Get1dPosition(i, j)];
            }
            return 0;
        }

        private int Get1dPosition(int i, int j) {
            return i * bmData.Stride + j * pixSize;
        }

    }

    public enum SobelFunc {
        NoVect,
        Vect
    }
}
