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

        enum RequestType
        {
            GET_IP = 1,
        };

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
                try
                {
                    string ip = GetLANIP();
                    logger.Debug("Ready to receive…");
                    byte[] data = new byte[1024];
                    int recv = sock2.ReceiveFrom(data, ref ep);
                    byte[] decryptData = Des.Decrypt(data, recv, "abcd1234");
                    string stringData = Encoding.ASCII.GetString(decryptData, 0, decryptData.Length);

                    logger.Info($"received: {stringData} from: {ep.ToString()}");

                    JToken root = JToken.Parse(stringData);
                    switch ((RequestType)(int)root["type"])
                    {
                        case RequestType.GET_IP:
                            JToken respObj = JToken.Parse("{}");
                            respObj["ip"] = ip;
                            byte[] respSrc = System.Text.Encoding.UTF8.GetBytes(respObj.ToString());
                            byte[] respEncrypt = Des.Encrypt(respSrc, respSrc.Length, "abcd1234");
                            sock2.SendTo(respEncrypt, 0, respEncrypt.Length, SocketFlags.None, ep);
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e.ToString());
                }
            }

        }


        public static string GetLANIP()
        {
            IPAddress[] IP = Dns.GetHostAddresses(Dns.GetHostName());
            int m_count = IP.Length;
            for (int i = 0; i < m_count; i++)
            {
                string ip = IP[i].ToString();
                if (ip.IndexOf(":") < 0 && !ip.EndsWith(".1"))
                {
                    return ip;
                }
            }
            return string.Empty;
        }
    }
}
