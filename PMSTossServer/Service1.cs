using Com.Huen.Sockets;
using System.ServiceProcess;

namespace PMSTossServer
{
    public partial class Service1 : ServiceBase
    {
        private TossServer ws;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            ws = new TossServer();
        }

        protected override void OnStop()
        {
            if (ws != null)
            {
                ws.Dispose();
            }
        }
    }
}
