using App2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Windows.Networking.Connectivity;
using Xamarin.Forms;
using System.Net;
using Sockets.Plugin;

namespace App2
{
    public partial class ServerCheckPage : ContentPage
    {
        readonly int listenPort = 1234;
        UdpSocketReceiver udpReceiver;
        bool isWifi = false;
        Network network = new Network();
        public ServerCheckPage()
        {
            InitializeComponent();
            //sldMain.Value = 0.5;

            //if (Device.OS == TargetPlatform.iOS)
            //{
            //    Padding = new Thickness(0, 20, 0, 0);

            //}

            //var networkStatus = networkConnection.IsConnected ? "Connected" : "Not Connected";
            network.ConnectedToServer += Network_ConnectedToServer;

            isWifi = network.isWifi();
            if (isWifi)
            {
                lblMain.Text = "KURWA DZIAŁA";
                btnRecive.IsVisible = true;
            }
            else
            {
                lblMain.Text = "COŚ SIĘ...COŚ SIĘ POPSUŁO";
            }
            //udpClient = new UdpSocketClient();
            //udpReceiver = new UdpSocketReceiver();
            //ReciveUdp();
            //udpReceiver.MessageReceived += Receiver_MessageReceived;
           
            btnRecive.Clicked += OnButtonClicked;
            //ReciveUdp();
            // listen for udp traffic on listenPort    
        }

        private void Network_ConnectedToServer(object source, ConnectedToServerEventArgs args)
        {
            lblMain.Text = "Znaleziono serwer: " + args.ServerAddress;
        }

        //private void Receiver_MessageReceived(object sender, Sockets.Plugin.Abstractions.UdpSocketMessageReceivedEventArgs e)
        //{
        //   var from = String.Format("{0}:{1}", e.RemoteAddress, e.RemotePort);
        //   var data = Encoding.UTF8.GetString(e.ByteData, 0, e.ByteData.Length);

        //   lblMain.Text = (from + " - " + data);
        //}

        private async Task ReciveUdp()
        {
            await udpReceiver.StartListeningAsync(listenPort);
        }

        async void OnButtonClicked(object sender, EventArgs e)
        {
            //ReciveUdp();
            //await udpReceiver.StartListeningAsync(listenPort);
            lblMain.Text = "Szukanie serwera...";
            await network.findServer(this);
            //if (isServer)
            //{
            //    await Navigation.PushAsync(new ServerCheckPage());
            //}
            //else
            //{
            //    lblMain.Text = "Nie znaleziono serwera :(";
            //}
        }

        async internal void loadNext()
        {
            //await Navigation.PushAsync(new ServerCheckPage());
            this.Content = new QueuePage().Content;
        }
        

    }
}
