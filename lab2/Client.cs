using System.Net.Sockets;
using System;
using System.Text;

namespace lab2 
{
    public class Client
    {
        string host;
        int port;
        TcpClient client;
        static const int ACCEPT = 2;
        static const int SIZE_DATA = 4096;
        public Client (string host, int port)
        {
            this.host = host;
            this.port = port;
            client = new TcpClient();
        }

        public void sendFile (string file) 
        {
            try
            {
                Console.WriteLine ($"connect to {host}:{port}");
                client.Connect(host, port);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return;
            }
            NetworkStream stream = client.GetStream();

            byte[] message = Encoding.UTF8.GetBytes(file);

            stream.Write(message, 0, message.Length);
            
            byte[] answer = new byte[ACCEPT];
            stream.Read(answer);

            FileInfo fileInfo = new FileInfo(file);
            byte[] len = Encoding.UTF8.GetBytes(fileInfo.Length.ToString());
            Console.WriteLine(fileInfo.Length.ToString());

            stream.Write(len, 0, len.Length);
            stream.Read(answer, 0, answer.Length);
            
            int file_size = (int)fileInfo.Length;
            int count = 0;

            StreamReader reader = new StreamReader(file);

            char[] chars = new char[SIZE_DATA];
            while (file_size > 0) 
            {
                count = reader.Read(chars, 0, SIZE_DATA);
                byte[] data = Encoding.UTF8.GetBytes(chars);
                stream.Write(data, 0, count);
                file_size -= count;
            }
            
            reader.Close();
            stream.Close();
            client.Close();

            Console.WriteLine("flash " + fileInfo.Length.ToString() + " bytes");
        }
    }
}