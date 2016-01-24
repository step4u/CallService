using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Alchemy;
using Alchemy.Classes;

namespace Com.Huen.Sockets
{
    public class AlchemyProperty
    {
        public int Port = 81;
        public int HeartBeat = 25;
        public string HeartbeatPackage = "7";
        public string Server = "";
        public string Action = "";
    }

    public class AlchemyClientCS : IDisposable
    {
        public delegate void OnConnectEventHandler(Object sender, UserContext context);
        public delegate void OnConnectedEventHandler(Object sender, UserContext context);
        public delegate void OnSendEventHandler(Object sender, UserContext context);
        public delegate void OnReceiveEventHandler(Object sender, UserContext context);
        public delegate void OnDisconnectEventHandler(Object sender, UserContext context);
        public delegate void OnHeartBeatTickEventHandler(Object sender);

        public event OnConnectEventHandler OnConnectEvt;
        public event OnConnectedEventHandler OnConnectedEvt;
        public event OnSendEventHandler OnSendEvt;
        public event OnReceiveEventHandler OnReceiveEvt;
        public event OnDisconnectEventHandler OnDisconnectEvt;
        public event OnHeartBeatTickEventHandler OnHeartBeatTickEvt;

        private System.Timers.Timer _timer;
        public string Server = string.Empty;
        public string Port = string.Empty;
        public double HeartBeat = 25000.0d;

        public WebSocketClient client;
        public AlchemyClientCS()
        {
            string _address = string.Empty;
            if (string.IsNullOrEmpty(Server) || string.IsNullOrEmpty(Port))
            {
                _address = "ws://127.0.0.1:81/";
            }
            else
            {
                _address = string.Format("ws://{0}:{1}/", Server, Port);
            }

            //client = new WebSocketClient(_address);
            //client.OnConnect = new OnEventDelegate(OnConnect);
            //client.OnConnected = new OnEventDelegate(OnConnected);
            //client.OnSend = new OnEventDelegate(OnSend);
            //client.OnReceive = new OnEventDelegate(OnReceive);
            //client.OnDisconnect = new OnEventDelegate(OnDisconnect);

            client = new WebSocketClient(_address)
            {
                OnConnect = OnConnect,
                OnConnected = OnConnected,
                OnSend = OnSend,
                OnReceive = OnReceive,
                OnDisconnect = OnDisconnect
            };

            //client.Connect();
        }

        void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (OnHeartBeatTickEvt != null)
                OnHeartBeatTickEvt(this);
        }

        private void OnConnect(UserContext context)
        {
            if (OnConnectEvt != null)
                OnConnectEvt(this, context);
        }

        private void OnConnected(UserContext context)
        {
            if (OnConnectedEvt != null)
                OnConnectedEvt(this, context);

            _timer = new System.Timers.Timer();
            _timer.AutoReset = true;
            _timer.Interval = HeartBeat;
            _timer.Elapsed += _timer_Elapsed;
            _timer.Enabled = true;
            _timer.Start();
        }

        private void OnSend(UserContext context)
        {
            if (OnSendEvt != null)
                OnSendEvt(this, context);
        }

        private void OnReceive(UserContext context)
        {
            if (OnReceiveEvt != null)
                OnReceiveEvt(this, context);
        }

        private void OnDisconnect(UserContext context)
        {
            if (OnDisconnectEvt != null)
                OnDisconnectEvt(this, context);
        }

        public void Connect()
        {
            if (client != null)
                client.Connect();
        }

        public void Send(string data)
        {
            if (client.ReadyState == WebSocketClient.ReadyStates.OPEN)
                client.Send(data);
        }

        public void Disconnect()
        {
            if (client.ReadyState == WebSocketClient.ReadyStates.OPEN)
                client.Disconnect();
        }

        public void Dispose()
        {
            if (client != null)
            {
                this.Disconnect();
                client = null;
            }

            if (_timer != null)
            {
                _timer.Stop();
                _timer.Enabled = false;
                _timer.Close();
                _timer.Dispose();
                _timer = null;
            }
        }
    }
}
