using System;
using System.Net;
using System.Net.Sockets;
using Com.Huen.Libs;
using Com.Huen.DataModel;
using System.Threading;
using System.Linq;

namespace Com.Huen.Sockets
{
    public delegate void PassDevice2PmsEventHandler(object sender, _pms_data_type pmsdata);

    public class HotelHelper2 : IDisposable
    {
        public event PassDevice2PmsEventHandler PassDevice2PmsEvent;

        private IPEndPoint remoteEp;
        private int expires = 60 * 1000;

        private UdpClient client;
        private Thread sockthread;
        private const int UDP_WAITING_MISEC = 2000;
        private string PBXIP = string.Empty;
        private int PBXPORT = 21007;
        private int PBXPORTCGI = 33003;

        IPAddress LOCALIPADDRESS = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        private int LOCALPORT = 21008;

        private bool IsRegistered = false;
        private System.Timers.Timer regtimer = null;

        public HotelHelper2() : this ("127.0.0.1", 21007)
        {
        }

        public HotelHelper2(string _pbxip) : this(_pbxip, 21007)
        {
        }

        public HotelHelper2(string _pbxip, int _pbxport)
        {
            PBXIP = _pbxip;
            PBXPORT = _pbxport;
            
            this.InitSocket();
        }

        private void InitSocket()
        {
            try
            {
                remoteEp = new IPEndPoint(IPAddress.Parse(this.PBXIP), this.PBXPORT);
                IPEndPoint localEp = new IPEndPoint(IPAddress.Any, LOCALPORT);
                client = new UdpClient(localEp);
                // client.Client.ReceiveTimeout = UDP_WAITING_MISEC;
                client.Connect(remoteEp);

                sockthread = new Thread(new ThreadStart(SendReceiveMessage));
                sockthread.IsBackground = true;
                sockthread.Start();

                this.Register();
            }
            catch (SocketException e)
            {
                util.WriteLog(e.ErrorCode, e.Message.ToString());
            }
            catch (Exception e)
            {
                util.WriteLog(e.Message.ToString());
            }
        }

        public void StopSocket()
        {
            try
            {
                //if (sockthread == null) return;

                //if (sockthread.IsAlive)
                //{
                //    sockthread.Abort();
                //}

                if (client != null)
                {
                    client.Close();
                }
            }
            catch (SocketException e)
            {
                util.WriteLog(e.Message.ToString());
            }
            catch (Exception e)
            {
                util.WriteLog(e.Message.ToString());
            }
        }

        public void RegTimerInit()
        {
            if (IsRegistered) return;

            regtimer = new System.Timers.Timer();
            regtimer.Interval = expires;
            regtimer.Elapsed += Regtimer_Elapsed;
            regtimer.Start();
        }

        public void RegTimerDispose()
        {
            if (!IsRegistered) return;
            if (regtimer == null) return;

            regtimer.Stop();
            regtimer.Close();
            regtimer.Dispose();
        }

        private void Regtimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Register();
            // System.Diagnostics.Debug.WriteLine("Registered");
        }

        public void Register()
        {
            _pms_reg_t _msg = GetMsg(STRUCTS.PMS_REGISTER_REQ);
            byte[] _sbuffer = util.GetBytes(_msg);

            try
            {
                client.Send(_sbuffer, _sbuffer.Length);
            }
            catch (SocketException sockex)
            {
                util.WriteLog(sockex.Message);
            }
        }

        public void UnRegister()
        {
            _pms_reg_t _msg = GetMsg(STRUCTS.PMS_UNREGISTER_REQ);
            byte[] _sbuffer = util.GetBytes(_msg);

            try
            {
                client.Send(_sbuffer, _sbuffer.Length);
            }
            catch (SocketException sockex)
            {
                util.WriteLog(sockex.Message);
            }
        }

        public void Send(_pms_data_type rdata)
        {
            _pms_data_type msg = GetMsg(rdata);
            byte[] buffer = util.GetBytes(msg);

            try
            {
                client.Send(buffer, buffer.Length);
            }
            catch (SocketException ex)
            {
                throw ex;
            }
        }

