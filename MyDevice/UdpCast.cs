using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyDevice
{
    class UdpCast
    {
        static ILog logger = LogManager.GetLogger("UB.File");
        Thread t = new Thread(new ThreadStart(RecvThread));
        public void Start()
        {
            t.IsBackground = true;
            t.Start();
        }

        public void Stop()
        {
            t.Abort();
        }

        public static void RecvThread()
        {
            int destPort = 40000;
            Socket sock2 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            //创建一个端口为destPort的终结点
            IPEndPoint iep;

            while (true)
            {
                try
                {
                    iep = new IPEndPoint(IPAddress.Any, destPort);
                    //绑定
                    sock2.Bind(iep);
                    break;
                }
                catch (System.Net.Sockets.SocketException e)
                {
                    if (e.ErrorCode == 0x00002740)
                    {
                        destPort++;
                        continue;
                    }
                }
            }
            EndPoint ep = (EndPoint)iep;
            while (true)
            {
                logger.Debug("Ready to receive…");
                byte[] data = new byte[1024];
                int recv = sock2.ReceiveFrom(data, ref ep);
                byte[] decryptData = Des.Decrypt(data, recv, "abcd1234");
                string stringData = Encoding.ASCII.GetString(decryptData, 0, decryptData.Length);

                logger.Info($"received: {stringData} from: {ep.ToString()}");
            }

        }

        public static void sendBroadcast(string msg, int port)
        {
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Broadcast, port);
            byte[] data = Encoding.UTF8.GetBytes(msg);
            sock.SetSocketOption(SocketOptionLevel.Socket,
                       SocketOptionName.Broadcast, 1);
            sock.SendTo(data, iep);
            sock.Close();
        }
    }
}
