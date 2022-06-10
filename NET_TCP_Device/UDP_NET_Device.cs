using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace NET_TCP_Device
{
    class UDP_NET_Device
    {
        private IPAddress udpRemoteAddress;    // хост для отправки данных
        private bool exit = false;

        private int udpRemotePort;             //  UDP порт для отправки данных
        private string udpAddress;             //  UDP IP address
        private string udpAppName;

        Thread sendingThread;

        public UDP_NET_Device()
        {
            
        }

        public void Connect(string addr, int rPort, string appName)
        {
            exit = false;

            udpAddress = addr;
            udpRemotePort = rPort;
            udpAppName = appName;

            udpRemoteAddress = IPAddress.Parse(addr);
            sendingThread = new Thread(SendingConnectionIP);
            sendingThread.Start();
        }

        public void Disconnect()
        {
            exit = true;
        }
        private void SendingConnectionIP()
        {
            UdpClient sender = new UdpClient(); // создаем UdpClient для отправки
            IPEndPoint endPoint = new IPEndPoint(udpRemoteAddress, udpRemotePort);
            byte[] data = Encoding.UTF8.GetBytes(udpAppName);

            try
            {
                while (!exit)
                {
                    sender.Send(data, data.Length, endPoint); // отправка
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                sender.Close();
            }
        }

        private static string LocalIPAddress()
        {
            string localIP = "";
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }
    }
}