        private void SendReceiveMessage()
        {
            while (true)
            {
                byte[] buffer = null;

                try
                {
                    buffer = client.Receive(ref remoteEp);
                }
                catch (SocketException se)
                {
                    string message = string.Format("HotelHelper2 socket receive error: {0}", se.ErrorCode);
                    util.WriteLog(message);
                }

                if (buffer == null) continue;

                _pms_reg_t msg = util.GetObject<_pms_reg_t>(buffer);

                switch (msg.cmd)
                {
                    case STRUCTS.PMS_REGISTER_RES:
                        if (msg.status == STRUCTS.PMS_STATUS_SUCCESS)
                        {
                            this.expires = msg.expires * 1000;
                            this.RegTimerInit();
                            this.IsRegistered = true;
                        }
                        break;
                    case STRUCTS.PMS_UNREGISTER_RES:
                        if (msg.status == STRUCTS.PMS_STATUS_SUCCESS)
                        {
                            RegTimerDispose();
                            this.expires = msg.expires;
                            this.IsRegistered = false;
                        }
                        break;
                    default:
                        _pms_data_type pmsmsg = util.GetObject<_pms_data_type>(buffer);

                        if (PassDevice2PmsEvent != null)
                            PassDevice2PmsEvent(this, pmsmsg);
                        break;
                }
            }
        }

        // 모닝콜 조회
        public _pms_data_type GetMorningCall(string _ext)
        {
            IPEndPoint _serverEP = new IPEndPoint(IPAddress.Parse(PBXIP), PBXPORTCGI);
            IPEndPoint _remoteEP = new IPEndPoint(IPAddress.Any, 0);
            UdpClient _client;
            _pms_data_type pms_data_type;

            try
            {
                _client = new UdpClient();
                _client.Client.ReceiveTimeout = UDP_WAITING_MISEC;
                _client.Connect(_serverEP);

                pms_data_type = new _pms_data_type()
                {
                    cmd = STRUCTS.CGI_PMS_GET_MORNING_CALL_REQ
                    , extension = _ext
                };

                byte[] _sbuffer = util.GetBytes(pms_data_type);
                byte[] _rbuffer = null;

                _client.Send(_sbuffer, _sbuffer.Length);
                _rbuffer = _client.Receive(ref _remoteEP);

                pms_data_type = util.GetObject<_pms_data_type>(_rbuffer);

                _client.Close();
            }
            catch (SocketException ex)
            {
                pms_data_type = new _pms_data_type() { cmd = STRUCTS.CGI_PMS_CLEAR_MORNING_CALL_RES, status = 1, extension = _ext, hour = -1, minutes = -1, try_interval = 0, repeat_times = 0, repeat_week = 0, ring_duration = 0, week = string.Empty };
            }

            return pms_data_type;
        }

        // 모닝콜 설정
        public bool SetMorningCall(string _ext, string _week, int _hour, int _minutes, int repeat_week)
        {
            IPEndPoint _serverEP = new IPEndPoint(IPAddress.Parse(PBXIP), PBXPORTCGI);
            IPEndPoint _remoteEP = new IPEndPoint(IPAddress.Any, 0);
            UdpClient _client = new UdpClient();
            _client.Client.ReceiveTimeout = UDP_WAITING_MISEC;
            _client.Connect(_serverEP);

            _pms_data_type pms_data_type = new _pms_data_type()
            {
                cmd = STRUCTS.CGI_PMS_CLEAR_MORNING_CALL_REQ
                ,
                extension = _ext
                ,
                week = _week
                ,
                hour = _hour
                ,
                minutes = _minutes
                ,
                try_interval = 3
                ,
                repeat_times = 5
                ,
                repeat_week = repeat_week
                ,
                ring_duration = 120
            };

            byte[] _sbuffer = util.GetBytes(pms_data_type);
            byte[] _rbuffer = null;

            try
            {
                _client.Send(_sbuffer, _sbuffer.Length);
                _rbuffer = _client.Receive(ref _remoteEP);

                pms_data_type = util.GetObject<_pms_data_type>(_rbuffer);
            }
            catch (SocketException e)
            {
                pms_data_type = new _pms_data_type() { cmd = STRUCTS.CGI_PMS_CLEAR_MORNING_CALL_RES, status = 1, extension = _ext, hour = -1, minutes = -1, try_interval = 0, repeat_times = 0, repeat_week = 0, ring_duration = 0, week = string.Empty };
            }

            _client.Close();

            bool _result = false;

            switch (pms_data_type.status)
            {
                case 0:
                    _result = true;
                    break;
                case 1:
                    _result = false;
                    break;
                default:
                    _result = false;
                    break;
            }

            return _result;
        }

