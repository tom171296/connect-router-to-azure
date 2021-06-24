using Amqp;
using Amqp.Framing;
using Amqp.Types;
using System;
using System.Threading.Tasks;

namespace simple_message_routed_subscriber
{
    internal class Simple_message_routed_subscriber
    {
        internal async Task Subscribe()
        {
           // The subscriber needs an address where it can subscribe to.
            // The address is build up following the guidelines on the wiki!
            // https://dev.azure.com/ns-topaas/CORTEX/_wiki/wikis/CORTEX.wiki/3485/Adresnaamgeving
            // It should at least contain the predefined prefix 'pubsub' followed by your application name.
            var amqpAddress = "pubsub.from.asb.to.internal";

            // We need to define the address of our edge-router which to where the subscriber should connect.
             var addressString = Environment.GetEnvironmentVariable("ROUTER_ADDRESS");
            var edgeRouterAddress = new Address($"amqp://{addressString}:5672");

            // The application needs to create an AMQP(s) connection via the connection factory.
            var connectionFactory = new ConnectionFactory();

            // Create the connection
            var connection = await connectionFactory.CreateAsync(edgeRouterAddress);

            // Create a new session.
            var session = new Session(connection);

            // Create a receiver link and let the link know to which AMQP address it should listen.
            var receiverLink = new ReceiverLink(session, "LinkName", amqpAddress);

            // Start receiving messages
            receiverLink.Start(1, HandleNewMessage(receiverLink));
        }

        private MessageCallback HandleNewMessage(ReceiverLink receiverLink)
        {
            return (link, message) =>
            {
                // Do something with the message
                Console.WriteLine(message.Body);

                // When a message is received the quality of service determines what the subscriber should let the publisher now.
                // If the QoS is at-most-once, the publisher doesn't expect any acknowledgement. But when the QoS is higher then at-most-once
                // there should be an acknowledgement giving to the publisher.

                // If the message is handled, the subscriber should let the publisher know that it's handled by sending acknowledgement.
                receiverLink.Accept(message);

                // If the message isn't handled because of an exceptio, it is possible to reject the message
                receiverLink.Reject(message, new Error(new Symbol("Error message"))
                {
                    Description = "Error description"
                });

                // If a message shouldn't be handled by the subscriber, an release can be given to the publisher. So it doesn't wait for acceptance.
                receiverLink.Release(message);
            };
        }
    }
}