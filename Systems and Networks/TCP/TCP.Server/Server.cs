using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TCP {
    public class TcpServer {

        private Socket serverSocket;
        private IPHostEntry serverMachineInfo;
        private IPEndPoint serverEndpoint;

        public TcpServer(int port) {
            serverMachineInfo = Dns.GetHostEntry(Dns.GetHostName());
            serverEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            serverSocket = new Socket(serverEndpoint.Address.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(serverEndpoint);
            serverSocket.Listen((int)SocketOptionName.MaxConnections);
            Console.WriteLine($"TCP Server is listening on {serverEndpoint.Address}:{serverEndpoint.Port}");
            AcceptConnections();
        }

        public void AcceptConnections() {
            while (true) {
                try {
                    var socket = serverSocket.Accept();
                    Console.WriteLine("Client connected...\n");
                    ProcessConnection(socket);
                } catch (Exception ex) {
                    Console.WriteLine($"Exception: {ex.Message}");
                }
            }
        }

        private void ProcessConnection(Socket socket) {
            const int minCharsCount = 3;
            const int charsToProcess = 64;
            const int bufSize = 1024;

            try {
                if (!socket.Connected) return;
                var data = "";
                while (true) {
                    var bytes = new byte[bufSize];
                    var recData = socket.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, recData);
                    if (data.Length >= charsToProcess) {
                        var str = data.Take(charsToProcess);
                        var charsTable = str.Distinct()
                            .ToDictionary(c => c, c => str.Count(x => x == c));
                        if (charsTable.Count < minCharsCount) {
                            var message = $"Server got only {charsTable.Count} chars, closing connection.<EOF>";
                            socket.Send(Encoding.ASCII.GetBytes(message));
                            socket.Close();
                            break;
                        }

                        var result = String.Join(", ",
                            charsTable.Select(c => $"{c.Key}: {c.Value}"));
                        Console.WriteLine($"Sending {result}\n");
                        socket.Send(Encoding.ASCII.GetBytes(result + "<EOF>"));
                        data = String.Join("", data.Skip(charsToProcess));
                    }
                }
            } finally {
                socket.Close();
            }
        }
    }
}
