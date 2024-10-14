using System.Net.Sockets;
using System.Net;
using System.Text;

namespace SOCKCSProxy
{

    class SOCKCSProxy
    {

        private const int TRANSFER_BUFFER_SIZE = 1024 * 8;
        private const int ProxyPort = 1080;  
        public static async Task Main(string[] args) 
        {
            TcpListener listener = new TcpListener(IPAddress.Any, ProxyPort);
            listener.Start();
            Console.WriteLine($"SOCKS5 proxy server started on port {ProxyPort}");

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                _ = HandleClientAsync(client);
            }
        }

        public async static Task HandleClientAsync(TcpClient client)
        {
            var stream = client.GetStream();

            byte[] buffer = new byte[1024];
            int readBytes = await stream.ReadAsync(buffer, 0,buffer.Length);
            
            if (readBytes == 0 || buffer[0] != 0x05) 
            {
                Console.WriteLine("it is not SOCKS5");
                return;
            }

            Console.WriteLine("begin protocol");

            await stream.WriteAsync(new byte[] {0x05, 0x00}, 0, 2);

            await stream.ReadAsync(buffer, 0, buffer.Length);

            if (readBytes == 0 || buffer[0] != 0x05)
            {
                Console.WriteLine("it is not SOCKS5");
                return;
            }

            if (buffer[1] != 0x01) 
            {
                Console.WriteLine("Unknown or unrealized comand");
                return;
            }

            Console.WriteLine("receive tcp connection");

            IPAddress destinationIP = null;
            int destinationPort = 0;

            switch (buffer[3])
            {
                case 0x01:
                    destinationIP = new IPAddress(new byte[] {buffer[4], buffer[5], buffer[6], buffer[7]} );
                    destinationPort = (int)buffer[8] << 8 | (int)buffer[9];
                    break;
                case 0x03:
                    int domainLength = (int)buffer[4];
                    string domainName = Encoding.ASCII.GetString(buffer, 5, domainLength);

                    try
                    {
                        IPHostEntry hostEntry = await Dns.GetHostEntryAsync(domainName);
                        destinationIP = hostEntry.AddressList[0];
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"DNS resolution failed for {domainName}: {ex.Message}");
                        await stream.WriteAsync(new byte[] 
                                                    { 0x05, 0x04, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 0, 10);
                        client.Close();
                        return;
                    }
                    destinationPort = (int)buffer[5 + domainLength] << 8 | (int)buffer[6 + domainLength];
                    break;

                default:
                    Console.WriteLine("Unknown address type");
                    await stream.WriteAsync(new byte[] 
                                                    { 0x05, 0x08, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 0, 10);
                    client.Close();
                    return;
            }
            
            Console.WriteLine($"get ip and port {destinationIP}");

            try
            {
                TcpClient remoteClient = new TcpClient();
                await remoteClient.ConnectAsync(destinationIP, destinationPort);
                

                await stream.WriteAsync(new byte[] { 0x05, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 0, 10);

                _ = Task.WhenAll(TransferStream(client.GetStream(), remoteClient.GetStream()), TransferStream(remoteClient.GetStream(), client.GetStream()));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unknown address type");
                await stream.WriteAsync(new byte[] { 0x05, 0x08, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 0, 10);
                client.Close();
                return;
            }
        }

        private static async Task TransferStream(NetworkStream input, NetworkStream output)
        {
            byte[] buffer = new byte[TRANSFER_BUFFER_SIZE];
            int length = 0;
            while((length = await input.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await output.WriteAsync(buffer, 0, length);
                Console.WriteLine("Goooal");
            }
        }
    }
}