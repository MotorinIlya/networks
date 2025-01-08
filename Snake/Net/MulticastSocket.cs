using System.Net;
using System.Net.Sockets;

namespace Snake.Net;

public class MulticastSocket : UdpClient
{
    private static IPAddress _multicastIP = IPAddress.Parse(NetConst.MulticastIP);

    private const int _multicastPort = NetConst.MulticastPort;

    public MulticastSocket () : base (AddressFamily.InterNetwork)
    {
        Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        JoinMulticastGroup(_multicastIP);
    }

    public void Bind () => Client.Bind(new IPEndPoint(IPAddress.Any, MulticastPort)); 

    public static IPAddress MulticastIP => _multicastIP;

    public static int MulticastPort => _multicastPort;


    
}