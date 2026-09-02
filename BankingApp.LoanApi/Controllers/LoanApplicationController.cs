namespace BankingApp.LoanApi.Controllers
{
    [Route("api/[controller]")]
    [Consumes("multipart/form-data")]
    [ApiController]
    public class LoanApplicationController(ILoanService loanService) : ControllerBase
    {
        private readonly ILoanService _loanService = loanService;

        [HttpPost("Submit")]
        public async Task<IActionResult> LoanApplicationSubmitAsync([FromForm] PostLoanApplicationRequest request)
        {
            await _loanService.LoanApplicationSubmitAsync(request);
            return Ok("Loan application submitted successfully");
        }

    }
}
