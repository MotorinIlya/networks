using System.Net;
using System.Net.Sockets;

namespace Snake.Net;

public class MulticastSocket : UdpClient
{
    private static IPAddress multicastIP = IPAddress.Parse(Const.MulticastIP);

    private const int multicastPort = Const.MulticastPort;

    public MulticastSocket () : base (AddressFamily.InterNetwork)
    {
        Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        JoinMulticastGroup(multicastIP, IPAddress.Any);
    }

    public void Bind (IPEndPoint endPoint) => Client.Bind(endPoint); 

    public static IPAddress MulticastIP => multicastIP;

    public static int MulticastPort => multicastPort;
}