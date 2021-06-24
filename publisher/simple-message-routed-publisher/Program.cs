using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Simple_message_routed_publisher
{
    internal class Program
    {
        public static ManualResetEvent Handle { get; } = new ManualResetEvent(false);

        private static async Task Main(string[] args)
        {
            var publisher = new Simple_message_routed_publisher();

            await publisher.Run();

            // CLI
            Console.CancelKeyPress += (s, e) =>
            {
                Handle.Set();
                e.Cancel = true;
            };

            Handle.WaitOne();
        }
    }
}