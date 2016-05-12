using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DIP {

    public static class CV {

        private static Color[] colors = new Color[] { Color.Red, Color.Purple, Color.Green, Color.HotPink, Color.Blue, Color.Brown, Color.Black };

        public static Bitmap Process(Bitmap image) {
            var current = new Bitmap(image.Width, image.Height);
            var currentBin = new Bitmap(image.Width, image.Height);

            var gs = image.GetGrayscaleBytes();
            var binarized = Binarize(gs);

            for (var i = 0; i < currentBin.Width; i++) {
                for (var j = 0; j < currentBin.Height; j++) {
                    var tmp = binarized[i, j] == 1 ? Color.White : Color.Black;
                    currentBin.SetPixel(i, j, tmp);
                }
            }

            var scanned = Scan(binarized);
            var curColor = 0;
            var colorsDict = new Dictionary<int, Color>();

            for (var i = 0; i < current.Width; i++) {
                for (var j = 0; j < current.Height; j++) {
                    var mark = scanned[i, j];
                    if (mark == 0) current.SetPixel(i, j, Color.White);
                    if (mark >= 2) {
                        Color tmpCol;
                        if (colorsDict.ContainsKey(mark)) tmpCol = colorsDict[mark];
                        else {
                            curColor = (curColor == colors.Count() - 1) ? 0 : curColor + 1;
                            colorsDict.Add(mark, colors[curColor]);
                            tmpCol = colors[curColor];
                        }
                        current.SetPixel(i, j, tmpCol);
                    }
                }
            }

            currentBin.Save("SAVEDBIN.bmp");
            current.Save("SAVED.bmp");
            return current;
        }

        public static int[,] Scan(int[,] binImage) {
            var width = binImage.GetLength(0);
            var height = binImage.GetLength(1);
            var res = (int[,])binImage.Clone();
            int currentLabel = 1;

            for (var i = 1; i < height - 1; i++) {
                for (var j = 1; j < width - 1; j++) {
                    var a = res[j, i];
                    var b = res[j - 1, i];
                    var c = res[j, i - 1];
                    if (a == 0) res[j, i] = 0;
                    else if (b < 2 && c < 2) {
                        currentLabel += 1;
                        res[j, i] = currentLabel;
                    } else if (b > 1 && c < 2)
                        res[j, i] = b;
                    else if (c > 1 && b < 2)
                        res[j, i] = c;
                    else if (b > 1 && c > 1) {
                        if (b == c) res[j, i] = c;
                        else {
                            res[j, i] = c;
                            for (var k = 1; k < height - 1; k++) {
                                for (var z = 1; z < width - 1; z++) {
                                    if (res[z, k] == b) res[z, k] = c;
                                }
                            }
                        }
                    }
                }
            }

            return res;
        }

        public static int[,] Binarize(byte[,] gsImage) {
            var width = gsImage.GetLength(0);
            var height = gsImage.GetLength(1);
            var res = new int[width, height];
            var gsArray = gsImage.Get1dArray();
            var threshold = GetOtsuThreshold(GetHistogram(gsArray), gsArray.Length);
            for (var i = 0; i < width; i++) {
                for (var j = 0; j < height; j++) {
                    res[i, j] = (gsImage[i, j] >= threshold) ? 0 : 1;
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
