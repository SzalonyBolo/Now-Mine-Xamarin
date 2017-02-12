using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App2.Network;
using Xamarin.Forms;

namespace App2
{
    public partial class QueuePage : ContentPage
    {
        //Network network;
        public ServerConnection serverConnection;
        public QueuePage(ServerConnection serverConnection)
        {
            InitializeComponent();
            this.serverConnection = serverConnection;
            //getQueue();
        }

        private async void getQueue()
        {
            //string queue = await network.receiveTCP();
            //if (queue != null && queue.Length > 0)
            //{
            //    int qCount = queue.Length;
            //    MusicPieceView[] mPieces = new MusicPieceView[qCount];
            //    for (int i = 0; i < qCount; i++)
            //    {
            //        YoutubeInfo.QueueInfo qInfo = new JsonConvert.(queue[i], YoutubeInfo.QueueInfo.class);
            //        mPieces[i] = new MusicPieceView(getBaseContext() ,qInfo);
            //        lnlQueue.addView(mPieces[i]);
            //   }
            //}
        }
    }
}
