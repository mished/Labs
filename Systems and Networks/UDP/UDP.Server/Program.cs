using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDP {
    class Program {
        static void Main(string[] args) {
            const int port = 11000;
            const int clientPort = 9050;
            const string hostname = "127.0.0.1";
            var server = new Server(hostname, port, clientPort);
        }
    }
}
