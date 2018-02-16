using esHelper.Common;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace esHelper
{
    public class Port
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sshIp">ssh主机ip</param>
        /// <param name="sshPort">ssh主机端口</param>
        /// <param name="username">ssh主机用户名</param>
        /// <param name="password">ssh主机密码</param>
        /// <param name="lanIp">本地或者远程主机ip</param>
        /// <param name="lanPort">本地或者远程主机端口</param>
        /// <returns></returns>
        public static SshClient GetSshClient(EsConnectionInfo connInfo)
        {
            SshClient client;
            //input.localPort = 8001;//todo：需要去获取本机未用端口
            try
            {
                PasswordConnectionInfo connectionInfo = new PasswordConnectionInfo(connInfo.sshIp, connInfo.sshPort, connInfo.username, connInfo.password);
                connectionInfo.Timeout = TimeSpan.FromSeconds(30);
                client = new SshClient(connectionInfo);
                //client.ErrorOccurred += Client_ErrorOccurred;
                client.Connect();
                if (!client.IsConnected)
                {
                    throw new Exception("SSH connect failed");
                }

                var portFwdL = new ForwardedPortLocal(connInfo.localIp, (uint)connInfo.localPort, connInfo.esIp, (uint)connInfo.esPort); //映射到本地端口
                client.AddForwardedPort(portFwdL);
                portFwdL.Start();

                if (!client.IsConnected)
                {
                    throw new Exception("Port forwarding failed");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return client;
        }

        public static int GetRandAvailablePort()
        {
            const int MIN_PORT_N = 1250;
            const int MAX_PORT_N = 8000;
            int MID = (MIN_PORT_N + 9 * MAX_PORT_N) / 10;
            Random rand = new Random();
            int start_port = rand.Next(MIN_PORT_N, MID);
            for (int i = start_port; i <= MAX_PORT_N; i++)
            {
                if (PortIsAvailable(i)) return i;
            }

            return -1;
        }

        // Get the used port list  
        public static List<int> PortIsUsed()
        {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipsTCP = ipGlobalProperties.GetActiveTcpListeners();
            IPEndPoint[] ipsUDP = ipGlobalProperties.GetActiveUdpListeners();
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            List<int> allPorts = new List<int>();
            foreach (IPEndPoint ep in ipsTCP) allPorts.Add(ep.Port);
            foreach (IPEndPoint ep in ipsUDP) allPorts.Add(ep.Port);
            foreach (TcpConnectionInformation conn in tcpConnInfoArray) allPorts.Add(conn.LocalEndPoint.Port);

            return allPorts;
        }

        // Check whether the port is in the used list  
        public static bool PortIsAvailable(int port)
        {
            bool isAvailable = true;
            List<int> portUsed = PortIsUsed();

            foreach (int p in portUsed)
            {
                if (p == port)
                {
                    isAvailable = false;
                    break;
                }
            }

            return isAvailable;
        }
    }
}
