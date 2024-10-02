using System.Net;
using System.Net.Sockets;
using System.Text;
using System;

namespace lab1
{
    class UdpMulticast
    {
        IPAddress multicastIP;
        string uniqString;
        UdpClient udpClient;
        UdpClient udpServer;
        static int multicastPort = 12345;
        Dictionary<string, DateTime> activeCopies;
        Dictionary<string, IPEndPoint> activateIP;
        AddressFamily addressFamily;
        public UdpMulticast (IPAddress multicastIP)
        {
            this.multicastIP = multicastIP;
            uniqString = RandomString.getRandomString(10);
            addressFamily = multicastIP.AddressFamily;

            udpClient = new UdpClient(0, addressFamily == AddressFamily.InterNetworkV6 ? 
                                        AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork);
            udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            if (addressFamily == AddressFamily.InterNetworkV6) udpClient.JoinMulticastGroup(multicastIP);
            else udpClient.JoinMulticastGroup(multicastIP, IPAddress.Any);

            udpServer = new UdpClient(addressFamily == AddressFamily.InterNetworkV6 ? 
                                        AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork);
            udpServer.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            if (addressFamily == AddressFamily.InterNetworkV6) udpServer.JoinMulticastGroup(multicastIP);
            else udpServer.JoinMulticastGroup(multicastIP, IPAddress.Any);

            activeCopies = new Dictionary<string, DateTime>();
            activateIP = new Dictionary<string, IPEndPoint>();
        }

        public void SearchMulticastCopies ()
        {
            IPEndPoint localEndPoint = new IPEndPoint(addressFamily == AddressFamily.InterNetworkV6 ? 
                                                        IPAddress.IPv6Any : IPAddress.Any, multicastPort);
            udpServer.Client.Bind(localEndPoint);

            Thread receiveThread = new Thread(ReceiveMessages);
            receiveThread.Start();

            Thread sendThread = new Thread(SendMessages);
            sendThread.Start();

            while (true)
            {
                Printer.PrintActiveIP(activeCopies, activateIP);
                Thread.Sleep(1000);
            }
        }
        void SendMessages()
        {
            IPEndPoint multicastEndPoint = new IPEndPoint(multicastIP, multicastPort);
            while (true)
            {
                byte[] data = Encoding.UTF8.GetBytes(uniqString);
                udpClient.Send(data, data.Length, multicastEndPoint);

                Thread.Sleep(1000);
            }
        }

        void ReceiveMessages()
        {
            IPEndPoint? remoteEndPoint = null;

            while (true)
            {
                byte[] data = udpServer.Receive(ref remoteEndPoint);
                string message = Encoding.UTF8.GetString(data);

                lock (activeCopies)
                {
                    if (!activeCopies.ContainsKey(message))
                    {
                        Console.WriteLine($"Обнаружена новая копия: {remoteEndPoint.ToString}:{message}");
                    }

                    activeCopies[message] = DateTime.Now;
                    activateIP[message] = remoteEndPoint;
                }
            }
        }
    }
}