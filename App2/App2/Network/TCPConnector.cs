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
        public byte[] Messege { get; set; }
    }

    public class TCPConnector
    {
        public delegate void MessegeTCPventHandler(object source, MessegeEventArgs args);
        public event MessegeTCPventHandler MessegeReceived;

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
                }
                return _tcpListener;
            }
        }

        private bool isSending = false;

        private async void ConnectionReceived(object sender, Sockets.Plugin.Abstractions.TcpSocketListenerConnectEventArgs e)
        {
            var client = e.SocketClient;
            var bytesRead = -1;
            var buf = new byte[1];
            string message = "";
            List<byte> bytes = new List<byte>();
            while (bytesRead != 0)
            {
                bytesRead = await e.SocketClient.ReadStream.ReadAsync(buf, 0, 1);
                if (bytesRead > 0)
                {
                    //Debug.WriteLine(buf[0]);
                    message += System.Text.Encoding.UTF8.GetString(buf, 0, 1);
                    bytes.Add(buf[0]);
                }
            }
            Debug.WriteLine("TCP: RECEIVED From: {0}:{1} - {2}", e.SocketClient.RemoteAddress, e.SocketClient.RemotePort, message);
            OnMessegeTCP(bytes.ToArray());
            await _tcpListener.StopListeningAsync();
        }


        protected virtual void OnMessegeTCP(byte[] bytes)
        {
            MessegeReceived?.Invoke(this, new MessegeEventArgs() { Messege = bytes });
        }

        public async Task receiveTCP()
        {
            Debug.WriteLine("TCP: Starting Listening!");
            tcpListener.ConnectionReceived += ConnectionReceived;
            await tcpListener.StartListeningAsync(4444);
        }

        public async Task<byte[]> getData(string messege, string serverAddress)
        {
            Debug.WriteLine("Sending {0} to {1}!", messege, serverAddress);
            byte[] data = Encoding.UTF8.GetBytes(messege + "\n");
            await tcpClient.ConnectAsync(serverAddress, 4444);
            await tcpClient.WriteStream.WriteAsync(data, 0, data.Length);
            int readByte = 0;
            List<byte> queue = new List<byte>();
            while (readByte != -1)
            {
                readByte = tcpClient.ReadStream.ReadByte();
                Debug.WriteLine("TCP/ Rec: {0}", readByte);
                queue.Add((byte)readByte);
            }
            await tcpClient.DisconnectAsync();
            return queue.ToArray();
        }

        public async Task<byte[]> SendData(byte[] data, string serverAddress)
        {
            if (isSending)
            {
                return null;
            }
            try
            {
                isSending = true;
                Debug.WriteLine("Sending {0} to {1}!", Encoding.UTF8.GetString(data, 0, data.Length), serverAddress);
                byte[] queueString = Encoding.UTF8.GetBytes("Queue: ");
                byte[] message = new byte[queueString.Length + data.Length];
                System.Buffer.BlockCopy(queueString, 0, message, 0, queueString.Length);
                System.Buffer.BlockCopy(data, 0, message, queueString.Length, data.Length);
                await tcpClient.ConnectAsync(serverAddress, 4444);
                Debug.WriteLine("Connected!");
                await tcpClient.WriteStream.WriteAsync(message, 0, message.Length);
                //await tcpClient.WriteStream.WriteAsync(data, 0, data.Length);
                int readByte = 0;
                List<byte> response = new List<byte>();
                while (readByte != -1)
                {
                    readByte = tcpClient.ReadStream.ReadByte();
                    Debug.WriteLine("TCP/ Rec: {0}", readByte);
                    response.Add((byte)readByte);
                }
                isSending = false;
                return response.ToArray();
            }
            catch (Exception ex)
            {
                isSending = false;
                Debug.WriteLine("Message:{0} Data:{1} Source:{2}", ex.Message, ex.Data, ex.Source);
                return null;
            }
        }


        public async Task stopReceiving()
        {
            tcpListener.ConnectionReceived -= ConnectionReceived;
            Debug.WriteLine("TCP: Stopping Listening!");
            await tcpListener.StopListeningAsync();
        }
    }
}
