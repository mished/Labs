using System;

namespace BiosInfo {
    class Program {
        static void Main(string[] args) {

            foreach(var str in BiosInfo.GetBiosInfo()) {
                Console.WriteLine(str);
            }
            Console.WriteLine("\nUSB Devices:");
            foreach (var str in BiosInfo.GetUsbInfo()) {                
                Console.WriteLine(str);
            }


        }
    }
}
