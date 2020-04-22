using System;

namespace SendsafelyApi.Utilities
{
    internal class Logger
    {
        public static void Log(string msg)
        {
            var debug = false;

            if (debug)
            {
                Console.WriteLine(msg);
            }
        }
    }
}
