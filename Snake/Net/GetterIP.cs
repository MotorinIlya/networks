using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Snake.Net;


public static class GetterIP
{
    public static IPAddress GetLocalIpAddress()
    {
        // Получаем все сетевые интерфейсы
        var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

        // Ищем первый активный интерфейс с IPv4-адресом
        foreach (var networkInterface in networkInterfaces)
        {
            // Пропускаем неактивные интерфейсы
            if (networkInterface.OperationalStatus != OperationalStatus.Up)
                continue;

            // Получаем IP-адреса интерфейса
            var ipProperties = networkInterface.GetIPProperties();
            var unicastAddresses = ipProperties.UnicastAddresses;

            // Ищем IPv4-адрес
            foreach (var unicastAddress in unicastAddresses)
            {
                if (unicastAddress.Address.AddressFamily == AddressFamily.InterNetwork) // IPv4
                {
                    // Пропускаем локальные адреса (например, 127.0.0.1)
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