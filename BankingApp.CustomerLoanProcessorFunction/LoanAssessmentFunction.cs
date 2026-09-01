using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BankingApp.CustomerLoanProcessorFunction;

public class LoanAssessmentFunction
{
    private readonly ILogger<LoanAssessmentFunction> _logger;

    public LoanAssessmentFunction(ILogger<LoanAssessmentFunction> logger)
    {
        _logger = logger;
    }

    [Function(nameof(LoanAssessmentFunction))]
    public async Task Run(
        [ServiceBusTrigger("%LoanQueueName%", Connection = "ServiceBusReader")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        _logger.LogInformation("Message ID: {id}", message.MessageId);
        _logger.LogInformation("Message Body: {body}", message.Body);
        _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

        // Complete the message
        await messageActions.CompleteMessageAsync(message);
    }
}