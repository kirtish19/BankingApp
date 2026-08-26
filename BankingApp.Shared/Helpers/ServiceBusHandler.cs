using Azure.Messaging.ServiceBus;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BankingApp.Shared.Helpers
{
    public class ServiceBusHandler : IServiceBusHandler
    {
        // Generic method to send messages with optional application properties
        public async Task SendMessageToQueueOrTopic<T>(T message, string queueOrTopicName, string connectionString, IDictionary<string, object>? applicationProperties = null)
        {
            await using var client = new ServiceBusClient(connectionString);

            ServiceBusSender sender = client.CreateSender(queueOrTopicName);
            var options = new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve };
            string jsonMessage = JsonSerializer.Serialize(message, options);
            ServiceBusMessage serviceBusMessage = new ServiceBusMessage(jsonMessage)
            {
                MessageId = Guid.NewGuid().ToString()
            };

            if (applicationProperties is not null)
            {
                foreach (var kvp in applicationProperties)
                {
                    // ApplicationProperties expects primitive types or types that are serializable to AMQP types
                    serviceBusMessage.ApplicationProperties[kvp.Key] = kvp.Value;
                }
            }

            await sender.SendMessageAsync(serviceBusMessage);
        }
    }
}
