using log4net.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace MyDevice
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        private UdpCast udpCast = new UdpCast();
        protected override void OnStart(string[] args)
        {
            InitLog();
            udpCast.Start();
        }

        protected override void OnStop()
        {
            udpCast.Stop();
        }

        public void Start()
        {
            OnStart(null);
        }

        private void InitLog()
        {
            string loggerIni = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Service1)).Location);
            loggerIni = Path.Combine(loggerIni, "log4net.xml");
            var fileInfo = new FileInfo(loggerIni);
            XmlConfigurator.Configure(fileInfo);
        }
    }
}
