using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Com.Huen.Sockets;

namespace ReplayServer
{
    public partial class ReplayService : ServiceBase
    {
        private RelayService server;

        public ReplayService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            server = new RelayService();
        }

        protected override void OnStop()
        {
        }
    }
}
