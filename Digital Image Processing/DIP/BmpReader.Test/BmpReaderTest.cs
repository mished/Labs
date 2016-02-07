using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using DIP;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BmpReader.Test {

    [TestClass]
    public class BmpReaderTest {

        private const string imgPath = "..\\..\\img\\";

        [TestMethod]
        [ExpectedException(typeof(FormatException), "Should throw if image is not of 'BM' type.")]
        public void TestBmpType() {
            using (var s = new FileStream($"{imgPath}PNG.png", FileMode.Open)) {
                var img = new Bmp4Image(s);
            }
        }

        [TestMethod]
        public void TestBmpReader() {
            foreach (var file in Directory.GetFiles(imgPath, "*.bmp")) {
                var b4img = new Bmp4Image(file);
                var img = new Bitmap(file);
                CollectionAssert.AreEqual(img.GetPixels().ToList(), b4img.PixelData.GetPixels().ToList(), "Image pixels should be equal.");
            }
        }
    }

    public static class BmpTestUtils {

        public static IEnumerable<Color> GetPixels(this Bitmap bitmap) {
            for (int i = 0; i < bitmap.Height; i++) {
                for (int j = 0; j < bitmap.Width; j++) {
                    yield return bitmap.GetPixel(j, i);
                }
            }
        }
    }
}
