using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Sockets.Plugin;
using System.Diagnostics;

namespace App2
{
    public class ConnectedToServerEventArgs : EventArgs
    {
        public string ServerAddress = "";
    }

    public class Network
    {
        public string nowmineAddress = "";
        private bool isTCPListening = false;

        public delegate void ConnectedToServerEventHandler(object source, ConnectedToServerEventArgs args);

        public event ConnectedToServerEventHandler ConnectedToServer;

        private string _broadcastAddress;
        public string broadcastAddress
        {
            get
            {   
                if (_broadcastAddress == null)
                {
                    var networkConnection = DependencyService.Get<INetworkConnection>();
                    _broadcastAddress = networkConnection.GetBroadcastAddress();
                }
                return _broadcastAddress;
            }
        }

        private UdpSocketReceiver _udpReceiver;
        public UdpSocketReceiver udpReceiver
        {
            get
            {
                if (_udpReceiver == null)
                {
                    _udpReceiver = new UdpSocketReceiver();
                    _udpReceiver.MessageReceived += async (sender, args) => 
                    {
                        var from = String.Format("{0}:{1}", args.RemoteAddress, args.RemotePort);
                        var data = Encoding.UTF8.GetString(args.ByteData, 0, args.ByteData.Length);
                        Debug.WriteLine("UDP: RECEIVED {0} - {1}", from, data);
                        nowmineAddress = args.RemoteAddress;
                        await _udpReceiver.StopListeningAsync();
                        Debug.WriteLine("UDP: Stopped Listening (anon)");
                        isTCPListening = false;
                        //tsc.SetResult(args.ByteData);
                        onConnectedToServer();
                    };
                }
                return _udpReceiver;
            }
        }

        private UdpSocketClient _udpClient;
        public UdpSocketClient udpClient
        {
            get
            {
                using (_udpClient = new UdpSocketClient())
                {
                    return _udpClient;
                }
            }
        }

        private TcpSocketListener _tcpClient;
        public TcpSocketListener tcpClient
        {
            get
            {
                if (_tcpClient == null)
                {
                    _tcpClient = new TcpSocketListener();
                }
                return _tcpClient;
            }
        }

        protected virtual void onConnectedToServer()
        {
            if (ConnectedToServer != null)
                ConnectedToServer(this, new ConnectedToServerEventArgs() { ServerAddress = nowmineAddress});
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

        public async Task receiveUdp(int port = 1234)
        {
            if (isTCPListening == false)
            {
                Debug.WriteLine("UDP: Starting Listening");
                isTCPListening = true;
                await udpReceiver.StartListeningAsync(port);
            }
        }

        public async Task sendBroadcastUdp(string message, int port = 1234)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            //await udpClient.SendToAsync(data, broadcastAddress, port);
            string address = "255.255.255.255";
            try
            {
                //await udpClient.SendToAsync(data, address, port);
                var uc = new UdpSocketClient();
                await uc.SendToAsync(data, address, port);
                uc.Dispose();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            
            Debug.WriteLine("UDP: Sent {0} to {1}:{2}", message, address, port);
        }

        public async Task findServer(ServerCheckPage serverCheck)
        {
            if (isTCPListening)
            {
                //await udpReceiver.StopListeningAsync();
                await tcpClient.StopListeningAsync();
                isTCPListening = false;
            }
            await sendBroadcastUdp("NowMine!");
            //await receiveUdp();
            string serverAddress = await receiveTCP();
            if (!string.IsNullOrEmpty(serverAddress))
            {
                ServerConnection.Address = serverAddress;
                serverCheck.loadNext();
            }
        }

        private async Task<string> receiveTCP()
        {
            
            isTCPListening = true;
            string message = "";
            tcpClient.ConnectionReceived += async (sender, args) =>
            {
                var client = args.SocketClient;
                var bytesRead = -1;
                var buf = new byte[1];

                while (bytesRead != 0)
                {
                    bytesRead = await args.SocketClient.ReadStream.ReadAsync(buf, 0, 1);
                    if (bytesRead > 0)
                    {
                        Debug.WriteLine(buf[0]);
                        message += System.Text.Encoding.UTF8.GetString(buf, 0, 1);
                    }
                }
                Debug.WriteLine("TCP: RECEIVED From: {0}:{1} - {2}", args.SocketClient.RemoteAddress, args.SocketClient.RemotePort, message);
                await _tcpClient.StopListeningAsync();
                //_tcpClient.Dispose();
                isTCPListening = false;
            };
            await tcpClient.StartListeningAsync(4444);
            while (string.IsNullOrEmpty(message))
            {

            }
            return message;
        }
    }
}
