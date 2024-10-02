using System.Net;


namespace lab1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start program");

            string multicastAddress = Console.ReadLine();

            IPAddress multicastIP;
            if (!IPAddress.TryParse(multicastAddress, out multicastIP))
            {
                Console.WriteLine("bad format multicast addres.");
                return;
            }

            UdpMulticast socket = new UdpMulticast(multicastIP);
            socket.SearchMulticastCopies();
        }
    }

}
