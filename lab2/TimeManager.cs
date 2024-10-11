using System;

namespace SendingFiles
{
    class TimeManager
    {
        string nameFile;
        DateTime waitTime;
        int wait;

        public TimeManager (int wait, string nameFile)
        {
            this.wait = wait;
            this.waitTime = DateTime.Now;
            this.nameFile = nameFile;
        }

        public void printInformation (string info) 
        {
            if (DateTime.Now.CompareTo(waitTime) >= 0) 
            {
                waitTime = waitTime.AddSeconds(wait);
                Console.WriteLine(nameFile + ": " + info);
            }
        }

        public void asyncPrintInformation (string info)
        {
            Console.WriteLine(nameFile + ": " + info);
        }
    }
}