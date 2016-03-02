using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCP;

namespace TCP {
    class Program {
        static void Main(string[] args) {
            const int port = 5000;
            var server = new TcpServer(port);
        }
    }
}
