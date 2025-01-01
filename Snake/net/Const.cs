using System;

namespace Snake.Net;

public class Const 
{
    public const string MulticastIP = "239.192.0.4";

    public const int MulticastPort = 9192;

    public static TimeSpan ExpirationTime = TimeSpan.FromSeconds(0.8 * 5);
}