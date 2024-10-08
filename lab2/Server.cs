using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace lab2
{
    public class Server : NetworkWorker
    {
        IPEndPoint point;
        TcpListener listener;
        string directory;
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
                File.Create(newFileName).Close();

                long len = long.Parse(Encoding.UTF8.GetString(length));
                long lenForCount = len;

                TimeManager manager = new TimeManager(3, Path.GetFileName(newFileName));
                FileStream fileStream = new FileStream(newFileName, FileMode.Create, FileAccess.Write);

                var sw = new Stopwatch();
                long freq = Stopwatch.Frequency;
                int count = 0;
                
                long time = 0;

                var watch = new Stopwatch();
                watch.Start();
                while (len > 0) 
                {
                    sw.Restart();
                    count = stream.Read(data, 0, SIZE_DATA);
                    sw.Stop();
                    fileStream.Write(data, 0, count);
                    len -= count;
                    time += (long)sw.ElapsedTicks;

                    long moment_speed = (long)((double)count * (double)freq / (double)sw.ElapsedTicks) / 1_000_000;

                    manager.printInformation(moment_speed.ToString() + "MB/s");
                }
                watch.Stop();
                manager.asyncPrintInformation("all time to send is " + ((double)time / (double)freq).ToString() + " seconds");
                manager.asyncPrintInformation("all time is " + ((double)watch.ElapsedTicks / (double)freq).ToString() + " seconds");
                manager.asyncPrintInformation("average speed is " + ((lenForCount * freq / time) / 1_000_000).ToString() + " MB/s");

                fileStream.Close();
                stream.Close();
                socket.Close();
            }
        }
    }
}