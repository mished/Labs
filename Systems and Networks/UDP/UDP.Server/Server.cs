using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UDP {
    public class Server {
        private Socket socket;
        private IPAddress broadcast; 
        private IPEndPoint endPoint;

        public Server(int port) {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
                ProtocolType.Udp);
            broadcast = IPAddress.Parse("127.0.0.1");
            endPoint = new IPEndPoint(broadcast, port);
            socket.Bind(endPoint);            
            Console.WriteLine($"Server waiting for broadcast at {endPoint.Address}:{endPoint.Port}");
            WaitForBroadcast(socket);
        }

        private void WaitForBroadcast(Socket socket) {
            var buffer = new byte[1024];
            
            while(true) {
                try {
                    var mess = ReadMessage();
                    buffer = Encoding.ASCII.GetBytes(mess);
                    socket.SendTo(buffer,endPoint);
                }
                finally {                
                    socket.Close();
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
