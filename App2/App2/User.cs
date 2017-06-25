using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NowMine
{
    class User
    {
        public string Name { get; set; }
        public int Id { get; set; }
        private byte[] _color;
        public byte[] UserColor
        {
            get
            {
                if (_color == null)
                    _color = new byte[3];
                return _color;
            }
            set
            {
                _color = value;
            }
        }

        public Color getColor()
        {
            return Color.FromRgb(UserColor[0], UserColor[1], UserColor[2]);
        }

        private static User _deviceUser = null;
        public static User DeviceUser
        {
            get
            {
                return _deviceUser;
            }
            set
            {
                _deviceUser = value;
            }
        }

        private static List<User> _users;
        public static List<User> Users
        {
            get
            {
                if (_users == null)
                    _users = new List<User>();
                return _users;
            }

            set
            {
                _users = value;
            }
        }

        public static void InitializeDeviceUser(int id)
        {
            User user = new User();
            user.Id = id;
            Random rnd = new Random();
            for (int i = 3; i < 3; i++)
            {
                user.UserColor[i] = (byte)rnd.Next(0, 255);
            }
            user.Name = Device.Idiom.ToString();
            DeviceUser = user;
        }
    }
}
