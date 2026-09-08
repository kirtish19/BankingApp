using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Http;

namespace BankingApp.CustomerLoanProcessorFunction;

public class ManualApprovalLoanProcessingFunction
{
    private readonly ILogger<ManualApprovalLoanProcessingFunction> _logger;
    private readonly ILoanAssessmentService _loanAssessmentService;


    public ManualApprovalLoanProcessingFunction(ILogger<ManualApprovalLoanProcessingFunction> logger, ILoanAssessmentService loanAssessmentService)
    {
        _logger = logger;
        _loanAssessmentService = loanAssessmentService;
    }

    [Function("ManualApprovalLoanProcessingFunction")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "patch")] HttpRequest req)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var requestData = JsonSerializer.Deserialize<UpdateLoanStatusRequest>(requestBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        await _loanAssessmentService.ManualLoanProcess(requestData);
        var response = new OkObjectResult("Loan updated successfully.");
        return response;
    }
}