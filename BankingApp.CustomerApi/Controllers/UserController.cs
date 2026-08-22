namespace BankingApp.CustomerApi.Controllers
{
    [Route("api/[controller]")]
    [Consumes("multipart/form-data")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUserAsync([FromForm] PostUserRegisterationRequest request)
        {
            await _userService.RegisterUserAsync(request);
            return Ok("");
        }
    }
}
