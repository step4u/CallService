using Com.Huen.Libs;
using System;
using System.Threading;
using System.Windows;

namespace CorePMS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        Mutex _mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            string _mutexName = "COREPMS";

            try
            {
                _mutex = new Mutex(false, _mutexName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Shutdown();
            }

            if (_mutex.WaitOne(0, false))
            {
                base.OnStartup(e);

                this.StartupUri = new Uri("pack://application:,,,/HCSPHONELIBS;component/Views/PMS.xaml", UriKind.RelativeOrAbsolute);
            }
            else
            {
                MessageBox.Show(util.LoadProjectResource("MSG_TXT_MUTEX", "COMMONRES", "").ToString(), "HUENPMS", MessageBoxButton.OK);
                Application.Current.Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Application.Current.Shutdown();
        }
    }
}
