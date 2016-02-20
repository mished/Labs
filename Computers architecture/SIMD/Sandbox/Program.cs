using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using SIMD;

namespace Sandbox {
    class Program {
        static void Main(string[] args) {

            var path = "..\\..\\img\\";
            var sw = new Stopwatch();
            var sw2 = new Stopwatch();
            var i = 0;

            Console.WriteLine("Sobel op, no Vect: ");
            sw2.Start();
            foreach (var file in Directory.GetFiles(path, "*.bmp")) {                
                var image = new Bitmap(file);
                sw.Reset();
                sw.Start();
                var sobelled = new Sobel(image, SobelFunc.NoVect).Image;
                sw.Stop();
                Console.WriteLine($"[{file}]: {sw.ElapsedMilliseconds}ms");
                sobelled.Save($"{i++}.bmp");
            }
            sw2.Stop();
            Console.WriteLine($"Elapsed time: {sw2.ElapsedMilliseconds}");


            Console.WriteLine("\nSobel op, Vect: ");
            i = 0;
            sw2.Reset();
            sw2.Start();            
            foreach (var file in Directory.GetFiles(path, "*.bmp")) {
                var image = new Bitmap(file);
                sw.Reset();
                sw.Start();
                var sobelled = new Sobel(image, SobelFunc.Vect).Image;
                sw.Stop();
                Console.WriteLine($"[{file}]: {sw.ElapsedMilliseconds}ms");
                sobelled.Save($"{i++}_VECT.bmp");
            }
            sw2.Stop();
            Console.WriteLine($"Elapsed time: {sw2.ElapsedMilliseconds}");


            Console.WriteLine("\nSobel op, Vect + Parallel: ");
            i = 0;
            sw2.Reset();
            sw2.Start();
            Parallel.ForEach(Directory.GetFiles(path, "*.bmp"), file => {
                var image = new Bitmap(file);
                var sw3 = new Stopwatch();
                sw3.Start();
                var sobelled = new Sobel(image, SobelFunc.Vect).Image;
                sw3.Stop();
                Console.WriteLine($"[{file}]: {sw3.ElapsedMilliseconds}ms");
                sobelled.Save($"{i++}_VECTPARALLEL.bmp");
            });
            sw2.Stop();
            Console.WriteLine($"Elapsed time: {sw2.ElapsedMilliseconds}");
        }
    }
}
