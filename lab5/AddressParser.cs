using System.Text;
using System.Net;

namespace SOCKCSProxy
{
    public class AddressParser
    {
        public async static Task<(IPAddress, int)> ParseAddress(byte[] buffer)
        {

            IPAddress? destinationIP = null;
            int destinationPort = 0;

            switch (buffer[3])
            {
                case 0x01:
                    destinationIP = new IPAddress(new byte[] {buffer[4], buffer[5], buffer[6], buffer[7]} );
                    destinationPort = (int)buffer[8] << 8 | (int)buffer[9];
                    return  (destinationIP, destinationPort);
                case 0x03:
                    int domainLength = (int)buffer[4];
                    string domainName = Encoding.ASCII.GetString(buffer, 5, domainLength);

                    IPHostEntry hostEntry = await Dns.GetHostEntryAsync(domainName);
                    destinationIP = hostEntry.AddressList[0];

                    destinationPort = (int)buffer[5 + domainLength] << 8 | (int)buffer[6 + domainLength];
                    return (destinationIP, destinationPort);

                default:
                    throw new UnknownAddressException();
            }
        }
    }
}