using System;
using System.Threading;

namespace DKSY.Natalie.CNSL
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Initializing...");
            NatalieAI AI = new NatalieAI();
            AI.Post += AI_Post;
            Console.WriteLine("Initialized.");
            AI.Start();
            while (AI.IsRunning) Thread.Sleep(5000);
            Console.WriteLine("Shutdown.");
        }

        private static void AI_Post(string message)
        {
            Console.WriteLine(message);
        }
    }
}
