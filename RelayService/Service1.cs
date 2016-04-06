using System.ServiceProcess;

namespace RelayService
{
    public partial class RelayService : ServiceBase
    {
        private Com.Huen.Sockets.RelayService rs = null;

        public RelayService()
        {
            InitializeComponent();

            System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);
        }

        protected override void OnStart(string[] args)
        {
            rs = new Com.Huen.Sockets.RelayService();
        }

        protected override void OnStop()
        {
        }
    }
}
