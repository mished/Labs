using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace TCP {
    public class Client {

        private Socket clientSocket;
        private IPEndPoint serverEndpoint;

        public Client(IPEndPoint serverEndpoint) {
            this.serverEndpoint = serverEndpoint;
            clientSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            StartSender();
        }

        private void StartSender() {
            clientSocket.Connect(serverEndpoint);
            Console.WriteLine($"Client connected to {clientSocket.RemoteEndPoint.ToString()}\n");

            using (var stream = new NetworkStream(clientSocket))
            using (var reader = new BinaryReader(stream))
            using (var writer = new BinaryWriter(stream))
            using (var fileStream = new FileStream("./log.txt", FileMode.Create))
            using (var fileWriter = new BinaryWriter(fileStream)) {
                while (true) {
                    var sentChars = 0;
                    var message = ReadMessage();
                    Console.WriteLine($"\nSending message: {message}\n");
                    writer.Write(message);
                    sentChars += message.Length;
                    var answer = "";
                    if (sentChars >= 64) {
                        answer = reader.ReadString();
                        sentChars = 0;
                    }
                    if (!String.IsNullOrEmpty(answer)) {
                        Console.WriteLine($"\nGot answer: {answer}\n");
                        fileWriter.Write(answer);
                    }
                }
            }
        }

        private string ReadMessage() {
            var message = "";
            while (true) {
                var ch = Console.ReadKey();
                if (ch.Key == ConsoleKey.PageDown)
                    break;
                message += ch.KeyChar;
            }
            return message;
        }
    }
}
