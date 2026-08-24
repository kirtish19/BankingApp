namespace BankingApp.Shared.Helpers
{
    public interface IServiceBusHandler<T> where T : class
    {
        public Task SendMessageToQueueOrTopic(T messsage, string topicName, string connectionString);
    }
}
