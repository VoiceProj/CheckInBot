using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace CheckInBot
{
    partial class BotService : ServiceBase
    {
        public BotService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: Добавьте код для запуска службы.
        }

        protected override void OnStop()
        {
            // TODO: Добавьте код, выполняющий подготовку к остановке службы.
        }

        protected override void OnPause()
        {
            
        }

        protected override void OnContinue()
        {
            
        }
    }
}
