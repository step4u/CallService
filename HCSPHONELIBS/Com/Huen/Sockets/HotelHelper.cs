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
        private const int UDP_WAITING_MISEC = 3000;
        private string PBXIP = string.Empty;
        private int PBXPORT = 33003;

        public HotelHelper() : this ("127.0.0.1", 33003)
        {
        }

        public HotelHelper(string _pbxip) : this(_pbxip, 33003)
        {
        }

        public HotelHelper(string _pbxip, int _pbxport)
        {
            PBXIP = _pbxip;
            PBXPORT = _pbxport;
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
                    cmd = STRUCTS.PMS_GET_MORNING_CALL_REQ
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
                pms_data_type = new _pms_data_type() { cmd = STRUCTS.PMS_CLEAR_MORNING_CALL_RES, status = 1, extension = _ext, hour = -1, minutes = -1, try_interval = 0, repeat_times = 0, repeat_week = 0, ring_duration = 0, week = string.Empty };
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
                cmd = STRUCTS.PMS_CLEAR_MORNING_CALL_REQ
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
            catch (SocketException sockex)
            {
                pms_data_type = new _pms_data_type() { cmd = STRUCTS.PMS_CLEAR_MORNING_CALL_RES, status = 1, extension = _ext, hour = -1, minutes = -1, try_interval = 0, repeat_times = 0, repeat_week = 0, ring_duration = 0, week = string.Empty };
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
                cmd = STRUCTS.PMS_CLEAR_MORNING_CALL_REQ
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
                pms_data_type = new _pms_data_type() { cmd = STRUCTS.PMS_CLEAR_MORNING_CALL_RES, status = 1 };
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

            //_cgi_extension_req cgi_ext_req = new _cgi_extension_req()
            //{
            //    cmd = SocketStruct.PMS_CLEAR_MORNING_CALL_REQ
            //    ,
            //    Ext = _ext
            //};

            _pms_data_type pms_data_type = new _pms_data_type()
            {
                cmd = STRUCTS.PMS_CLEAR_MORNING_CALL_REQ
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
        public _pms_data_type GetPolicy(string _ext)
        {
            IPEndPoint _serverEP = new IPEndPoint(IPAddress.Parse(PBXIP), PBXPORT);
            IPEndPoint _remoteEP = new IPEndPoint(IPAddress.Any, 0);
            UdpClient _client = new UdpClient();
            _client.Client.ReceiveTimeout = UDP_WAITING_MISEC;
            _client.Connect(_serverEP);

            _pms_data_type pms_data_type = new _pms_data_type()
            {
                cmd = STRUCTS.PMS_GET_ALL_REQ
                , extension = _ext
            };
            byte[] _sbuffer = util.GetBytes(pms_data_type);
            byte[] _rbuffer = null;

            try
            {
                _client.Send(_sbuffer, _sbuffer.Length);
                _rbuffer = _client.Receive(ref _remoteEP);

                pms_data_type = util.GetObject<_pms_data_type>(_rbuffer);
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

                pms_data_type = new _pms_data_type() { cmd = STRUCTS.PMS_GET_OUTGOING_POLICY_RES, status = states, extension = _ext, allowedPrefix = string.Empty, forbiddenPrefix = string.Empty };
            }

            _client.Close();

            return pms_data_type;
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

            _pms_data_type pms_data_type = new _pms_data_type()
            {
                cmd = STRUCTS.PMS_SET_ALL_REQ
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
                pms_data_type = new _pms_data_type() { cmd = STRUCTS.PMS_SET_ALL_RES, status = 1 };
            }

            _client.Close();

            bool _result = false;

            if (pms_data_type.cmd == STRUCTS.PMS_SET_ALL_RES)
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

                pms_data_type = util.GetObject<_pms_data_type>(_rbuffer);
            }
            catch (SocketException sockex)
            {
                util.WriteLog(sockex.Message);
                pms_data_type = new _pms_data_type() { cmd = STRUCTS.PMS_SET_ALL_RES, status = 1 };
            }

            _client.Close();

            bool _result = false;

            if (pms_data_type.cmd == STRUCTS.PMS_SET_ALL_RES)
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

        public bool CheckOut(string _ext)
        {
            IPEndPoint _serverEP = new IPEndPoint(IPAddress.Parse(PBXIP), PBXPORT);
            IPEndPoint _remoteEP = new IPEndPoint(IPAddress.Any, 0);
            UdpClient _client = new UdpClient();
            _client.Client.ReceiveTimeout = UDP_WAITING_MISEC;
            _client.Connect(_serverEP);

            //_pms_data_type pms_data_type = new _pms_data_type()
            //{
            //    cmd = STRUCTS.PMS_SET_ALL_REQ,
            //    extension = _ext,
            //    allowedPrefix = "",
            //    forbiddenPrefix = "all",
            //    language = 2,
            //    hour = -1,
            //    minutes = -1,
            //    repeat_times = 5,
            //    ring_duration = 180,
            //};

            _pms_data_type pms_data_type = new _pms_data_type()
            {
                cmd = STRUCTS.PMS_CLEAR_FUNCTION_KEY_REQ,
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
                pms_data_type = new _pms_data_type() { cmd = STRUCTS.PMS_CLEAR_FUNCTION_KEY_RES, status = 1 };
            }

            _client.Close();

            bool _result = false;

            if (pms_data_type.cmd == STRUCTS.PMS_CLEAR_FUNCTION_KEY_RES)
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

            _pms_data_type pms_data_type = new _pms_data_type()
            {
                cmd = STRUCTS.PMS_SET_POST_PARCEL_REQ,
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
                pms_data_type = new _pms_data_type() { cmd = STRUCTS.PMS_SET_ALL_RES, status = 1 };
            }

            _client.Close();

            bool _result = false;

            if (pms_data_type.cmd == STRUCTS.PMS_SET_POST_PARCEL_RES)
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

    }
}