        // 모닝콜 삭제
        public bool ClearMorningCall(string _ext)
        {
            IPEndPoint _serverEP = new IPEndPoint(IPAddress.Parse(PBXIP), PBXPORTCGI);
            IPEndPoint _remoteEP = new IPEndPoint(IPAddress.Any, 0);
            UdpClient _client = new UdpClient();
            _client.Client.ReceiveTimeout = UDP_WAITING_MISEC;
            _client.Connect(_serverEP);

            _pms_data_type pms_data_type = new _pms_data_type()
            {
                cmd = STRUCTS.CGI_PMS_CLEAR_MORNING_CALL_REQ
                , extension = _ext
            };
            byte[] _sbuffer = util.GetBytes(pms_data_type);
            byte[] _rbuffer = null;
            //_cgi_res_hdr cgi_res_hdr;

            try
            {
                _client.Send(_sbuffer, _sbuffer.Length);
                _rbuffer = _client.Receive(ref _remoteEP);

                pms_data_type = util.GetObject<_pms_data_type>(_rbuffer);
                pms_data_type.status = 0;
            }
            catch(SocketException sockex)
            {
                util.WriteLog(sockex.Message);
                pms_data_type = new _pms_data_type() { cmd = STRUCTS.CGI_PMS_CLEAR_MORNING_CALL_RES, status = 1 };
            }
            _client.Close();

            bool _result = false;

            switch (pms_data_type.status)
            {
                case 0:
                    _result = true;
                    break;
                case 1:
                    _result = false;
                    break;
                default:
                    _result = false;
                    break;
            }

            return _result;
        }

        public bool ClearMorningCall2(string _ext)
        {
            IPEndPoint _serverEP = new IPEndPoint(IPAddress.Parse(PBXIP), PBXPORTCGI);
            IPEndPoint _remoteEP = new IPEndPoint(IPAddress.Any, 0);
            UdpClient _client = new UdpClient();
            _client.Client.ReceiveTimeout = UDP_WAITING_MISEC;
            _client.Connect(_serverEP);

            _pms_data_type pms_data_type = new _pms_data_type()
            {
                cmd = STRUCTS.CGI_PMS_CLEAR_MORNING_CALL_REQ
                ,
                extension = _ext
                ,
                hour = 255
            };

            byte[] _sbuffer = util.GetBytes(pms_data_type);

            _client.Send(_sbuffer, _sbuffer.Length);
            byte[] _rbuffer = _client.Receive(ref _remoteEP);
            _client.Close();

            pms_data_type = util.GetObject<_pms_data_type>(_rbuffer);

            bool _result = false;

            switch (pms_data_type.status)
            {
                case 0:
                    _result = false;
                    break;
                case 1:
                    _result = true;
                    break;
                default:
                    _result = false;
                    break;
            }

            return _result;
        }


