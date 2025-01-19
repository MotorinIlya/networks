using System;

namespace Snake.Net;

//const for net package
public static class NetConst 
{
    public const string MulticastIP = "239.192.0.4";
    public const int MulticastPort = 9192;
    public const int UnicastPort = 0;
    public static TimeSpan ExpirationTime = TimeSpan.FromSeconds(0.8);
    public const string MyIp = "127.0.0.1";
    public const int StartDelay = 1000;
}