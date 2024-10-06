using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace lab2
{
    public class Server 
    {
        IPEndPoint point;
        TcpListener listener;
        string directory;
        static int SIZE_DATA = 4096;
        static int ACCEPT = 2;
        public Server (int port)
        {
            point = new IPEndPoint(IPAddress.Any, port);
            listener = new TcpListener(point);
            directory = Directory.GetCurrentDirectory() + "/uploads/";
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public void Start ()
        {
            listener.Start();
            Console.WriteLine("TCP server is running");

            while (true)
            {
                TcpClient socket = listener.AcceptTcpClient();
                Thread fileThread = new Thread(ReceiveFile);
                fileThread.Start(socket);
            }
        }

        void ReceiveFile (object? obj) 
        {   
            if (obj is TcpClient socket)
            {
                byte[] fileName = new byte[SIZE_DATA];
                byte[] length = new byte[SIZE_DATA];
                byte[] accept = new byte[ACCEPT];
                byte[] data = new byte[SIZE_DATA];
                for (int i = 0; i < ACCEPT; i++) accept[i] = (byte)1;


                NetworkStream stream = socket.GetStream();

                stream.Read(fileName, 0, SIZE_DATA);
                stream.Write(accept, 0, ACCEPT);

                stream.Read(length, 0, SIZE_DATA);
                stream.Write(accept, 0, ACCEPT);

                string name = Encoding.UTF8.GetString(fileName).TrimEnd('\0').Trim();
                FileCreator fileCreator = new FileCreator(directory, Path.GetFileName(name));
                string newFileName = fileCreator.CreateName();
                
                Console.WriteLine(newFileName);

                File.Create(newFileName).Close();
                int len = int.Parse(Encoding.UTF8.GetString(length));
                int lenForCount = len;

                //TimeManager manager = new TimeManager(3);
                FileStream fileStream = new FileStream(newFileName, FileMode.Create, FileAccess.Write);
                var sw = new Stopwatch();
                int count = 0;
                while (len > 0) 
                {
                    sw.Start();
                    count = stream.Read(data, 0, SIZE_DATA);
                    sw.Stop();
                    fileStream.Write(data, 0, count);
                    len -= count;
                    //Console.WriteLine("why manager is stupid");
                    //manager.printInformation((((double)count) / (double)sw.Elapsed.Microseconds).ToString() + "MB/s");
                }

                fileStream.Close();
                stream.Close();
                socket.Close();
            }
        }
    }
}