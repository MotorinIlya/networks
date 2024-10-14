using System;
using System.Diagnostics;

namespace SendingFiles
{
    class TimeManager
    {
        string nameFile;
        DateTime waitTime;
        int wait;
        Stopwatch sw;
        long freq = Stopwatch.Frequency;
        long time;
        long count_all_bytes;
        long count_bytes_now;
        long all_bytes;
        public TimeManager(int wait, string nameFile, long all_bytes)
        {
            this.wait = wait;
            this.waitTime = DateTime.Now;
            this.all_bytes = all_bytes;
            this.nameFile = nameFile;
            this.sw = new Stopwatch();
            this.time = 0;
            count_all_bytes = 0;
            count_bytes_now = 0;
        }

        public void Start() { sw.Start(); }
        public void Restart() { sw.Restart(); }

        public void Stop()
        {
            sw.Stop();
            time += sw.ElapsedTicks;
        }

        public void AddBytes(long bytes)
        {
            count_all_bytes += bytes;
            count_bytes_now = bytes;
        }

        public void PrintSpeed(bool periodic) 
        {

            if ((DateTime.Now.CompareTo(waitTime) >= 0) || !periodic) 
            {
                waitTime = waitTime.AddSeconds(wait);

                double moment_speed = ((double)count_bytes_now * (double)freq / (double)sw.ElapsedTicks) / 1_000_000;
                Console.WriteLine(nameFile + ": moment speed is " + moment_speed.ToString() + " MB/s");

                double average_speed = ((double)count_all_bytes * (double)freq / (double)time) / 1_000_000;
                Console.WriteLine(nameFile + ": average speed is " + average_speed + " MB/s");

                Console.WriteLine(nameFile + $": is {(count_all_bytes * 100) / all_bytes}% complete");
                Console.WriteLine(nameFile + $": spend {time / freq} seconds");
            }
        }

        public void PrintInformation(string info)
        {
            Console.WriteLine(nameFile + ": " + info);
        }
    }
}