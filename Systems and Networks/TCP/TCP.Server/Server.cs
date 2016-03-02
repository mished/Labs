using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace TCP {
    public class TcpServer {

        private Socket serverSocket;
        private IPHostEntry serverMachineInfo;
        private IPEndPoint serverEndpoint;

        public TcpServer(int port) {
            serverMachineInfo = Dns.GetHostEntry(Dns.GetHostName());
            serverEndpoint = new IPEndPoint(serverMachineInfo.AddressList[1], port);
            serverSocket = new Socket(serverEndpoint.Address.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(serverEndpoint);
            serverSocket.Listen((int)SocketOptionName.MaxConnections);
            Console.WriteLine($"TCP Server is listening on {serverEndpoint.Address}:{serverEndpoint.Port}");
            AcceptConnections();
        }

        public void AcceptConnections() {
            while (true) {
                var socket = serverSocket.Accept();
                Console.WriteLine("Client connected...\n");
                ProcessConnection(socket);
            }
        }

        private void ProcessConnection(Socket socket) {
            const int minCharsCount = 3;

            if (!socket.Connected) return;
            using (var stream = new NetworkStream(socket))
            using (var reader = new BinaryReader(stream))
            using (var writer = new BinaryWriter(stream)) {
                while (true) {
                    var str = reader.ReadChars(64);
                    var charsTable = str.Distinct()
                        .ToDictionary(c => c,
                            c => str.Count(x => x == c));
                    if (charsTable.Count < minCharsCount) {
                        writer.Write($"Server got only {charsTable.Count} chars, closing connection.");
                        socket.Close();
                        break;
                    }

                    var result = String.Join(", ",
                        charsTable.Select(c => $"{c.Key}: {c.Value}"));
                    Console.WriteLine($"Sending {result}\n");
                    writer.Write(result);
                }
            }
        }
    }
}
