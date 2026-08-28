namespace BankingApp.Shared.Helpers
{
    public interface IServiceBusHandler
    {
        // Generic method allowing callers to pass strongly-typed message payloads
        // and optional application properties to be added to the Service Bus message.
        Task SendMessageToQueueOrTopic<T>(T messsage, string topicName, string connectionString, IDictionary<string, object>? applicationProperties = null);
    }
}