        // 체크인/아웃 발신 허용/금지 조회
        public _cgi_pms_data_type GetPolicy(string _ext)
        {
            IPEndPoint _serverEP = new IPEndPoint(IPAddress.Parse(PBXIP), PBXPORTCGI);
            IPEndPoint _remoteEP = new IPEndPoint(IPAddress.Any, 0);
            UdpClient _client = new UdpClient();
            _client.Client.ReceiveTimeout = UDP_WAITING_MISEC;
            _client.Connect(_serverEP);

            _cgi_pms_data_type pms_data_type = new _cgi_pms_data_type()
            {
                cmd = STRUCTS.CGI_PMS_GET_ALL_REQ
                , extension = _ext
            };
            byte[] _sbuffer = util.GetBytes(pms_data_type);
            byte[] _rbuffer = null;

            try
            {
                _client.Send(_sbuffer, _sbuffer.Length);
                _rbuffer = _client.Receive(ref _remoteEP);

                pms_data_type = util.GetObject<_cgi_pms_data_type>(_rbuffer);
            }
            catch (SocketException ex)
            {
                string err_msg = string.Format("{0}\r\n{1}", ex.ErrorCode, ex.Message);
                util.WriteLog(err_msg);

                int states = -1;
                if (ex.SocketErrorCode == SocketError.TimedOut)
                {
                    states = STRUCTS.ERR_SOCKET_TIMEOUT;
                }
                else
                {
                    states = 1;
                }

                pms_data_type = new _cgi_pms_data_type() { cmd = STRUCTS.CGI_PMS_GET_OUTGOING_POLICY_RES, status = states, extension = _ext, allowedPrefix = string.Empty, forbiddenPrefix = string.Empty };
            }

            _client.Close();

            return pms_data_type;
        }

        // 체크인/아웃 발신 허용/금지
        public bool CheckIn(chkinroom _chkinroom)
        {
            IPEndPoint _serverEP = new IPEndPoint(IPAddress.Parse(PBXIP), PBXPORTCGI);
            IPEndPoint _remoteEP = new IPEndPoint(IPAddress.Any, 0);
            UdpClient _client = new UdpClient();
            _client.Client.ReceiveTimeout = UDP_WAITING_MISEC;
            _client.Connect(_serverEP);

            string allowedstr = string.Empty;
            string forbiddenstr = string.Empty;

            _pms_data_type pms_data_type = new _pms_data_type()
            {
                cmd = STRUCTS.CGI_PMS_SET_ALL_REQ
                , extension = _chkinroom.roomnum
                , language = _chkinroom.language
                , hour = -1
                , minutes = -1
                , week = string.Empty
            };

            if (_chkinroom.allowed[0] == "all")
            {
                allowedstr = _chkinroom.allowed[0];
            }
            else
            {
                for (int i = 0; i < _chkinroom.allowed.Length; i++)
                {
                    if (string.IsNullOrEmpty(_chkinroom.allowed[i]))
                        continue;

                    if (string.IsNullOrEmpty(allowedstr))
                    {
                        allowedstr += _chkinroom.allowed[i];
                    }
                    else
                    {
                        allowedstr += string.Format(",{0}", _chkinroom.allowed[i]);
                    }
                }
            }

            if (_chkinroom.forbidden[0] == "all")
            {
                forbiddenstr = _chkinroom.forbidden[0];
            }
            else
            {
                for (int i = 0; i < _chkinroom.forbidden.Length; i++)
                {
                    if (string.IsNullOrEmpty(_chkinroom.forbidden[i]))
                        continue;

                    if (string.IsNullOrEmpty(forbiddenstr))
                    {
                        forbiddenstr += _chkinroom.forbidden[i];
                    }
                    else
                    {
                        forbiddenstr += string.Format(",{0}", _chkinroom.forbidden[i]);
                    }
                }
            }

            pms_data_type.allowedPrefix = allowedstr;
            pms_data_type.forbiddenPrefix = forbiddenstr;
            //pms_data_type.forbiddenPrefix = "";

            byte[] _sbuffer = util.GetBytes(pms_data_type);
            byte[] _rbuffer = null;
            //_cgi_res_hdr cgi_res_hdr;

            try
            {
                _client.Send(_sbuffer, _sbuffer.Length);
                _rbuffer = _client.Receive(ref _remoteEP);

                pms_data_type = util.GetObject<_pms_data_type>(_rbuffer);
            }
            catch (SocketException sockex)
            {
                util.WriteLog(sockex.Message);
                pms_data_type = new _pms_data_type() { cmd = STRUCTS.CGI_PMS_SET_ALL_RES, status = 1 };
            }

            _client.Close();

            bool _result = false;

            if (pms_data_type.cmd == STRUCTS.CGI_PMS_SET_ALL_RES)
            {
                switch (pms_data_type.status)
                {
                    case 0:
                        _result = true;
                        break;
                    case 1:
                        _result = false;
                        break;
                    default:
                        _result = false;
                        break;
                }
            }
            else
            {
                _result = false;
            }

            return _result;
        }

