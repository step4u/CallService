using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Threading;

using Com.Huen.Sql;
using Com.Huen.Sockets;
using Com.Huen.Libs;
using Com.Huen.DataModel;

namespace Com.Huen.Sockets
{
    public class HotelHelper : IDisposable
    {
        private const int UDP_WAITING_MISEC = 2000;
        private string PBXIP = string.Empty;
        private int PBXPORT = 33003;

        public string PBXIp
        {
            get { return this.PBXIP; }
            set { this.PBXIP = value; }
        }

        public int PBXPort
        {
            get { return this.PBXPORT; }
            set { this.PBXPORT = value; }
        }

        public HotelHelper() : this ("127.0.0.1", 33003)
        {
        }

        public HotelHelper(string _pbxip) : this(_pbxip, 33003)
        {
        }

        public HotelHelper(string _pbxip, int _pbxport)
        {
            PBXIp = _pbxip;
            PBXPort = _pbxport;
        }


        // 모닝콜 조회
        public _pms_data_type GetMorningCall(string _ext)
        {
            IPEndPoint _serverEP;
            IPEndPoint _remoteEP;
            UdpClient _client;
            _pms_data_type pms_data_type;

            try
            {
                _serverEP = new IPEndPoint(IPAddress.Parse(PBXIP), PBXPORT);
                _remoteEP = new IPEndPoint(IPAddress.Any, 0);

                _client = new UdpClient();
                _client.Client.ReceiveTimeout = UDP_WAITING_MISEC;
                _client.Connect(_serverEP);

                pms_data_type = new _pms_data_type()
                {
                    cmd = STRUCTS.CGI_PMS_GET_MORNING_CALL_REQ
                    ,
                    extension = _ext
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
            IPEndPoint _serverEP = new IPEndPoint(IPAddress.Parse(PBXIP), PBXPORT);
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
            IPEndPoint _serverEP = new IPEndPoint(IPAddress.Parse(PBXIP), PBXPORT);
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
            IPEndPoint _serverEP = new IPEndPoint(IPAddress.Parse(PBXIP), PBXPORT);
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
            IPEndPoint _serverEP = new IPEndPoint(IPAddress.Parse(PBXIP), PBXPORT);
            IPEndPoint _remoteEP = new IPEndPoint(IPAddress.Any, 0);
            UdpClient _client = new UdpClient();
            _client.Client.ReceiveTimeout = UDP_WAITING_MISEC;
            _client.Connect(_serverEP);

            _cgi_pms_data_type cgi_pms_data_type = new _cgi_pms_data_type()
            {
                cmd = STRUCTS.CGI_PMS_GET_ALL_REQ
                , extension = _ext
            };
            byte[] _sbuffer = util.GetBytes(cgi_pms_data_type);
            byte[] _rbuffer = null;

            try
            {
                _client.Send(_sbuffer, _sbuffer.Length);
                _rbuffer = _client.Receive(ref _remoteEP);

                cgi_pms_data_type = util.GetObject<_cgi_pms_data_type>(_rbuffer);
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

                cgi_pms_data_type = new _cgi_pms_data_type() { cmd = STRUCTS.CGI_PMS_GET_OUTGOING_POLICY_RES, status = states, extension = _ext, allowedPrefix = string.Empty, forbiddenPrefix = string.Empty };
            }

            _client.Close();

            return cgi_pms_data_type;
        }

        // 체크인/아웃 발신 허용/금지
        public bool CheckIn(chkinroom _chkinroom)
        {
            IPEndPoint _serverEP = new IPEndPoint(IPAddress.Parse(PBXIP), PBXPORT);
            IPEndPoint _remoteEP = new IPEndPoint(IPAddress.Any, 0);
            UdpClient _client = new UdpClient();
            _client.Client.ReceiveTimeout = UDP_WAITING_MISEC;
            _client.Connect(_serverEP);

            string allowedstr = string.Empty;
            string forbiddenstr = string.Empty;

            _cgi_pms_data_type cgi_pms_data_type = new _cgi_pms_data_type()
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

            cgi_pms_data_type.allowedPrefix = allowedstr;
            cgi_pms_data_type.forbiddenPrefix = forbiddenstr;
            //pms_data_type.forbiddenPrefix = "";

            byte[] _sbuffer = util.GetBytes(cgi_pms_data_type);
            byte[] _rbuffer = null;
            //_cgi_res_hdr cgi_res_hdr;

            try
            {
                _client.Send(_sbuffer, _sbuffer.Length);
                _rbuffer = _client.Receive(ref _remoteEP);

                cgi_pms_data_type = util.GetObject<_cgi_pms_data_type>(_rbuffer);
            }
            catch (SocketException sockex)
            {
                util.WriteLog(sockex.Message);
                cgi_pms_data_type = new _cgi_pms_data_type() { cmd = STRUCTS.CGI_PMS_SET_ALL_RES, status = 1 };
            }

            _client.Close();

            bool _result = false;

            if (cgi_pms_data_type.cmd == STRUCTS.CGI_PMS_SET_ALL_RES)
            {
                switch (cgi_pms_data_type.status)
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

        public bool CheckIn(_cgi_pms_data_type pms_data_type)
        {
            IPEndPoint _serverEP = new IPEndPoint(IPAddress.Parse(PBXIP), PBXPORT);
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

                pms_data_type = util.GetObject<_cgi_pms_data_type>(_rbuffer);
            }
            catch (SocketException sockex)
            {
                util.WriteLog(sockex.Message);
                pms_data_type = new _cgi_pms_data_type() { cmd = STRUCTS.CGI_PMS_SET_ALL_RES, status = 1 };
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
            IPEndPoint _serverEP = new IPEndPoint(IPAddress.Parse(PBXIP), PBXPORT);
            IPEndPoint _remoteEP = new IPEndPoint(IPAddress.Any, 0);
            UdpClient _client = new UdpClient();
            _client.Client.ReceiveTimeout = UDP_WAITING_MISEC;
            _client.Connect(_serverEP);

            _cgi_pms_data_type pms_data_type = new _cgi_pms_data_type()
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

                pms_data_type = util.GetObject<_cgi_pms_data_type>(_rbuffer);
            }
            catch (SocketException sockex)
            {
                util.WriteLog(sockex.Message);
                pms_data_type = new _cgi_pms_data_type() { cmd = STRUCTS.CGI_PMS_CLEAR_FUNCTION_KEY_RES, status = 1 };
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
            IPEndPoint _serverEP = new IPEndPoint(IPAddress.Parse(PBXIP), PBXPORT);
            IPEndPoint _remoteEP = new IPEndPoint(IPAddress.Any, 0);
            UdpClient _client = new UdpClient();
            _client.Client.ReceiveTimeout = UDP_WAITING_MISEC;
            _client.Connect(_serverEP);

            _cgi_pms_data_type pms_data_type = new _cgi_pms_data_type()
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

                pms_data_type = util.GetObject<_cgi_pms_data_type>(_rbuffer);
            }
            catch (SocketException sockex)
            {
                util.WriteLog(sockex.Message);
                pms_data_type = new _cgi_pms_data_type() { cmd = STRUCTS.CGI_PMS_SET_ALL_RES, status = 1 };
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

            IPEndPoint _serverEP = new IPEndPoint(IPAddress.Parse(PBXIP), PBXPORT);
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
                _rbuffer = _client.Receive(ref _remoteEP);

                pms_rdata = util.GetObject<_cgi_pms_data_type>(_rbuffer);

                if (pms_rdata.status == 0)
                {
                    result = true;
                }
            }
            catch (SocketException sockex)
            {
                util.WriteLog(sockex.Message);
                // pms_rdata = new _pms_data_type() { cmd = STRUCTS.PMS_CLEAR_FUNCTION_KEY_RES, status = 1 };
                result = false;
            }
            _client.Close();

            return result;
        }

        // for House Keep
        public bool SetHouseKeep(string code, string ext, string mtime)
        {
            bool result = false;

            IPEndPoint _serverEP = new IPEndPoint(IPAddress.Parse(PBXIP), PBXPORT);
            IPEndPoint _remoteEP = new IPEndPoint(IPAddress.Any, 0);
            UdpClient _client = new UdpClient();
            _client.Client.ReceiveTimeout = UDP_WAITING_MISEC;
            _client.Connect(_serverEP);

            _pms_data_type pms_sdata = this.GetPolicies4HouseKeep(code, ext, mtime);
            if (pms_sdata.cmd == 0) return false;

            _pms_data_type pms_rdata = new _pms_data_type();

            byte[] _sbuffer = util.GetBytes(pms_sdata);
            byte[] _rbuffer = null;

            try
            {
                _client.Send(_sbuffer, _sbuffer.Length);
                _rbuffer = _client.Receive(ref _remoteEP);

                pms_rdata = util.GetObject<_pms_data_type>(_rbuffer);

                if (pms_rdata.status == 0)
                {
                    result = true;
                }
            }
            catch (SocketException sockex)
            {
                util.WriteLog(sockex.Message);
                // pms_rdata = new _pms_data_type() { cmd = STRUCTS.PMS_CLEAR_FUNCTION_KEY_RES, status = 1 };
                result = false;
            }
            _client.Close();

            return result;
        }

        public bool RestoreSystem(_pms_data_type data)
        {
            bool result = false;
            _pms_data_type rdata = new _pms_data_type();

            IPEndPoint _serverEP = new IPEndPoint(IPAddress.Parse(PBXIP), PBXPORT);
            IPEndPoint _remoteEP = new IPEndPoint(IPAddress.Any, 0);
            UdpClient _client = new UdpClient();
            _client.Client.ReceiveTimeout = UDP_WAITING_MISEC;
            _client.Connect(_serverEP);

            data.cmd = STRUCTS.CGI_PMS_SET_ALL_REQ;
            byte[] _sbuffer = util.GetBytes(data);
            byte[] _rbuffer = null;

            try
            {
                _client.Send(_sbuffer, _sbuffer.Length);
                _rbuffer = _client.Receive(ref _remoteEP);

                rdata = util.GetObject<_pms_data_type>(_rbuffer);
            }
            catch (SocketException sockex)
            {
                util.WriteLog(sockex.Message);
                result = false;
            }
            _client.Close();

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

        protected _pms_data_type GetPolicies4HouseKeep(string code, string ext, string mtime)
        {
            _pms_data_type data = new _pms_data_type();
 
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
                    string hh = mtime.Substring(0, 2);
                    string mm = mtime.Substring(2, 2);
                    int hour = string.IsNullOrEmpty(hh) == true ? 0 : int.Parse(hh);
                    int minutes = string.IsNullOrEmpty(mm) == true ? 0 : int.Parse(mm);
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

    }
}
