using Azure.Messaging.ServiceBus;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BankingApp.Shared.Helpers
{
    public class ServiceBusHandler : IServiceBusHandler
    {
        //TODO - add logic to custom add the additonal properties
        public async Task SendMessageToQueueOrTopic(object message, string queueOrTopicName, string connectionString)
        {
            await using var client = new ServiceBusClient(connectionString);

            ServiceBusSender sender = client.CreateSender(queueOrTopicName);
            var options = new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve };
            string jsonMessage = JsonSerializer.Serialize(message, options);
            ServiceBusMessage serviceBusMessage = new ServiceBusMessage(jsonMessage);
            serviceBusMessage.MessageId = Guid.NewGuid().ToString();
            await sender.SendMessageAsync(serviceBusMessage);
        }
    }
}
