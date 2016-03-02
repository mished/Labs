using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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

            try {
                using (var fileStream = new FileStream("./log.txt", FileMode.Create))
                using (var fileWriter = new BinaryWriter(fileStream, Encoding.ASCII)) {
                    clientSocket.Connect(serverEndpoint);
                    Console.WriteLine($"[{DateTime.Now}] Client connected to {clientSocket.RemoteEndPoint.ToString()}\r\n");
                    fileWriter.Write($"[{DateTime.Now}] Client connected to {clientSocket.RemoteEndPoint.ToString()}\r\n");

                    var task = Task.Run(() => {

                        while (true) {
                            var answer = ReceiveMessage();
                            Console.WriteLine(answer);
                            fileWriter.Write(answer);
                            fileWriter.Flush();
                        }

                    });
                    while (true) {
                        var message = ReadMessage();
                        Console.WriteLine($"\n[{DateTime.Now}] Sending message: {message}\r\n");
                        fileWriter.Write($"\n[{DateTime.Now}] Sending message: {message}\r\n");
                        clientSocket.Send(Encoding.ASCII.GetBytes(message));
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine($"Exception: {ex.Message}");
                Console.ReadKey();
            } finally {
                clientSocket.Close();
            }
        }

        private string ReceiveMessage() {
            const int bufSize = 1024;
            var data = "";

            while (data.IndexOf("<EOF>") == -1) {
                var bytes = new byte[bufSize];
                var recData = clientSocket.Receive(bytes);
                data += Encoding.ASCII.GetString(bytes, 0, recData);
            }
            return $"[{DateTime.Now}] Client received: {data.Replace("<EOF>", "")}\r\n";
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
