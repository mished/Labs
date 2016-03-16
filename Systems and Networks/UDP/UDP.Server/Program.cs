using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDP {
    class Program {
        static void Main(string[] args) {
            const int port = 11000;
            var server = new Server(port);
        }
    }
}