        public bool CheckIn(_pms_data_type pms_data_type)
        {
            IPEndPoint _serverEP = new IPEndPoint(IPAddress.Parse(PBXIP), PBXPORTCGI);
            IPEndPoint _remoteEP = new IPEndPoint(IPAddress.Any, 0);
            UdpClient _client = new UdpClient();
            _client.Client.ReceiveTimeout = UDP_WAITING_MISEC;
            _client.Connect(_serverEP);

            byte[] _sbuffer = util.GetBytes(pms_data_type);
            byte[] _rbuffer = null;

            try
            {
                _client.Send(_sbuffer, _sbuffer.Length);
                _rbuffer = _client.Receive(ref _remoteEP);

                pms_data_type = util.GetObject<_pms_data_type>(_rbuffer);
            }
            catch (SocketException sockex)
            {
                util.WriteLog(sockex.Message);
                pms_data_type = new _pms_data_type() { cmd = STRUCTS.CGI_PMS_SET_ALL_RES, status = 1 };
            }

            _client.Close();

            bool _result = false;

            if (pms_data_type.cmd == STRUCTS.CGI_PMS_SET_ALL_RES)
            {
                switch (pms_data_type.status)
                {
                    case 0:
                        _result = true;
                        break;
                    case 1:
                        _result = false;
                        break;
                    default:
                        _result = false;
                        break;
                }
            }
            else
            {
                _result = false;
            }

            return _result;
        }
        // 체크인 end

        // 체크아웃
        public bool CheckOut(string _ext)
        {
            IPEndPoint _serverEP = new IPEndPoint(IPAddress.Parse(PBXIP), PBXPORTCGI);
            IPEndPoint _remoteEP = new IPEndPoint(IPAddress.Any, 0);
            UdpClient _client = new UdpClient();
            _client.Client.ReceiveTimeout = UDP_WAITING_MISEC;
            _client.Connect(_serverEP);

            _pms_data_type pms_data_type = new _pms_data_type()
            {
                cmd = STRUCTS.CGI_PMS_CLEAR_FUNCTION_KEY_REQ,
                extension = _ext,
            };

            byte[] _sbuffer = util.GetBytes(pms_data_type);
            byte[] _rbuffer = null;

            try
            {
                _client.Send(_sbuffer, _sbuffer.Length);
                _rbuffer = _client.Receive(ref _remoteEP);

                pms_data_type = util.GetObject<_pms_data_type>(_rbuffer);
            }
            catch (SocketException sockex)
            {
                util.WriteLog(sockex.Message);
                pms_data_type = new _pms_data_type() { cmd = STRUCTS.CGI_PMS_CLEAR_FUNCTION_KEY_RES, status = 1 };
            }

            _client.Close();

            bool _result = false;

            if (pms_data_type.cmd == STRUCTS.CGI_PMS_CLEAR_FUNCTION_KEY_RES)
            {
                switch (pms_data_type.status)
                {
                    case 0:
                        _result = true;
                        break;
                    case 1:
                        _result = false;
                        break;
                    default:
                        _result = false;
                        break;
                }
            }
            else
            {
                _result = false;
            }

            return _result;
        }

        // 우편물 수령 요청
        public bool SetParcel(string _ext, int _value)
        {
            IPEndPoint _serverEP = new IPEndPoint(IPAddress.Parse(PBXIP), PBXPORTCGI);
            IPEndPoint _remoteEP = new IPEndPoint(IPAddress.Any, 0);
            UdpClient _client = new UdpClient();
            _client.Client.ReceiveTimeout = UDP_WAITING_MISEC;
            _client.Connect(_serverEP);

            _pms_data_type pms_data_type = new _pms_data_type()
            {
                cmd = STRUCTS.CGI_PMS_SET_POST_PARCEL_REQ,
                extension = _ext,
                post_parcel = _value
            };

            byte[] _sbuffer = util.GetBytes(pms_data_type);
            byte[] _rbuffer = null;

            try
            {
                _client.Send(_sbuffer, _sbuffer.Length);
                _rbuffer = _client.Receive(ref _remoteEP);

                pms_data_type = util.GetObject<_pms_data_type>(_rbuffer);
            }
            catch (SocketException sockex)
            {
                util.WriteLog(sockex.Message);
                pms_data_type = new _pms_data_type() { cmd = STRUCTS.CGI_PMS_SET_ALL_RES, status = 1 };
            }

            _client.Close();

            bool _result = false;

            if (pms_data_type.cmd == STRUCTS.CGI_PMS_SET_POST_PARCEL_RES)
            {
                switch (pms_data_type.status)
                {
                    case 0:
                        _result = true;
                        break;
                    case 1:
                        _result = false;
                        break;
                    default:
                        _result = false;
                        break;
                }
            }
            else
            {
                _result = false;
            }

            return _result;
        }

