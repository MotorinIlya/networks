using System.Net;
using System.Net.Sockets;

namespace lab1
{
    public class Printer
    {
        static TimeSpan expirationTime = TimeSpan.FromSeconds(5);
        public static void PrintActiveIP (Dictionary<IPEndPoint, DateTime> activeCopies)
        {
            lock (activeCopies)
            {
                DateTime now = DateTime.Now;
                List<string> expiredCopies = new List<string>();
                foreach (var copy in activeCopies)
                {
                    if (now - copy.Value > expirationTime)
                    {
                        activeCopies.Remove(copy.Key);
                        Console.WriteLine($"Копия {copy.Key} отключилась.");
                    }
                }

                if (activeCopies.Count > 0)
                {
                    Console.WriteLine("Активные копии:");
                    foreach (var copy in activeCopies)
                    {
                        Console.WriteLine($"{copy.Key}");
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}