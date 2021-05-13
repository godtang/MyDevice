using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace broadcast
{
    class Program
    {
        public static void sendBroadcast(string msg, int port)
        {
            byte[] encryptData = Des.Encrypt(System.Text.Encoding.UTF8.GetBytes(msg), "abcd1234");
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Broadcast, port);
            sock.SetSocketOption(SocketOptionLevel.Socket,
                       SocketOptionName.Broadcast, 1);
            sock.SendTo(encryptData, iep);
            sock.Close();
        }
        static void Main(string[] args)
        {

            sendBroadcast("{port:23456}", 40000);
        }
    }
}
