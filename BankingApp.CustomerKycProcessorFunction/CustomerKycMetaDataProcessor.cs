namespace BankingApp.CustomerKycProcessorFunction;

public class CustomerKycMetaDataProcessor
{
    private readonly ILogger<CustomerKycMetaDataProcessor> _logger;
    private readonly IMetaDataProcessorService _metaDataProcessorService;

    public CustomerKycMetaDataProcessor(ILogger<CustomerKycMetaDataProcessor> logger, IMetaDataProcessorService metaDataProcessorService)
    {
        _logger = logger;
        _metaDataProcessorService = metaDataProcessorService;
    }

    // Use configuration placeholders in the attribute when possible (resolved from environment or app settings)
    [Function(nameof(CustomerKycMetaDataProcessor))]
    public async Task Run(
        [ServiceBusTrigger("%KycTopicName%", "%KycSubscriptionName%", Connection = "ServiceBusReader", AutoCompleteMessages = false)]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        var options = new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve };
        var documentMessage = JsonSerializer.Deserialize<CustomerKYCMessage>(message.Body, options);
        await _metaDataProcessorService.ProcessMetaData(documentMessage!);
        // Complete the message
        await messageActions.CompleteMessageAsync(message);
    }
}