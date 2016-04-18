using System;
using System.Net;
using System.Net.Sockets;

namespace CommonBase.Utils
{
    public class NetworkUtils
    {
        public static string LocalIpAddress
        {
            get { return GetLocalAddress(AddressFamily.InterNetwork); }
        }

        public static string GetLocalAddress(AddressFamily addressFamily)
        {
            string localAddress = String.Empty;
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == addressFamily)
                {
                    localAddress = ip.ToString();
                    break;
                }
            }
            return localAddress;
        }
    }
}