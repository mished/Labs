using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UDP {
    public class Server {
        private UdpClient server;       
        private IPEndPoint endPoint;

        public Server(string hostname,int port) {         
            var broadcast = IPAddress.Parse(hostname);
            endPoint = new IPEndPoint(broadcast, port);
            server = new UdpClient(endPoint);          
            Console.WriteLine($"Server waiting for broadcast at {endPoint.Address}:{endPoint.Port}");
            WaitForBroadcast();
        }

        private void WaitForBroadcast() {
            var buffer = new byte[1024];
            
            while(true) {
                try {
                   
                }
                finally {                
                   
                }
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
