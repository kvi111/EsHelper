using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace esHelper.Common
{
    public class EsConnectionInfo
    {
        public string connectionName { get; set; }
        public string esIp { get; set; }
        public int esPort { get; set; }
        public string esUsername { get; set; }
        public string esPassword { get; set; }


        public bool isUseSSH { get; set; }
        public string sshIp { get; set; }
        public int sshPort { get; set; }
        public string username { get; set; }
        public string password { get; set; }


        public string localIp { get { return "localhost"; } }
        public int localPort { get; set; }

        public string GetLastUrl()
        {
            if (isUseSSH)
            {
                return "http://" + localIp + ":" + localPort.ToString();
            }
            else
            {
                return "http://" + esIp + ":" + esPort.ToString();
            }
        }
    }
}
