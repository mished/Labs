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

        public Server(string hostname, int port, int clientPort) {
            endPoint = new IPEndPoint(IPAddress.Parse(hostname), clientPort);
            server = new UdpClient(port);
            Console.WriteLine($"Server start listenning. Local End Point: {endPoint.ToString()}");            
            ProcessConnection();
        }
       
        private void ProcessConnection() {        
            const int minCharsCount = 3;
            const int charsToProcess = 64;           

            try
            {                
                var data = "";
                while (true)
                {              
                    var bytes = server.Receive(ref endPoint);
                    data += Encoding.ASCII.GetString(bytes);
                    if (data.Length >= charsToProcess)
                    {
                        var str = data.Take(charsToProcess);
                        var charsTable = str.Distinct()
                            .ToDictionary(c => c, c => str.Count(x => x == c));
                        if (charsTable.Count < minCharsCount)
                        {
                            var message = $"Server got only {charsTable.Count} chars, closing connection.<EOF>";
                            server.Send(Encoding.ASCII.GetBytes(message + "<EOF>"), message.Length + 5, endPoint);
                            server.Close();
                            break;
                        }

                        var result = String.Join(", ",
                            charsTable.Select(c => $"{c.Key}: {c.Value}"));
                        Console.WriteLine($"Sending {result}\n");
                        server.Send(Encoding.ASCII.GetBytes(result + "<EOF>"), result.Length + 5, endPoint);
                        data = String.Join("", data.Skip(charsToProcess));
                    }
                }
            }
            finally
            {
                server.Close();
            }
        }
    }
}
