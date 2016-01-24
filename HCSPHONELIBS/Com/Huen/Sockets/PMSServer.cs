using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Com.Huen.Libs;
using Com.Huen.DataModel;
using Com.Huen.Sockets;
using Com.Huen.Sql;

using System.IO;
using System.Data;
using System.Threading;
using System.Windows.Threading;
using System.Net;
using System.Net.Sockets;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Com.Huen.Sockets
{
    public delegate void RequestPMSSetEventHandler(PMSServer sender, _pms_data_type pmsdata);

    public class PMSServer : IDisposable
    {
        public event RequestPMSSetEventHandler ReqPMSSetEvent;

        private Socket _sockSrv = null;
        private EndPoint _localep;
        private EndPoint _remoteep;
        private Thread _thread;
        private bool _srvLoop = true;

        public PMSServer() : this(21004)
        {
        }

        public PMSServer(int port)
        {
            _localep = new IPEndPoint(IPAddress.Any, port);
            _remoteep = new IPEndPoint(IPAddress.Any, 0);

            try
            {
                _sockSrv = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                _sockSrv.Bind(_localep);

                this.StartSockThread();
            }
            catch(SocketException se)
            {
                string __msg = string.Format("CdrSrv socket start error: {0}", se.ErrorCode);
                util.WriteLog(__msg);
            }
            catch (ThreadStartException te)
            {
                string __msg = string.Format("PMSServer socket thread start error: {0}", te.Message);
                util.WriteLog(__msg);
            }
        }

        public void Dispose()
        {
            _sockSrv.Close(); _sockSrv = null;
        }

        private void StartSockThread()
        {
            _thread = new Thread(new ThreadStart(SrvReceiver));
            _thread.IsBackground = true;
            _thread.Start();
        }

        private void SrvReceiver()
        {
            int _count = 0;

            while (_srvLoop)
            {
                byte[] _buffer = new byte[1024];
                _count = 0;

                try
                {
                    _count = _sockSrv.ReceiveFrom(_buffer, SocketFlags.None, ref _remoteep);
                }
                catch (SocketException se)
                {
                    string _msg = string.Format("PMSServer socket receive error: {0}", se.ErrorCode);
                    util.WriteLog(_msg);
                }

                if (_count == 0) continue;

                byte[] _databuffer = new byte[_count];
                Buffer.BlockCopy(_buffer, 0, _databuffer, 0, _count);

                _pms_data_type _pmsdata = util.GetObject<_pms_data_type>(_databuffer);

                if (ReqPMSSetEvent != null)
                    ReqPMSSetEvent(this, _pmsdata);
            }
        }
    }
}
