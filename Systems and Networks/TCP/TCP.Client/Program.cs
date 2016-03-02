using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TCP {
    class Program {
        static void Main(string[] args) {
            var client = new Client(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000));
        }
    }
}
