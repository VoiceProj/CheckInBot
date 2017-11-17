using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace CheckInBot
{
    class Program
    {
        static void Main(string[] args)
        {
            string cmd = (args.Count() > 0) ? args[0] : "";
            switch (cmd)
            {
                case "console":
                    var bot = new Bot();
                    bot.Start();
                    Console.WriteLine("Бот запущен, для завершения нажмите Enter");
                    Console.ReadLine();
                    break;
                default:
                    var svc = new BotService();
                    ServiceBase.Run(svc);
                    break;
            }
            
        }
    }
}
