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
        private List<User> users;
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
            Device.BeginInvokeOnMainThread(() => { sltQueue.Children.Clear(); }); 
            foreach (MusicPiece musicPiece in Queue)
            {
                Device.BeginInvokeOnMainThread(() => { sltQueue.Children.Add(musicPiece); });
            }
        }

        public async Task getQueue()
        {
            Debug.WriteLine("Get Queue!");
            IList<YoutubeInfo> infos = await serverConnection.getQueueTCP();
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

        internal async Task getUsers()
        {
            Debug.WriteLine("Get Users!");
            this.users = new List<User>(await serverConnection.getUsers());
        }

        public void SuccessfulQueued(object s, PiecePosArgs e)
        {
            //await getQueue();
            int qPos = e.QPos == -1 ? Queue.Count : e.QPos;
            Queue.Insert(qPos, e.MusicPiece);
            renderQueue();
        }

        internal void PlayedNow(object s, GenericEventArgs<int> e)
        {
            int qPos = e.EventData == -1 ? Queue.Count : e.EventData;
            Queue.RemoveAt(0);
            MusicPiece playingNow = Queue.ElementAt(qPos);
            Queue.Insert(0, playingNow);
            Queue.RemoveAt(qPos);
            renderQueue();
        }

        internal void DeletePiece(object s, GenericEventArgs<int> e)
        {
            Queue.RemoveAt(e.EventData);
            Device.BeginInvokeOnMainThread(() => { renderQueue(); });
        }

        internal void QueueReveiced(object s, PiecePosArgs e)
        {
            int qPos = e.QPos == -1 ? Queue.Count : e.QPos;
            Queue.Insert(qPos, e.MusicPiece);
            renderQueue();
        }
    }
}
