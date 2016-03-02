using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TCP {
    class Program {
        static void Main(string[] args) {
            var client = new Client(new IPEndPoint(IPAddress.Parse("10.26.11.96"), 11000));
        }
    }
}
