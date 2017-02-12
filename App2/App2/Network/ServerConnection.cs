using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace App2.Network
{
    public class ServerConnection
    {
        public delegate void ServerConnectedEventHandler(object s, EventArgs e);
        public event ServerConnectedEventHandler ServerConnected;
        private string serverAddress = "";

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

        public async Task<bool> findServer()
        {
            tcpConnector.MessegeReceived += OnServerFound;
            Device.StartTimer(TimeSpan.FromSeconds(10), () =>
           {
               Task.Factory.StartNew(async () =>
               {
                   await udpConnector.sendBroadcastUdp("NowMine!");
                   await tcpConnector.receiveTCP();
                   if (string.IsNullOrEmpty(serverAddress))
                   {
                       return false;
                   }
                       return true;
               });
               return false;
            });
            return false;
        }

        private void OnServerFound(object source, MessegeEventArgs args)
        {
            string messege = args.messege;
            //tutaj sprawdzanie czy to ip itd
            serverAddress = messege;
            OnServerConnected();
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
