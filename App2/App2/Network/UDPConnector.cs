using Sockets.Plugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App2.Network
{
    public class UDPConnector
    {
        private UdpSocketClient _udpClient;
        public UdpSocketClient udpClient
        {
            get
            {
                using (_udpClient = new UdpSocketClient())
                {
                    return _udpClient;
                }
            }
        }

        public async Task sendBroadcastUdp(string message, int port = 1234)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            string address = "255.255.255.255";
            try
            {
                var uc = new UdpSocketClient();
                await uc.SendToAsync(data, address, port);
                uc.Dispose();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            Debug.WriteLine("UDP: Sent {0} to {1}:{2}", message, address, port);
        }
    }


}
