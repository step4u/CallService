using System;
using System.Windows;

using Com.Huen.Sockets;

namespace TestTossServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TossServer ws;
        RelayService relayserver;

        public MainWindow()
        {
            InitializeComponent();
            this.Closing += MainWindow_Closing;
            this.Closed += MainWindow_Closed;

            ws = new TossServer();
            this.WindowState = WindowState.Minimized;

            //relayserver = new RelayService();
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ws != null)
            {
                ws.Dispose();
            }
        }
    }
}
