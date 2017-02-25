using Sockets.Plugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace NowMine.Network
{
    public class MessegeEventArgs : EventArgs
    {
        public string messege { get; set; }
    }

    public class TCPConnector
    {
        public delegate void MessegeReceivedEventHandler(object source, MessegeEventArgs args);
        public event MessegeReceivedEventHandler MessegeReceived;

        private TcpSocketClient _tcpClient;
        public TcpSocketClient tcpClient
        {
            get
            {
                if (_tcpClient == null)
                    _tcpClient = new TcpSocketClient();
                return _tcpClient;
            }
        }

        private TcpSocketListener _tcpListener;
        public TcpSocketListener tcpListener
        {
            get
            {
                if (_tcpListener == null)
                {
                    _tcpListener = new TcpSocketListener();
                    _tcpListener.ConnectionReceived += ConnectionReceived;
                }
                return _tcpListener;
            }
        }

        private async void ConnectionReceived(object sender, Sockets.Plugin.Abstractions.TcpSocketListenerConnectEventArgs e)
        {
            var client = e.SocketClient;
            var bytesRead = -1;
            var buf = new byte[1];
            string message = "";
            while (bytesRead != 0)
            {
                bytesRead = await e.SocketClient.ReadStream.ReadAsync(buf, 0, 1);
                if (bytesRead > 0)
                {
                    Debug.WriteLine(buf[0]);
                    message += System.Text.Encoding.UTF8.GetString(buf, 0, 1);
                }
            }
            Debug.WriteLine("TCP: RECEIVED From: {0}:{1} - {2}", e.SocketClient.RemoteAddress, e.SocketClient.RemotePort, message);
            OnMessegeReceived(message);
            await _tcpListener.StopListeningAsync();
        }

        protected virtual void OnMessegeReceived(string messege)
        {
            MessegeReceived?.Invoke(this, new MessegeEventArgs() { messege = messege });
        }

        public async Task receiveTCP()
        {
            Debug.WriteLine("TCP: Starting Listening!");
            await tcpListener.StartListeningAsync(4444);
        }

        public async Task<byte[]> getBSON(string messege, string serverAddress)
        {
            Debug.WriteLine("Sending {0} to {1}!", messege, serverAddress);
            await tcpClient.ConnectAsync(serverAddress, 4444);
            byte[] data = Encoding.UTF8.GetBytes(messege + "\n");
            await tcpClient.WriteStream.WriteAsync(data, 0, data.Length);
            int readByte = 0;
            List<byte> queue = new List<byte>();
            while (readByte != -1)
            {
                readByte = tcpClient.ReadStream.ReadByte();
                Debug.WriteLine("TCP/ Rec: {0}", readByte);
                queue.Add((byte)readByte);
            }
            return queue.ToArray();
        }

        public async Task stopReceiving()
        {
            Debug.WriteLine("TCP: Stopping Listening!");
            await tcpListener.StopListeningAsync();
        }
    }
}
