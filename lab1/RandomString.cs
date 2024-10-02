using System;

namespace lab1
{
    public class RandomString
    {
        public static string getRandomString(int length) 
        {
            string pool = "9876543210";
            string tmp = "";
            Random R = new Random();
            for (int x = 0; x < length; x++)
            {
                tmp += pool[R.Next(0, pool.Length)].ToString();
            }
            return tmp;
        }
    }
}