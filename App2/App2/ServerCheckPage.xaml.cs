using App2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Net;
using Sockets.Plugin;
using App2.Network;
using System.Diagnostics;

namespace App2
{
    public partial class ServerCheckPage : ContentPage
    {
        private ServerConnection _serverConnection { set; get; }
        internal ServerConnection serverConnection
        {
            get
            {
                if (_serverConnection == null)
                    _serverConnection = new ServerConnection();
                return _serverConnection;
            }
        }
        public ServerCheckPage()
        {
            InitializeComponent();

            if (serverConnection.isWifi())
            {
                lblMain.Text = "Wyszukiwanie Serwera Now Mine!";
                searchServer();
            }
            else
            {
                lblMain.Text = "Połącz się z siecią wifi w której działa Now Mine!";
            }
        }

        private async void searchServer()
        {
            serverConnection.ServerConnected += ServerConnected;
            await serverConnection.findServer();
        }

        private void ServerConnected(object s, EventArgs e)
        {
            serverConnection.ServerConnected -= ServerConnected;
            Debug.WriteLine("GUI: Open Queue Page!");
            Device.BeginInvokeOnMainThread(async () => { await Navigation.PushAsync(new QueuePage(serverConnection)); });
        }
    }
}
