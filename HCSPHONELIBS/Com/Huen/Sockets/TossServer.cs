using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Net;
using System.Collections.Concurrent;

using Alchemy;
using Alchemy.Classes;
using Newtonsoft.Json;
using Com.Huen.Views;
using System.Diagnostics;

namespace Com.Huen.Sockets
{
    public class TossServer : IDisposable
    {
        private PMSServer pmsSrv;
        private CDRecorder cdrSrv;

        private WebSocketServer aServer;
        protected ConcurrentDictionary<WebSocketUser, string> OnlineUsers = new ConcurrentDictionary<WebSocketUser, string>();

        public TossServer()
        {
            aServer = new WebSocketServer(false, 81, IPAddress.Any)
            {
                OnReceive = OnReceive,
                OnSend = OnSend,
                OnConnect = OnConnect,
                OnDisconnect = OnDisconnect,
                TimeOut = new TimeSpan(0, 5, 0)
            };

            aServer.Start();

            pmsSrv = new PMSServer();
            pmsSrv.ReqPMSSetEvent += pmsSrv_ReqPMSSetEvent;

            cdrSrv = new CDRecorder();
            cdrSrv.RequestCDREvent += cdrSrv_RequestCDREvent;
        }

        void cdrSrv_RequestCDREvent(object sender, int cmd, string caller, string callee, int result)
        {
            Clean c = new Clean() { caller = caller, callee = callee, result = result };

            var u = OnlineUsers.Keys.FirstOrDefault();

            ResponseFromTossServer r = null;

            Debug.WriteLine("ToossServer callee: " + callee);

            switch (callee)
            {
                case "0001":
                    // Make up room request
                    r = new ResponseFromTossServer() { Type = CommandType.MakeupRoomReq, Data = c };
                    break;
                case "0002":
                    // Make up room done
                    r = new ResponseFromTossServer() { Type = CommandType.MakeupRoomDone, Data = c };
                    break;
                case "0003":
                    // DnD 요청
                    r = new ResponseFromTossServer() { Type = CommandType.DnDReq, Data = c };
                    break;
                case "0004":
                    // DnD 취소
                    r = new ResponseFromTossServer() { Type = CommandType.DnDCancel, Data = c };
                    break;
                case "0005":
                    // 세탁 요청
                    r = new ResponseFromTossServer() { Type = CommandType.LaundaryReq, Data = c };
                    break;
                case "0006":
                    // 세탁 취소
                    r = new ResponseFromTossServer() { Type = CommandType.LaundaryCancel, Data = c };
                    break;
                case "0007":
                    // make up room confirm
                    r = new ResponseFromTossServer() { Type = CommandType.MakeupRoomConfirm, Data = c };
                    break;
                case "0008":
                    // 방청소 확인
                    r = new ResponseFromTossServer() { Type = CommandType.MakeupRoomConfirm, Data = c };
                    break;
                default:
                    switch (caller)
                    {
                        case "morning":
                            r = new ResponseFromTossServer() { Type = CommandType.MorningCall, Data = c };
                            break;
                        default:
                            return;
                    }
                    break;
            }

            if (u != null)
                u.Context.Send(JsonConvert.SerializeObject(r));
        }

        void pmsSrv_ReqPMSSetEvent(PMSServer sender, _pms_data_type pmsdata)
        {
            //var user = OnlineUsers.Keys.Where(o => o.Context.ClientAddress == context.ClientAddress).Single();
            var u = OnlineUsers.Keys.FirstOrDefault();
            ResponseFromTossServer r = new ResponseFromTossServer() { Type = CommandType.Message, Data = pmsdata };
            if (u != null)
                u.Context.Send(JsonConvert.SerializeObject(r));
        }

        public void OnConnect(UserContext context)
        {
            //var json = context.DataFrame.ToString();
            //Connect obj = JsonConvert.DeserializeObject<Connect>(json);

            var me = new WebSocketUser { Context = context };
            OnlineUsers.TryAdd(me, String.Empty);

            //Response r = new Response() { Type = ResponseType.Connection };
            //context.Send(JsonConvert.SerializeObject(r));
        }

