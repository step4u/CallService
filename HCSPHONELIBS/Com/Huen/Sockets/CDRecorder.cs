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
    public delegate void RequestCleanEventHandler(CDRecorder sender, int cmd, string caller, string callee);
    public delegate void ResultCleanEventHandler(CDRecorder sender, int cmd, string caller, string callee);
    public delegate void RequestCDREventHandler(object sender, int cmd, string caller, string callee, int result);

    public class CDRecorder : IDisposable
    {
        public event RequestCleanEventHandler ReqCleanEvent;
        public event ResultCleanEventHandler ResCleanEvent;
        public event RequestCDREventHandler RequestCDREvent;


        private ModifyRegistry _reg;
        //private BackgroundWorker _bw;

        private Socket _sockCdrSrv = null;
        private EndPoint _localep;
        private EndPoint _remoteep;
        private Thread _threadCdrSrv;
        private bool _IsCdrSrvStarted = false;

        public CDRecorder() : this (21003)
        {
        }

        public CDRecorder(int port)
        {
            _localep = new IPEndPoint(IPAddress.Any, port);
            _remoteep = new IPEndPoint(IPAddress.Any, 0);

            try
            {
                _sockCdrSrv = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                _sockCdrSrv.Bind(_localep);
            }
            catch(SocketException se)
            {
                string __msg = string.Format("CdrSrv socket start error: {0}", se.ErrorCode);
                util.WriteLog(__msg);
            }

            this.StartCdrSockThread();
        }

        public void Dispose()
        {
            _IsCdrSrvStarted = false;
            _sockCdrSrv.Close(); _sockCdrSrv = null;
        }

        public void InitializeProperties()
        {
            _reg = new ModifyRegistry(util.LoadProjectResource("REG_SUBKEY_CDR", "COMMONRES", "").ToString());
        }

        private void StartCdrSockThread()
        {
            _IsCdrSrvStarted = true;
            _threadCdrSrv = new Thread(new ThreadStart(CdrSrvReceiver));
            _threadCdrSrv.IsBackground = true;
            _threadCdrSrv.Start();
        }

        private void CdrSrvReceiver()
        {
            int _count = 0;

            while (_IsCdrSrvStarted)
            {
                byte[] _buffer = new byte[1024];
                _count = 0;

                try
                {
                    _count = _sockCdrSrv.ReceiveFrom(_buffer, SocketFlags.None, ref _remoteep);
                }
                catch (SocketException se)
                {
                    string _msg = string.Format("CdrSrv socket receive error: {0}", se.ErrorCode);
                    util.WriteLog(_msg);
                }

                if (_count == 0) return;

                byte[] _databuffer = new byte[_count];
                Buffer.BlockCopy(_buffer, 0, _databuffer, 0, _count);

                CdrRequest_t _cdr = util.GetObject<CdrRequest_t>(_databuffer);
                CdrList _cdrdata = util.GetObject<CdrList>(_cdr.data);
                this.CdrToDB(_cdr, _cdrdata);

                //if ((_cdrdata.callee == "0001"
                //    || _cdrdata.callee == "0003"
                //    || _cdrdata.callee == "0004"
                //    || _cdrdata.callee == "0005"
                //    || _cdrdata.callee == "0006"
                //    || _cdrdata.callee == "0007"
                //    || _cdrdata.callee == "0008")
                //    && _cdrdata.result == 0)
                //{
                //    if (RequestCDREvent != null)
                //        RequestCDREvent(this, _cdr.cmd, _cdrdata.caller, _cdrdata.callee, _cdrdata.result);
                //}

                //if (RequestCDREvent != null)
                //    RequestCDREvent(this, _cdr.cmd, _cdrdata.caller, _cdrdata.callee, _cdrdata.result);
            }
        }

        private void CdrToDB(CdrRequest_t _cdr, CdrList _cdrdata)
        {
            CDRData _cdrd = new CDRData() {
                OFFICE_NAME = _cdrdata.office_name
                ,
                STARTDATE = new DateTime(_cdrdata.start_yyyy, _cdrdata.start_month, _cdrdata.start_day, _cdrdata.start_hour, _cdrdata.start_min, _cdrdata.start_sec)
                ,
                ENDDATE = new DateTime(_cdrdata.end_yyyy, _cdrdata.end_month, _cdrdata.end_day, _cdrdata.end_hour, _cdrdata.end_min, _cdrdata.end_sec)
                ,
                CALLER = _cdrdata.caller
                ,
                CALLER_TYPE = _cdrdata.caller_type
                ,
                CALLER_IPN_NUMBER = _cdrdata.caller_ipn_number
                ,
                CALLER_GROUP_CODE = _cdrdata.caller_group_code
                ,
                CALLER_GROUP_NAME = _cdrdata.caller_group_name
                ,
                CALLER_HUMAN_NAME = _cdrdata.caller_human_name
                ,
                CALLEE = _cdrdata.callee
                ,
                CALLEE_TYPE = _cdrdata.callee_type
                ,
                CALLEE_IPN_NUMBER = _cdrdata.callee_ipn_number
                ,
                CALLEE_GROUP_CODE = _cdrdata.callee_group_code
                ,
                CALLEE_GROUP_NAME = _cdrdata.callee_group_name
                ,
                CALLEE_HUMAN_NAME = _cdrdata.callee_human_name
                ,
                RESULT = _cdrdata.result
                ,
                SEQ = _cdrdata.seq
            };

            StringBuilder __slqsb = new StringBuilder();

            __slqsb.Append(" insert into CDRINFO ");
            __slqsb.Append(" ( ");
            __slqsb.Append(" OFFICE_NAME ");
            __slqsb.Append(" , STARTDATE ");
            __slqsb.Append(" , ENDDATE ");
            __slqsb.Append(" , CALLER ");
            __slqsb.Append(" , CALLER_TYPE ");
            __slqsb.Append(" , CALLER_IPN_NUMBER ");
            __slqsb.Append(" , CALLER_GROUP_CODE ");
            __slqsb.Append(" , CALLER_GROUP_NAME ");
            __slqsb.Append(" , CALLER_HUMAN_NAME ");
            __slqsb.Append(" , CALLEE ");
            __slqsb.Append(" , CALLEE_TYPE ");
            __slqsb.Append(" , CALLEE_IPN_NUMBER ");
            __slqsb.Append(" , CALLEE_GROUP_CODE ");
            __slqsb.Append(" , CALLEE_GROUP_NAME ");
            __slqsb.Append(" , CALLEE_HUMAN_NAME ");
            __slqsb.Append(" , RESULT ");
            __slqsb.Append(" , SEQ ");
            __slqsb.Append(" ) values ( ");
            __slqsb.AppendFormat(" '{0}' ", _cdrd.OFFICE_NAME);
            __slqsb.AppendFormat(" , '{0}' ", _cdrd.STARTDATE.ToString("yyyy-MM-dd HH:mm:ss"));
            __slqsb.AppendFormat(" , '{0}' ", _cdrd.ENDDATE.ToString("yyyy-MM-dd HH:mm:ss"));
            __slqsb.AppendFormat(" , '{0}' ", _cdrd.CALLER);
            __slqsb.AppendFormat(" , '{0}' ", _cdrd.CALLER_TYPE);
            __slqsb.AppendFormat(" , '{0}' ", _cdrd.CALLER_IPN_NUMBER);
            __slqsb.AppendFormat(" , '{0}' ", _cdrd.CALLER_GROUP_CODE);
            __slqsb.AppendFormat(" , '{0}' ", _cdrd.CALLER_GROUP_NAME);
            __slqsb.AppendFormat(" , '{0}' ", _cdrd.CALLER_HUMAN_NAME);
            __slqsb.AppendFormat(" , '{0}' ", _cdrd.CALLEE);
            __slqsb.AppendFormat(" , '{0}' ", _cdrd.CALLEE_TYPE);
            __slqsb.AppendFormat(" , '{0}' ", _cdrd.CALLEE_IPN_NUMBER);
            __slqsb.AppendFormat(" , '{0}' ", _cdrd.CALLEE_GROUP_CODE);
            __slqsb.AppendFormat(" , '{0}' ", _cdrd.CALLEE_GROUP_NAME);
            __slqsb.AppendFormat(" , '{0}' ", _cdrd.CALLEE_HUMAN_NAME);
            __slqsb.AppendFormat(" , '{0}' ", _cdrd.RESULT);
            __slqsb.AppendFormat(" , '{0}' ", _cdrd.SEQ);
            __slqsb.Append(" ); ");

            using (FirebirdDBHelper db = new FirebirdDBHelper(__slqsb.ToString(), util.strDBConn))
            {
                try
                {
                    db.BeginTran();
                    int __count = db.GetEffectedCount();
                    db.Commit();

                    CdrResponse_t __cdrRes = this.GetCdrRtnMsg(_cdr);
                    byte[] __buffer = util.GetBytes(__cdrRes);

                    try
                    {
                        _sockCdrSrv.SendTo(__buffer, SocketFlags.None, _remoteep);
                    }
                    catch (SocketException __se)
                    {
                        util.WriteLog(string.Format("Socket send message error : {0}", __se.Message));
                    }
                }
                catch (FirebirdSql.Data.FirebirdClient.FbException __fex)
                {
                    db.Rollback();
                    util.WriteLog(string.Format("{0} : {1}\r\nMessage : {2}", "Rollback Exception Type", __fex.GetType(), __fex.Message));
                }
                catch(SocketException __se)
                {
                    util.WriteLog(string.Format("Socket send message error : {0}", __se.Message));
                }
            }
        }

        private CdrResponse_t GetCdrRtnMsg(CdrRequest_t _msg)
        {
            CdrResponse_t __cdrRes = new CdrResponse_t();
            __cdrRes.cmd = 2;
            __cdrRes.pCdr = _msg.pCdr;
            __cdrRes.status = 0;

            return __cdrRes;
        }
    }
}