        public void Dispose()
        {

        }

        // for All Operations
        public bool SetSystem(string code, string ext, string period, string language)
        {
            bool result = false;

            IPEndPoint _serverEP = new IPEndPoint(IPAddress.Parse(PBXIP), PBXPORTCGI);
            IPEndPoint _remoteEP = new IPEndPoint(IPAddress.Any, 0);
            UdpClient _client = new UdpClient();
            _client.Client.ReceiveTimeout = UDP_WAITING_MISEC;
            _client.Connect(_serverEP);

            _cgi_pms_data_type pms_sdata = this.GetPolicies(code, ext, period, language);
            _cgi_pms_data_type pms_rdata = new _cgi_pms_data_type();

            byte[] _sbuffer = util.GetBytes(pms_sdata);
            byte[] _rbuffer = null;

            try
            {
                _client.Send(_sbuffer, _sbuffer.Length);
                _rbuffer = _client.Receive(ref remoteEp);

                pms_rdata = util.GetObject<_cgi_pms_data_type>(_rbuffer);

                if (pms_rdata.status == 0)
                {
                    result = true;
                }

                _client.Close();

                System.Diagnostics.Debug.WriteLine("PMS Sent data : cmd:{0}, status:{1}, extension:{2}", pms_sdata.cmd, pms_sdata.status, pms_sdata.extension);
                System.Diagnostics.Debug.WriteLine("PMS Received data : cmd:{0}, status:{1}, extension:{2}", pms_rdata.cmd, pms_rdata.status, pms_rdata.extension);
            }
            catch (SocketException sockex)
            {
                util.WriteLog(sockex.Message);
                result = false;
            }

            return result;
        }

        // for House Keep
        public bool SetHouseKeep(string code, string ext, string txt)
        {
            bool result = false;

            IPEndPoint _serverEP = new IPEndPoint(IPAddress.Parse(PBXIP), PBXPORTCGI);
            IPEndPoint _remoteEP = new IPEndPoint(IPAddress.Any, 0);
            UdpClient _client = new UdpClient();
            _client.Client.ReceiveTimeout = UDP_WAITING_MISEC;
            _client.Connect(_serverEP);

            _cgi_pms_data_type pms_sdata = this.GetPolicies4HouseKeep(code, ext, txt);
            if (pms_sdata.cmd == 0)
            {
                result = true;
            }
            else
            {
                _cgi_pms_data_type pms_rdata = new _cgi_pms_data_type();

                byte[] _sbuffer = util.GetBytes(pms_sdata);
                byte[] _rbuffer = null;

                try
                {
                    _client.Send(_sbuffer, _sbuffer.Length);
                    _rbuffer = _client.Receive(ref remoteEp);

                    pms_rdata = util.GetObject<_cgi_pms_data_type>(_rbuffer);

                    if (pms_rdata.status == 0)
                    {
                        result = true;
                    }

                    _client.Close();
                }
                catch (SocketException sockex)
                {
                    util.WriteLog(sockex.Message);
                    result = false;
                }
            }

            return result;
        }

