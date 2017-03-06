using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NowMine.Network;
using Xamarin.Forms;
using System.Diagnostics;

namespace NowMine
{
    public partial class QueuePage : ContentPage
    {
        //Network network;
        public ServerConnection serverConnection;
        private List<MusicPiece> _queue;
        public List<MusicPiece> Queue
        {
            get
            {
                if (_queue == null)
                    _queue = new List<MusicPiece>();
                return _queue;
            }
            set
            {
                _queue = value;
            }
        }
        public QueuePage(ServerConnection serverConnection)
        {
            InitializeComponent();
            this.serverConnection = serverConnection;
        }

        private void renderQueue()
        {
            sltQueue.Children.Clear();
            foreach (MusicPiece musicPiece in Queue)
            {
                sltQueue.Children.Add(musicPiece);
            }
        }

        public async Task getQueue()
        {
            Debug.WriteLine("Get Queue!");
            IList<YoutubeInfo> infos = await serverConnection.getQueue();
            if (infos == null)
            {
                sltQueue.Children.Add(new Label() { Text = "Nie dogadałem się z serwerem :/" } );
            }
            else
            {
                foreach (YoutubeInfo info in infos)
                {
                    var musicPiece = new MusicPiece(info);
                    Queue.Add(musicPiece);
                }
                renderQueue();
            }
        }

        public void SuccessfulQueued(object s, SuccessfulQueuedArgs e)
        {
            //await getQueue();
            int qPos = e.QPos == -1 ? Queue.Count : e.QPos;
            Queue.Insert(e.QPos, e.MusicPiece);
            renderQueue();
        }
    }
}
