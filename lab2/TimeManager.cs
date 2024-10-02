using System;

namespace lab2
{
    class TimeManager
    {
        DateTime waitTime;
        int wait;

        public TimeManager (int wait)
        {
            this.wait = wait;
            this.waitTime = DateTime.Now;
        }

        public void printInformation (string info) 
        {
            if (DateTime.Now.CompareTo(waitTime) >= 0) 
            {
                waitTime.AddSeconds(wait);
                Console.WriteLine(info);
            }
        }
    }
}