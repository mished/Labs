using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UDP {
    public class Client {
        UdpClient client;
        IPEndPoint endPoint ;

        public Client(string hostname,int port) {
            var broadcast = IPAddress.Parse(hostname);
            endPoint = new IPEndPoint(broadcast, port);            
            ConnectToServer();    

           // Broadcast();
        }

        private void ConnectToServer()
        {   try {
                client = new UdpClient();
                client.Connect(endPoint);
                Console.WriteLine($"[{DateTime.Now}] Client connected to {endPoint.ToString()}\n");
                Console.ReadKey();
            }
            
            finally {
            

            }
            
        }        

        private string ReadMessage()
        {
            var message = "";
            while (true)
            {
                var ch = Console.ReadKey();
                if (ch.Key == ConsoleKey.PageDown)
                    break;
                message += ch.KeyChar;
            }
            return message;
        }
    }
}
