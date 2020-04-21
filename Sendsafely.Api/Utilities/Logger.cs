using System;

namespace Sendsafely.Api.Utilities
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
