namespace BankingApp.CustomerLoanProcessorFunction;

public class LoanAssessmentFunction
{
    private readonly ILogger<LoanAssessmentFunction> _logger;
    private readonly ILoanAssessmentService _loanAssessmentService;

    public LoanAssessmentFunction(ILogger<LoanAssessmentFunction> logger, ILoanAssessmentService loanAssessmentService)
    {
        _logger = logger;
        _loanAssessmentService = loanAssessmentService;
    }

    [Function(nameof(LoanAssessmentFunction))]
    public async Task Run(
        [ServiceBusTrigger("%LoanQueueName%", Connection = "ServiceBusReader", AutoCompleteMessages = false)]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        var loanApplicationMessage = JsonSerializer.Deserialize<LoanApplicationMessage>(message.Body);
        await _loanAssessmentService.ProcessLoanApplication(loanApplicationMessage!);
        await messageActions.CompleteMessageAsync(message);
    }
}