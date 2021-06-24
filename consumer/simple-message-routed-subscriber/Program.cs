using System;
using System.Threading;
using System.Threading.Tasks;

namespace simple_message_routed_subscriber
{
    internal class Program
    {
        public static ManualResetEvent Handle { get; } = new ManualResetEvent(false);

        private static async Task Main(string[] args)
        {
            var subscriber = new Simple_message_routed_subscriber();

            await subscriber.Subscribe();

            Console.WriteLine("Press Ctrl+C to stop the worker");
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                Handle.Set();
            };

            Handle.WaitOne();
        }
    }
}