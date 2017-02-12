using Sockets.Plugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App2.Network
{
    public class MessegeEventArgs : EventArgs
    {
        public string messege { get; set; }
    }

    public class TCPConnector
    {
        public delegate void MessegeReceivedEventHandler(object source, MessegeEventArgs args);
        public event MessegeReceivedEventHandler MessegeReceived;
        private TcpSocketListener _tcpClient;
        public TcpSocketListener tcpClient
        {
            get
            {
                if (_tcpClient == null)
                {
                    _tcpClient = new TcpSocketListener();
                    _tcpClient.ConnectionReceived += ConnectionReceived;
                }
                return _tcpClient;
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
            await _tcpClient.StopListeningAsync();
        }

        protected virtual void OnMessegeReceived(string messege)
        {
            MessegeReceived?.Invoke(this, new MessegeEventArgs() { messege = messege });
        }

        public async Task receiveTCP()
        {
            Debug.WriteLine("TCP: Starting Listening!");
            await tcpClient.StartListeningAsync(4444);
        }

        public async Task stopReceiving()
        {
            Debug.WriteLine("TCP: Stopping Listening!");
            await tcpClient.StopListeningAsync();
        }
    }
}
