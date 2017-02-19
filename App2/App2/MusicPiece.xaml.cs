using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace App2
{
    public partial class MusicPiece : ContentView
    { 
        private YoutubeInfo info = null;
        private DateTime created { get; set; }
        private DateTime played { get; set; }

        public YoutubeInfo Info
        {
            get { return info; }
            set
            {
                info = value;
                setTitle = info.title;
                setChannelName = info.channelName;
                //setImage = info.thumbnail.url;
                HeightRequest = 150;
            }
        }

        private string setTitle
        {
            set
            {
                lblTitle.Text = value;
            }
        }

        private string setChannelName
        {
            set
            {
                lblChannelName.Text = value;
            }
        }

        //private string setImage
        //{
        //    set
        //    {

        //        BitmapImage bmp = new BitmapImage(new Uri(value, UriKind.RelativeOrAbsolute));
        //        imgMain.Source = bmp;
        //    }
        //}

        public MusicPiece()
        {
            InitializeComponent();
        }

        // to the SearchBar
        public MusicPiece(YoutubeInfo inf)
        {
            InitializeComponent();
            this.info = inf;
            lblTitle.Text = inf.title;
            lblChannelName.Text = inf.channelName;
            //setImage = inf.thumbnail.url;
            lbluserName.Text = inf.userName;
            //lbluserName.Text = Visibility.Hidden;
            created = DateTime.Now;
        }

        //to Queue
        //public MusicPiece(YoutubeInfo inf, User user)
        //{
        //    InitializeComponent();
        //    this.info = inf;
        //    lblTitle.Content = info.title;
        //    lblChannelName.Content = info.channelName;
        //    setImage = info.thumbnail.Url;
        //    created = DateTime.Now;
        //    lbluserName.Content = user.name;
        //}

        public MusicPiece copy()
        {
            MusicPiece musicPiece = new MusicPiece();
            musicPiece.InitializeComponent();
            musicPiece.info = this.info;
            musicPiece.lblTitle.Text = info.title;
            musicPiece.lblChannelName.Text = info.channelName;
            //musicPiece.setImage = info.thumbnail.url;
            musicPiece.created = DateTime.Now;
            musicPiece.lbluserName.Text = lbluserName.Text;
            return musicPiece;
        }

        //internal void nowPlayingVisual()
        //{
        //    SolidColorBrush redBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        //    this.border.BorderBrush = redBrush;
        //}

        //internal void userColorBrush(User user)
        //{
        //    SolidColorBrush userBrush = new SolidColorBrush(user.getColor());
        //    this.border.BorderBrush = userBrush;
        //    this.dropShadowEffect.Color = user.getColor();
        //    this.recBackground.Fill = userBrush;
        //    this.recBackground.Opacity = 0.3d;
        //}

        //internal void historyVisual()
        //{
        //    SolidColorBrush greyBrush = new SolidColorBrush(Color.FromRgb(111, 111, 111));
        //    this.border.BorderBrush = greyBrush;
        //    this.lblTitle.BorderBrush = greyBrush;
        //    this.lblChannelName.BorderBrush = greyBrush;
        //}

        public void setPlayedDate()
        {
            this.played = DateTime.Now;
        }
    }
}
