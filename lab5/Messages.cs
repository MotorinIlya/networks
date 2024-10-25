using System.Net.Sockets;
using System.Net;


namespace SOCKCSProxy
{
    public class Messages
    {
        public static async Task SendWithoutAuthentication(NetworkStream stream)
        {
            stream.WriteAsync(new byte[] {
                (byte)Bytes.Version, 
                (byte)Bytes.WithoutAuthentication }, 
                0, 2);
        }

        public static async Task SendUnknownAddress(NetworkStream stream)
        {
            stream.WriteAsync(new byte[] { 
                (byte)Bytes.Version, 
                (byte)Bytes.UnknownAddress, 
                (byte)Bytes.Reserve, 
                (byte)Bytes.IPV4, 
                (byte)Bytes.AddressPort, 
                (byte)Bytes.AddressPort, 
                (byte)Bytes.AddressPort, 
                (byte)Bytes.AddressPort, 
                (byte)Bytes.AddressPort, 
                (byte)Bytes.AddressPort }, 
                0, 10);
        }

        public static async Task SendDNSFailed(NetworkStream stream)
        {
            stream.WriteAsync(new byte[] { 
                (byte)Bytes.Version, 
                (byte)Bytes.UnavailableHost, 
                (byte)Bytes.Reserve, 
                (byte)Bytes.IPV4, 
                (byte)Bytes.AddressPort, 
                (byte)Bytes.AddressPort, 
                (byte)Bytes.AddressPort, 
                (byte)Bytes.AddressPort, 
                (byte)Bytes.AddressPort, 
                (byte)Bytes.AddressPort }, 
                0, 10);
        }

        public static async Task SendAccept(NetworkStream stream)
        {
            stream.WriteAsync(new byte[] { 
                (byte)Bytes.Version, 
                (byte)Bytes.Accept, 
                (byte)Bytes.Reserve, 
                (byte)Bytes.IPV4, 
                (byte)Bytes.AddressPort, 
                (byte)Bytes.AddressPort, 
                (byte)Bytes.AddressPort, 
                (byte)Bytes.AddressPort, 
                (byte)Bytes.AddressPort, 
                (byte)Bytes.AddressPort }, 
                0, 10);
        }
    }
}