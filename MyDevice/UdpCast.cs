using log4net;
using Newtonsoft.Json.Linq;
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
            string a = GetAllIP();
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
                try
                {
                    logger.Debug("Ready to receive…");
                    byte[] data = new byte[1024];
                    int recv = sock2.ReceiveFrom(data, ref ep);
                    byte[] decryptData = Des.Decrypt(data, recv, "abcd1234");
                    string stringData = Encoding.ASCII.GetString(decryptData, 0, decryptData.Length);

                    logger.Info($"received: {stringData} from: {ep.ToString()}");

                    JToken root = JToken.Parse(stringData);

                    sendBroadcast("hello", (int)root["port"]);
                }
                catch (Exception e)
                {
                    logger.Error(e.ToString());
                }
            }

        }

        public static void sendBroadcast(string msg, int port)
        {
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Broadcast, port);
            byte[] data = Encoding.UTF8.GetBytes(msg);
            byte[] encryptData = Des.Encrypt(data, data.Length, "abcd1234");
            sock.SetSocketOption(SocketOptionLevel.Socket,
                       SocketOptionName.Broadcast, 1);
            sock.SendTo(encryptData, iep);
            sock.Close();
        }

        public static string GetAllIP()
        {
            IPAddress[] IP = Dns.GetHostAddresses(Dns.GetHostName());
            int m_count = IP.Length;
            string m_AllIP = string.Empty;
            for (int i = 0; i < m_count; i++)
            {
                if (i > 0)
                    m_AllIP = m_AllIP + "|";
                m_AllIP += IP[i].ToString();

            }
            return m_AllIP;
        }
    }
}