        public bool RestoreSystem(_cgi_pms_data_type data)
        {
            bool result = false;

            IPEndPoint _serverEP = new IPEndPoint(IPAddress.Parse(PBXIP), PBXPORTCGI);
            IPEndPoint _remoteEP = new IPEndPoint(IPAddress.Any, 0);
            UdpClient _client = new UdpClient();
            _client.Client.ReceiveTimeout = UDP_WAITING_MISEC;
            _client.Connect(_serverEP);

            _cgi_pms_data_type rdata = new _cgi_pms_data_type();

            data.cmd = STRUCTS.CGI_PMS_SET_ALL_REQ;
            byte[] _sbuffer = util.GetBytes(data);
            byte[] _rbuffer = null;

            try
            {
                _client.Send(_sbuffer, _sbuffer.Length);
                _rbuffer = _client.Receive(ref remoteEp);

                rdata = util.GetObject<_cgi_pms_data_type>(_rbuffer);

                _client.Close();
            }
            catch (SocketException sockex)
            {
                util.WriteLog(sockex.Message);
                result = false;
            }

            if (rdata.cmd == STRUCTS.CGI_PMS_SET_ALL_RES)
            {
                if (rdata.status == 0)
                {
                    result = true;
                }
            }

            return result;
        }

        protected _cgi_pms_data_type GetPolicies(string code, string ext, string period, string language)
        {
            _cgi_pms_data_type data = new _cgi_pms_data_type();
            DateTime tmpdate;

            switch (code)
            {
                case "0":
                    // 퇴실
                    data.cmd = STRUCTS.CGI_PMS_CLEAR_FUNCTION_KEY_REQ;
                    data.extension = ext;
                    break;
                case "1":
                    // 대실
                    tmpdate = DateTime.Now.AddHours(4);
                    data.cmd = STRUCTS.CGI_PMS_SET_ALL_REQ;
                    data.extension = ext;
                    data.checkout_month = tmpdate.Month;
                    data.checkout_day = tmpdate.Day;
                    data.checkout_hour = tmpdate.Hour;
                    data.checkout_minitues = tmpdate.Minute;
                    data.checkout_before_min = 30;
                    data.checkout_try_interval = 10;
                    data.checkout_repeat_times = 2;
                    data.checkout_ring_duration = 30;
                    data.language = SetLanguage(language);
                    data.allowedPrefix = "all";
                    data.forbiddenPrefix = "";
                    break;
                case "2":
                    // 숙박
                    tmpdate = DateTime.Now.AddDays(string.IsNullOrEmpty(period) == true ? 0 : int.Parse(period));
                    data.cmd = STRUCTS.CGI_PMS_SET_ALL_REQ;
                    data.extension = ext;
                    data.checkout_month = tmpdate.Month;
                    data.checkout_day = tmpdate.Day;
                    data.checkout_hour = 12;
                    data.checkout_minitues = 0;
                    data.checkout_before_min = 30;
                    data.checkout_try_interval = 10;
                    data.checkout_repeat_times = 2;
                    data.checkout_ring_duration = 30;
                    data.language = SetLanguage(language);
                    data.allowedPrefix = "all";
                    data.forbiddenPrefix = "";
                    break;
                case "5":
                    // 투숙일 변경
                    tmpdate = DateTime.Now.AddDays(string.IsNullOrEmpty(period) == true ? 0 : int.Parse(period));
                    data.cmd = STRUCTS.CGI_PMS_SET_CHECKOUT_TIME_REQ;
                    data.extension = ext;
                    data.checkout_month = tmpdate.Month;
                    data.checkout_day = tmpdate.Day;
                    data.checkout_hour = 12;
                    data.checkout_minitues = 0;
                    data.checkout_before_min = 30;
                    data.checkout_try_interval = 10;
                    data.checkout_repeat_times = 2;
                    data.checkout_ring_duration = 30;
                    data.language = SetLanguage(language);
                    break;
                case "O":
                    // 정보 변경
                    // 언어설정
                    data.cmd = STRUCTS.CGI_PMS_SET_LANGUAGE_REQ;
                    data.extension = ext;
                    data.language = SetLanguage(language);
                    break;
                default:
                    break;
            }

            return data;
        }

        private int SetLanguage(string language)
        {
            int lang = 2;

            if (language.ToLower().Equals("kor"))
            {
                lang = 2;
            }
            else if (language.ToLower().Equals("eng"))
            {
                lang = 1;
            }
            else if (language.ToLower().Equals("chi"))
            {
                lang = 5;
            }
            else if (language.ToLower().Equals("jap"))
            {
                lang = 6;
            }
            else
            {
                lang = 2;
            }

            return lang;
        }

