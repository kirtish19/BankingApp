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

        [HttpGet()]
        public async Task<IActionResult> GetAllAsync()
        {
            var loanApplications = await _loanService.GetAllLoanApplications();
            return Ok(loanApplications);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLoanApplicationAsync(Guid id)
        {
            var loanApplication = await _loanService.GetLoanApplicationById(id);
            if (loanApplication is null) return NotFound("Loan application not found");
            return Ok(loanApplication);
        }
    }
}
