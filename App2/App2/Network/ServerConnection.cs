﻿using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NowMine.Network
{
    public class ServerConnection
    {
        public delegate void ServerConnectedEventHandler(object s, EventArgs e);
        public event ServerConnectedEventHandler ServerConnected;

        public delegate void UDPQueuedEventHandler(object s, PiecePosArgs e);
        public event UDPQueuedEventHandler UDPQueued;

        public delegate void DeletePieceEventHandler(object s, GenericEventArgs<int> e);
        public event DeletePieceEventHandler DeletePiece;

        public delegate void PlayedNowEventHandler(object s, GenericEventArgs<int> e);
        public event PlayedNowEventHandler PlayedNow;

        private string _serverAddress;
        public string serverAddress
        {
            get { return _serverAddress; }
            set { _serverAddress = value; }
        }

        private UDPConnector _udpConnector;
        public UDPConnector udpConnector
        {
            get
            {
                if (_udpConnector == null)
                {
                    _udpConnector = new UDPConnector();
                }
                return _udpConnector;
            }
        }

        private TCPConnector _tcpConnector;
        public TCPConnector tcpConnector
        {
            get
            {
                if (_tcpConnector == null)
                {
                    _tcpConnector = new TCPConnector();
                }
                return _tcpConnector;
            }
        }

        protected virtual void OnServerConnected()
        {
            ServerConnected?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnDeletepiece(int qPos)
        {
            DeletePiece?.Invoke(this, new GenericEventArgs<int>(qPos));
        }

        protected virtual void OnPlayedNow(int qPos)
        {
            PlayedNow?.Invoke(this, new GenericEventArgs<int>(qPos));
        }

        protected virtual void OnUDPQueued(YoutubeQueued piece)
        {
            MusicPiece mPiece = new MusicPiece(piece);
            UDPQueued?.Invoke(this, new PiecePosArgs(mPiece, piece.qPos));
        }

        internal async Task<IList<User>> getUsers()
        {
            try
            {
                byte[] bQueue = await tcpConnector.getData("GetUsers", serverAddress);
                using (MemoryStream ms = new MemoryStream(bQueue))
                using (BsonReader reader = new BsonReader(ms))
                {
                    reader.ReadRootValueAsArray = true;
                    JsonSerializer serializer = new JsonSerializer();
                    IList<User> users = serializer.Deserialize<IList<User>>(reader);
                    Debug.WriteLine("Got User list with {0} items", users.Count);
                    return users;
                }
                //await tcpConnector.receiveTCP();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Data: {0}; message: {1}", e.Data, e.Message);
                return null;
            }
        }

        public bool findServer()
        {
            tcpConnector.MessegeReceived += OnServerFound;
            Device.StartTimer(TimeSpan.FromSeconds(3), () =>
           {
               Task.Factory.StartNew(async () =>
               {
                   await udpConnector.sendBroadcastUdp("NowMine!");
                   await tcpConnector.receiveTCP();
                   if (string.IsNullOrEmpty(serverAddress))
                   {
                       return true;
                   }
                       return false;
               });
               return false;
            });
            return false;

        }

        internal void startListeningUDP()
        {
            udpConnector.MessegeReceived += UDPMessageReceived;
            udpConnector.receiveBroadcastUDP();
        }

        private void UDPMessageReceived(object source, MessegeEventArgs args)
        {
            byte[] bytes = args.Messege;
            string msg = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            Debug.WriteLine("UDP Received: {0}", msg);
            string command = msg.Substring(0, msg.IndexOf(':'));
            int startIndex = Encoding.UTF8.GetBytes(command + ": ").Length;
            switch (command)
            {   
                case "Queue":
                    using (MemoryStream ms = new MemoryStream(bytes, startIndex, bytes.Length - startIndex))
                    using (BsonReader reader = new BsonReader(ms))
                    {
                        //reader.ReadRootValueAsArray = true;
                        JsonSerializer serializer = new JsonSerializer();
                        YoutubeQueued yt = serializer.Deserialize<YoutubeQueued>(reader);
                        if (yt.userId == User.DeviceUser.Id)
                        {
                            return;
                        }
                        Debug.WriteLine("UDP/ Adding to Queue {0}", yt.title);
                        OnUDPQueued(yt); 
                    }
                    break;

                case "Delete":
                    int qPosDelete = int.Parse(msg.Substring(msg.IndexOf(':') + 1));
                    //to int
                    OnDeletepiece(qPosDelete);
                    break;

                case "PlayedNow":
                    int qPosPlayedNow = BitConverter.ToInt32(bytes, startIndex);
                    //int qPosPlayedNow = int.Parse(msg.Substring(msg.IndexOf(':') + 1));
                    OnPlayedNow(qPosPlayedNow);
                    break;

                default:
                    Debug.WriteLine("UDP/ Cannot interpret right...");
                    break;
            }
                
        }

        internal async Task<int> SendToQueue(YoutubeInfo info)
        {
            using (var ms = new MemoryStream())
            using (var writer = new BsonWriter(ms))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(writer, info, typeof(YoutubeInfo));
                Debug.WriteLine("Sending to Queue: {0}", Convert.ToBase64String(ms.ToArray()));
                byte[] response = await tcpConnector.SendData(ms.ToArray(), serverAddress);

                return BitConverter.ToInt32(response, 0);
            }
        }

        public async Task<IList<YoutubeInfo>> getQueueTCP()
        {
            //tcpConnector.MessegeReceived += OnQueueReceived;
            try
            {
                byte[] bQueue = await tcpConnector.getData("GetQueue", serverAddress);
                using (MemoryStream ms = new MemoryStream(bQueue))
                using (BsonReader reader = new BsonReader(ms))
                {
                    reader.ReadRootValueAsArray = true;
                    JsonSerializer serializer = new JsonSerializer();
                    IList<YoutubeInfo> ytInfos = serializer.Deserialize<IList<YoutubeInfo>>(reader);
                    Debug.WriteLine("Got Queue with {0} items", ytInfos.Count);
                    return ytInfos;
                }
                //await tcpConnector.receiveTCP();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Data: {0}; message: {1}", e.Data, e.Message);
                return null;
            }
        }

        private void OnServerFound(object source, MessegeEventArgs args)
        {
            string messege = System.Text.Encoding.UTF8.GetString(args.Messege, 0, args.Messege.Length);
            var ipAddressBuilder = new StringBuilder();
            for (int i = 0; i < 4; i++)
            {
                ipAddressBuilder.Append(args.Messege[i]);
                if (i != 3)
                    ipAddressBuilder.Append('.');
            }
            serverAddress = ipAddressBuilder.ToString();
            //tutaj sprawdzanie czy to ip itd
            int userID = BitConverter.ToInt32(args.Messege, 4);
            User.InitializeDeviceUser(userID);
            OnServerConnected();
            tcpConnector.MessegeReceived -= OnServerFound;
        }

        public bool isWifi()
        {
            Debug.WriteLine("NET: Getting Wifi status");
            var networkConnection = DependencyService.Get<INetworkConnection>();
            networkConnection.CheckNetworkConnection();
            bool status = networkConnection.IsConnected;
            Debug.WriteLine("NET: Wifi status is {0}", status);
            return status;
        }
    }
}
