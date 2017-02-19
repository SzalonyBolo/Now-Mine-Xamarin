using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App2.Network;
using Xamarin.Forms;
using System.Diagnostics;

namespace App2
{
    public partial class QueuePage : ContentPage
    {
        //Network network;
        public ServerConnection serverConnection;
        private List<MusicPiece> queue;
        public QueuePage(ServerConnection serverConnection)
        {
            InitializeComponent();
            this.serverConnection = serverConnection;
            btnGetQueue.Clicked += BtnGetQueue_Clicked;
            this.queue = new List<MusicPiece>();
        }

        private async void BtnGetQueue_Clicked(object sender, EventArgs e)
        {
            await getQueue();
            renderQueue();
        }

        private void renderQueue()
        {
            //sltQueue.Children.Clear();
            foreach (MusicPiece musicPiece in queue)
            {
                sltQueue.Children.Add(musicPiece);
                //sltQueue.Children.Add(new Label() { Text = musicPiece.Info.title });
            }
        }

        private async Task getQueue()
        {
            Debug.WriteLine("Get Queue!");
            IList<YoutubeInfo> infos = await serverConnection.getQueue();
            foreach(YoutubeInfo info in infos)
            {
                var musicPiece = new MusicPiece(info);
                queue.Add(musicPiece);
            }
            //if (queue != null && queue.Length > 0)
            //{
            //    int qCount = queue.Length;
            //    //MusicPieceView[] mPieces = new MusicPieceView[qCount];
            //    for (int i = 0; i < qCount; i++)
            //    {
            //        //YoutubeInfo.QueueInfo qInfo = new JsonConvert.(queue[i], YoutubeInfo.QueueInfo.class);
            //        //mPieces[i] = new MusicPieceView(getBaseContext() ,qInfo);
            //        //lnlQueue.addView(mPieces[i]);
            //   }
            //}
        }
    }
}
