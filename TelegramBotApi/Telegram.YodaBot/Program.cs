using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telegram.YodaBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var bot = new YodaBot();
            bot.Start();
            if (Console.ReadKey() != null)
                bot.Stop();
        }
    }
}
