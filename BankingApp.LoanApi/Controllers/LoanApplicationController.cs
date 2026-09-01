using BankingApp.LoanApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankingApp.LoanApi.Controllers
{
    [Route("api/[controller]")]
    [Consumes("multipart/form-data")]
    [ApiController]
    public class LoanApplicationController : ControllerBase
    {
        [HttpPost("Submit")]
        public async Task<IActionResult> LoanApplicationSubmitAsync([FromForm] PostLoanApplicationRequest request)
        {
            return Ok("User registered successfully");
        }

    }
}
