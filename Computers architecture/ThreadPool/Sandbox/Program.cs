using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Mished.ThreadPool;
using SIMD;

namespace Sandbox {

    class Program {

        static void Main(string[] args) {

            var path = "..\\..\\img\\";
            var i = 0;
            var pool = new GoPool(10);
            var qPool = new GoPool(1);

            Directory.GetFiles(path, "*.bmp").ToList().ForEach(file => {
                var image = new Bitmap(file);

                pool.Go(() => {
                    var sw = new Stopwatch();

                    sw.Start();
                    var sobelled = new Sobel(image, SobelFunc.Vect).Image;
                    sw.Stop();

                    qPool.Go(() => sobelled.Save($"{i++}_POOL.bmp"));
                    Console.WriteLine($"[{file}]: {sw.ElapsedMilliseconds}ms");
                });
            });
            pool.Dispose();
            qPool.Dispose();
        }
    }
}