        /// <summary>
        /// Event fired when a data is received from the Alchemy Websockets server instance.
        /// Parses data as JSON and calls the appropriate message or sends an error message.
        /// </summary>
        /// <param name="context">The user's connection context</param>
        public void OnReceive(UserContext context)
        {
            try
            {
                var json = context.DataFrame.ToString();
                RequestToTossServer obj = JsonConvert.DeserializeObject<RequestToTossServer>(json);
                ResponseFromTossServer r;

                switch (obj.Type)
                {
                    case CommandType.RegisterReq:
                        Register(obj.userIdentity, context);
                        break;
                    case CommandType.HeartBeatReq:
                        var user = OnlineUsers.Keys.Where(o => o.Context.ClientAddress == context.ClientAddress).Single();
                        r = new ResponseFromTossServer() { Type = CommandType.HeartBeatRes };
                        user.Context.Send(JsonConvert.SerializeObject(r));
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e) // Bad JSON! For shame.
            {
                var r = new ResponseFromTossServer { Type = CommandType.Error, Data = new { e.Message } };
                context.Send(JsonConvert.SerializeObject(r));
            }
        }

        /// <summary>
        /// Event fired when the Alchemy Websockets server instance sends data to a client.
        /// Logs the data to the console and performs no further action.
        /// </summary>
        /// <param name="context">The user's connection context</param>
        public void OnSend(UserContext context)
        {
            
        }

        /// <summary>
        /// Event fired when a client disconnects from the Alchemy Websockets server instance.
        /// Removes the user from the online users list and broadcasts the disconnection message
        /// to all connected users.
        /// </summary>
        /// <param name="context">The user's connection context</param>
        public void OnDisconnect(UserContext context)
        {
            var user = OnlineUsers.Keys.Where(o => o.Context.ClientAddress == context.ClientAddress).Single();
            string trash; // Concurrent dictionaries make things weird
            OnlineUsers.TryRemove(user, out trash);
        }

        /// <summary>
        /// Register a user's context for the first time with a username, and add it to the list of online users
        /// </summary>
        /// <param name="name">The name to register the user under</param>
        /// <param name="context">The user's connection context</param>
        private void Register(string identify, UserContext context)
        {
            var json = context.DataFrame.ToString();
            RequestToTossServer obj = JsonConvert.DeserializeObject<RequestToTossServer>(json);

            var u = OnlineUsers.Keys.Where(o => o.Context.ClientAddress == context.ClientAddress).Single();
            u.Identity = obj.userIdentity;
            //OnlineUsers[u] = obj.userIdentity;

            ResponseFromTossServer r = new ResponseFromTossServer() { Type = CommandType.RegisterRes };
            var sjson = JsonConvert.SerializeObject(r);
            context.Send(sjson);
        }

        /// <summary>
        /// Send a command to a pbx
        /// </summary>
        /// <param name="user">connected user</param>
        /// <param name="context">The user's connection context</param>
        private static void Send2Devices(String userIdentity, UserContext context)
        {
            
        }

        /// <summary>
        /// Broadcasts an error message to the client who caused the error
        /// </summary>
        /// <param name="errorMessage">Details of the error</param>
        /// <param name="context">The user's connection context</param>
        private static void SendError(string errorMessage, UserContext context)
        {
            var r = new ResponseFromTossServer { Type = CommandType.Error, Data = new { Message = errorMessage } };

            context.Send(JsonConvert.SerializeObject(r));
        }

        /// <summary>
        /// SendResponse a message to users
        /// </summary>
        /// <param name="message">Message to be SendResponse</param>
        /// <param name="users">Optional list of users to broadcast to. If null, broadcasts to all. Defaults to null.</param>
        private void SendResponse(string message, ICollection<WebSocketUser> users)
        {
            foreach (var u in OnlineUsers.Keys.Where(users.Contains))
            {
                u.Context.Send(message);
            }
        }

        public void Dispose()
        {
            if (aServer != null)
            {
                aServer.Stop();
                //aServer.Dispose();
            }
        }
    }

    /// <summary>
    /// Holds the name and context instance for an online user
    /// </summary>
    public class WebSocketUser
    {
        public string Identity = String.Empty;
        public UserContext Context { get; set; }
    }

    /// <summary>
    /// Defines a command that the client sends to the server
    /// </summary>
    public enum CommandType
    {
        Message = 1,
        RegisterReq = 2,
        RegisterRes = 3,
        MakeupRoomReq = 4,
        MakeupRoomCancel = 5,
        MakeupRoomDone = 6,
        MakeupRoomConfirm = 7,
        LaundaryReq = 8,
        LaundaryCancel = 9,
        LaundaryDone = 10,
        ParcelExistReq = 11,
        ParcelExistEnd = 12,
        HeartBeatReq = 13,
        HeartBeatRes = 14,
        MorningCall = 15,
        DnDReq = 16,
        DnDCancel = 17,
        Error = 255
    }

    public class RequestToTossServer
    {
        public CommandType Type { get; set; }
        public String userIdentity { get; set; }
        public _pms_data_type pbx_struct { get; set; }
    }

    /// <summary>
    /// Defines the response object to send back to the client
    /// </summary>
    public class ResponseFromTossServer
    {
        public CommandType Type { get; set; }
        //public dynamic Data { get; set; }
        public object Data { get; set; }
    }

    /// <summary>
    /// Defines the response object to send back to the client
    /// </summary>
    public class Clean
    {
        public string caller { get; set; }
        public string callee { get; set; }
        public int result { get; set; }
    }
}
