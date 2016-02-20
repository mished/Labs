using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;
using SIMD;
using System.IO;

namespace Sandbox {
    class Program {
        static void Main(string[] args) {

            var path = "..\\..\\img\\";
            var sw = new Stopwatch();
            var i = 0;

            Console.WriteLine("Sobel op, no Vect: ");
            foreach (var file in Directory.GetFiles(path, "*.bmp")) {                
                var image = new Bitmap(file);
                sw.Reset();
                sw.Start();
                var sobelled = new Sobel(image, SobelFunc.NoVect).Image;
                sw.Stop();
                Console.WriteLine($"[{file}]: {sw.ElapsedMilliseconds}ms");
                sobelled.Save($"{i++}.bmp");
            }
            i = 0;
            Console.WriteLine("\nSobel op, Vect: ");
            foreach (var file in Directory.GetFiles(path, "*.bmp")) {
                var image = new Bitmap(file);
                sw.Reset();
                sw.Start();
                var sobelled = new Sobel(image, SobelFunc.Vect).Image;
                sw.Stop();
                Console.WriteLine($"[{file}]: {sw.ElapsedMilliseconds}ms");
                sobelled.Save($"{i++}_VECT.bmp");
            }
        }
    }
}
