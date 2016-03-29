using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UDP {
    public class Client {
        UdpClient client;
        IPEndPoint endPoint ;

        public Client(string hostname,int port,int serverPort) {            
            client = new UdpClient(port);         
            endPoint = new IPEndPoint(IPAddress.Parse(hostname), serverPort);
            StartSending(port);          
        }

        private void StartSending(int port)
        {

            try
            {
                using (var fileStream = new FileStream("log.txt", FileMode.Create))
                using (var fileWriter = new StreamWriter(fileStream))
                {                  
                    Console.WriteLine($"[{DateTime.Now}] Client connected. Local End Point: {endPoint.ToString()}\n");                  
                    fileWriter.WriteLine($"[{DateTime.Now}] Client connected. Local End Point: {endPoint.ToString()}");

                    var task = Task.Run(() => {

                        while (true)
                        {
                            var answer = ReceiveMessage();
                            Console.WriteLine(answer);
                            fileWriter.Write(answer);
                            fileWriter.Flush();
                        }

                    });
                    while (true)
                    {
                        var message = ReadMessage();
                        Console.WriteLine($"\n[{DateTime.Now}] Sending message: {message}\n");
                        fileWriter.WriteLine($"[{DateTime.Now}] Sending message: {message}");                        
                        client.Send(Encoding.ASCII.GetBytes(message), message.Length, endPoint);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                Console.ReadKey();
            }
            finally
            {
                client.Close();
            }
        }

        private string ReceiveMessage()
        {
            const int bufSize = 1024;
            var data = "";

            while (data.IndexOf("<EOF>") == -1)
            {
                var bytes = new byte[bufSize];
                bytes = client.Receive(ref endPoint);
                data += Encoding.ASCII.GetString(bytes);
            }
            return $"[{DateTime.Now}] Client received: {data.Replace("<EOF>", "")}\n";
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
