namespace BankingApp.Shared.Helpers
{
    public interface IServiceBusHandler
    {
        public Task SendMessageToQueueOrTopic(object messsage, string topicName, string connectionString);
    }
}
