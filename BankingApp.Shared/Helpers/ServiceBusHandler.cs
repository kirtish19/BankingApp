using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace BankingApp.Shared.Helpers
{
    public class ServiceBusHandler<T> : IServiceBusHandler<T> where T : class
    {
        //TODO - add logic to custom add the additonal properties
        public async Task SendMessageToQueueOrTopic(T message, string queueOrTopicName, string connectionString)
        {
            await using var client = new ServiceBusClient(connectionString);

            ServiceBusSender sender = client.CreateSender(queueOrTopicName);

            string jsonMessage = JsonSerializer.Serialize(message);
            ServiceBusMessage serviceBusMessage = new ServiceBusMessage(jsonMessage);
            serviceBusMessage.MessageId = Guid.NewGuid().ToString();
            await sender.SendMessageAsync(serviceBusMessage);
        }
    }
}
