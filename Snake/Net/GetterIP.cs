using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Snake.Net;


public static class GetterIP
{
    public static IPAddress GetLocalIpAddress()
    {
        var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
        foreach (var networkInterface in networkInterfaces)
        {
            if (networkInterface.OperationalStatus != OperationalStatus.Up)
                continue;
            var ipProperties = networkInterface.GetIPProperties();
            var unicastAddresses = ipProperties.UnicastAddresses;
            foreach (var unicastAddress in unicastAddresses)
            {
                if (unicastAddress.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    if (!IPAddress.IsLoopback(unicastAddress.Address))
                    {
                        return unicastAddress.Address;
                    }
                }
            }
        }

        throw new InvalidOperationException("No active network interface with an IPv4 address found.");
    }
}