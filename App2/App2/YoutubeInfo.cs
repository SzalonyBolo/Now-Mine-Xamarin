using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NowMine
{
    public class YoutubeInfo
    { 
        public string id;
        public string title;
        public string channelName;
        public Thumbnail thumbnail;
        public string userName;
        public string color;
    }

    public class YoutubeQueued : YoutubeInfo
    {
        public int qPos;
        public int userId;
    }
    //public class Thumbnail
    //{
    //    public string ETag;
    //    public long height;
    //    public string url;
    //    public long width;
    //}

    public class GenericEventArgs<T> : EventArgs
    {
        public T EventData { get; private set; }

        public GenericEventArgs(T EventData)
        {
            this.EventData = EventData;
        }
    }
}
