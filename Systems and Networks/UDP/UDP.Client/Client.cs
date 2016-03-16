using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UDP {
    public class Client {
        UdpClient listener;
        IPEndPoint groupEP ;

        public Client(int port) {
            listener = new UdpClient(port);
            groupEP = new IPEndPoint(IPAddress.Any, port);

            Broadcast();
        }

        private void Broadcast() {
            while(true) {
                var bytes = listener.Receive(ref groupEP);     
                var mess = Encoding.ASCII.GetChars(bytes);
                Console.WriteLine(new string(mess));
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