        protected _cgi_pms_data_type GetPolicies4HouseKeep(string code, string ext, string txt)
        {
            _cgi_pms_data_type data = new _cgi_pms_data_type();
 
            switch (code)
            {
                case "A":
                    // 우편물 도착 알림
                    data.cmd = STRUCTS.CGI_PMS_SET_POST_PARCEL_REQ;
                    data.extension = ext;
                    data.post_parcel = 1;
                    break;
                case "B":
                    // 우편물 도착 알림 취소
                    data.cmd = STRUCTS.CGI_PMS_SET_POST_PARCEL_REQ;
                    data.extension = ext;
                    data.post_parcel = 0;
                    break;
                case "C":
                    // 모닝콜 설정
                    data.cmd = STRUCTS.CGI_PMS_SET_MORNING_CALL_REQ;
                    data.extension = ext;
                    string hh = txt.Substring(8, 2);
                    string mm = txt.Substring(10, 2);
                    data.hour = string.IsNullOrEmpty(hh) == true ? 0 : int.Parse(hh);
                    data.minutes = string.IsNullOrEmpty(mm) == true ? 0 : int.Parse(mm);
                    break;
                case "D":
                    // 모닝콜 취소
                    data.cmd = STRUCTS.CGI_PMS_CLEAR_MORNING_CALL_REQ;
                    data.extension = ext;
                    break;
                case "J":
                    // DND 취소
                    
                    break;
                default:
                    data.cmd = 0;
                    break;
            }

            return data;
        }

        protected bool GetResult(_pms_data_type sdata, _pms_data_type rdata)
        {
            bool result = false;

            if (sdata.cmd == STRUCTS.CGI_PMS_CLEAR_FUNCTION_KEY_RES)
            {
                switch (sdata.status)
                {
                    case 0:
                        result = true;
                        break;
                    case 1:
                        result = false;
                        break;
                    default:
                        result = false;
                        break;
                }
            }
            else
            {
                result = false;
            }

            return result;
        }

        public _pms_reg_t GetMsg(int _cmd)
        {
            _pms_reg_t msg = new _pms_reg_t();

            switch (_cmd)
            {
                case STRUCTS.PMS_REGISTER_REQ:
                case STRUCTS.PMS_UNREGISTER_REQ:
                    msg.cmd = _cmd;
                    msg.ip = LOCALIPADDRESS.ToString();
                    msg.port = LOCALPORT;
                    msg.status = 0;
                    break;
            }
            return msg;
        }

        public _pms_data_type GetMsg(_pms_data_type rdata)
        {
            _pms_data_type msg = new _pms_data_type();

            switch (rdata.cmd)
            {
                case STRUCTS.PMS_SET_MORNING_CALL_REQ:
                    msg.cmd = STRUCTS.PMS_SET_MORNING_CALL_RES;
                    msg.status = rdata.status;
                    msg.pList = rdata.pList;
                    msg.extension = rdata.extension;
                    break;
                case STRUCTS.PMS_SET_LANGUAGE_REQ:
                    msg.cmd = STRUCTS.PMS_SET_LANGUAGE_RES;
                    msg.status = rdata.status;
                    msg.pList = rdata.pList;
                    msg.extension = rdata.extension;
                    break;
                case STRUCTS.PMS_REPORT_FUNCTION_KEY_REQ:
                    msg.cmd = STRUCTS.PMS_REPORT_FUNCTION_KEY_RES;
                    msg.status = rdata.status;
                    msg.pList = rdata.pList;
                    msg.extension = rdata.extension;
                    break;
                case STRUCTS.PMS_REPORT_MAKEUP_STATUS_REQ:
                    msg.cmd = STRUCTS.PMS_REPORT_MAKEUP_STATUS_RES;
                    msg.status = rdata.status;
                    msg.pList = rdata.pList;
                    msg.extension = rdata.extension;
                    break;
            }
            return msg;
        }

    }
}
