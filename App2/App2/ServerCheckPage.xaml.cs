using NowMine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Net;
using Sockets.Plugin;
using NowMine.Network;
using System.Diagnostics;

namespace NowMine
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

        private void searchServer()
        {
            serverConnection.ServerConnected += ServerConnected;
            serverConnection.findServer();
        }

        private async void ServerConnected(object s, EventArgs e)
        {
            serverConnection.ServerConnected -= ServerConnected;
            Debug.WriteLine("GUI: Open Queue Page!");
            Device.BeginInvokeOnMainThread(() => { lblMain.Text = "Znaleziono Serwer!"; });
            var tabbedPage = new TabbedPage();
            var queuePage = new QueuePage(serverConnection);
            await queuePage.getQueue();

            var ytSearchPage = new YoutubeSearchPage(serverConnection);
            ytSearchPage.SuccessfulQueued += queuePage.SuccessfulQueued;

            tabbedPage.Children.Add(queuePage);
            tabbedPage.Children.Add(ytSearchPage);
            Device.BeginInvokeOnMainThread(() => { App.Current.MainPage = tabbedPage; });
            //QueuePage queuePage = new QueuePage(serverConnection);
            //await queuePage.getQueue();
            //Device.BeginInvokeOnMainThread(async () => { await Navigation.PushAsync(queuePage); });
        }
    }
}
