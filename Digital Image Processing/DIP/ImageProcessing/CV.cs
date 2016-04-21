using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace DIP {

    public static class CV {

        public static Bitmap Process(Bitmap image) {
            var current = new Bitmap(image.Width, image.Height);
            var gs = image.GetGrayscaleBytes();
            var binarized = Binarize(gs);
            var scanned = Scan(binarized);
            
            for (var i = 0; i < current.Width; i++) {
                for (var j = 0; j < current.Height; j++) {
                    if (scanned[i, j] == 0) current.SetPixel(i, j, Color.White);
                    if (scanned[i, j] == 2) current.SetPixel(i, j, Color.Red);
                    else if (scanned[i, j] > 2) current.SetPixel(i, j, Color.FromArgb(255, scanned[i, j], scanned[i, j], scanned[i, j]));
                }
            }
            current.Save("e:\\SAVE.bmp");
            throw new NotImplementedException();
        }

        public static byte[,] Scan(byte[,] binImage) {
            var width = binImage.GetLength(0);
            var height = binImage.GetLength(1);
            var res = (byte[,])binImage.Clone();
            byte currentLabel = 1;
            var newValues = new Dictionary<int, int>();
            for (var i = 1; i < height - 1; i++) {
                for (var j = 1; j < width - 1; j++) {
                    var a = res[j, i];
                    var b = res[j - 1, i];
                    var c = res[j, i - 1];
                    if (a == 0) res[j, i] = 0;
                    else if (b < 2 && c < 2)
                        res[j, i] = ++currentLabel;
                    else if (b > 1 && c < 2)
                        res[j, i] = b;
                    else if (c > 1 && b < 2)
                        res[j, i] = c;
                    else if (b > 1 && c > 1) {
                        if (b == c) res[j, i] = b;
                        else {
                            if (newValues.ContainsKey(c)) {
                                var tmp = (byte)newValues[c];
                                res[j, i] = tmp;
                                if (newValues.ContainsKey(b)) newValues[b] = tmp;
                                else newValues.Add(b, tmp);
                            } else {
                                res[j, i] = c;
                                if (newValues.ContainsKey(b)) newValues[b] = c;
                                else newValues.Add(b, c);
                            }
                        }
                    }
                }
            }
            for (var i = 1; i < height - 1; i++) {
                for (var j = 1; j < width - 1; j++) {
                    var cur = res[j, i];
                    if (newValues.ContainsKey(cur)) res[j, i] = (byte)newValues[cur];
                }
            }
            return res;
        }

        public static byte[,] Binarize(byte[,] gsImage) {
            var width = gsImage.GetLength(0);
            var height = gsImage.GetLength(1);
            var res = new byte[width, height];
            var gsArray = gsImage.Get1dArray();
            var threshold = GetOtsuThreshold(GetHistogram(gsArray), gsArray.Length);
            for (var i = 0; i < width; i++) {
                for (var j = 0; j < height; j++) {
                    res[i, j] = (byte)(gsImage[i, j] >= threshold ? 0 : 1);
                }
            }
            return res;
        }

        private static int GetOtsuThreshold(int[] histogram, int total) {
            var sum = 0;
            for (var i = 1; i < 256; ++i)
                sum += i * histogram[i];
            int sumB = 0, wB = 0, wF = 0, mB = 0, mF = 0, max = 0, between = 0, threshold1 = 0, threshold2 = 0;
            for (var i = 0; i < 256; ++i) {
                wB += histogram[i];
                if (wB == 0)
                    continue;
                wF = total - wB;
                if (wF == 0)
                    break;
                sumB += i * histogram[i];
                mB = sumB / wB;
                mF = (sum - sumB) / wF;
                between = wB * wF * (mB - mF) * (mB - mF);
                if (between >= max) {
                    threshold1 = i;
                    if (between > max) {
                        threshold2 = i;
                    }
                    max = between;
                }
            }
            return (threshold1 + threshold2) / 2;
        }

        private static int[] GetHistogram(byte[] image) {
            var frq = new int[256];
            image.ToList()
                .ForEach(x => frq[x] += 1);
            return frq;
        }
    }
}
