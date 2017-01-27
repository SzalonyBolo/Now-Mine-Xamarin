using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App2
{
    class ServerConnection
    {
        static string address = "";
        public static string Address
        {
            get
            {
                return address;
            }
            set
            {
                address = value;
            }
        }

    }
}
