using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
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
    }
}
