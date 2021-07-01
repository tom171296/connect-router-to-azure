using Amqp;
using Amqp.Framing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Simple_message_routed_publisher
{
    public class Simple_message_routed_publisher
    {
        internal async Task Run()
        {
            // The publisher needs an address where it can publish to.
            var publishAddress = "pubsub/your/own/address/here";

            // We need to define the address of our edge-router which to where the publisher should connect.
            var addressString = Environment.GetEnvironmentVariable("ROUTER_ADDRESS");
            var edgeRouterAddress = new Address($"amqp://{addressString}:5672");

            // The application needs to create an AMQP(s) connection via the connection factory.
            var connectionFactory = new ConnectionFactory();

            connectionFactory.AMQP.HostName = "simple-message-routed-publisher";
            connectionFactory.AMQP.ContainerId = "amq.topic";

            // To handle a missing receiver for a short time (no credits available to send a message)
            // AMQPNetLite uses an in memory buffer. This buffer can keep all messages that are not send yet,
            // but that will take up a lot of memory. To make this functionality helpfull you can create your own buffer.
            // The buffer now can contain a max amount of messages. Each message that comes in more will result in releasing
            // the message that is longest in the buffer (following the FIFO principal).
            connectionFactory.BufferManager = new BufferManager(64, 1024 * 1024, 50 * 1024 * 1024);

            // Create the connection
            var connection = await connectionFactory.CreateAsync(edgeRouterAddress);

            // Create a new session.
            var session = new Session(connection);

            // Create the sender: new Senderlink("session", senderlinkName, defaultPublishAddress);
            // The address is settable per message that is send.
            var sender = new SenderLink(session, "ISKA26-5", publishAddress);

            // Create the AMQP message that is send.
            // The body should always contain a string value, for example JSON.
            var amqpMessage = new Message(DateTimeOffset.Now + "Sending from the other side of the network")
            {
                // Define the message header.
                Header = new Header
                {
                    // Duration in miliseconds that the message can flow over the infrastructure, this value should be considered per message type.
                    Ttl = 1
                },
                // Define the message properties.
                Properties = new Properties
                {
                    MessageId = Guid.NewGuid().ToString(),

                    // The addres to which the message is being published.
                    To = publishAddress,

                    // Logical queue id, for example personnel number or Train number
                    Subject = "999900",

                    // Time that can be used by the consumer to determine if the message is still valid and needs to be processed.
                    AbsoluteExpiryTime = DateTime.UtcNow.AddMinutes(1)
                }
            };

            // Now we have to implicitly define the Quality of Service that we want to send or message with.
            // To send a message with the Quality of Service At-least-once we have to define an outcome callback.
            var outcomeCallback = new OutcomeCallback(OutcomeCallback);

            // Then we send the message via the send method where you have to use the callback method.
            // You can also use the following call sender.Send(amqpMessage), this will result in a Quality of service of At-most-once,
            // because the application is not waiting for an acknowledgement to handle.
            Thread.Sleep(TimeSpan.FromSeconds(5));
            Console.WriteLine("Sending message");
            for (int i = 0; i < 10000; i++)
            {
                Console.WriteLine("Sending message");
                sender.Send(amqpMessage, outcomeCallback, null);
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
            }
        }

        /// <summary>
        /// Callback method that is invoked when the AMQP infrastructure returns an outcome for a certain message.
        /// </summary>
        /// <param name="link">The <see cref="SenderLink"/> that is used to send the message.</param>
        /// <param name="message">The <see cref="Message"/> that is send.</param>
        /// <param name="outcome">The <see cref="Outcome"/> of the transport.</param>
        /// <param name="_">Some object that can be passed through via the send method.</param>
        private void OutcomeCallback(ILink link, Message message, Outcome outcome, object _)
        {
            // The outcome will define what happened to the message
            // Accepted: Indicates succesful processing at the receiver(s).
            // Rejected: Indicates an invalid and unprocessable message.
            // Released: Indicates that the message was not (and will not be) processed.
            // Modified: Indicates that the message was modified, but not processed.
            // Received: Indicates partial message data seen by the receiver as well as the starting point for a resumed transfer.
        }
    }
}