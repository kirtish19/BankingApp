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
            return Ok("User registered successfully");
        }

        [Consumes("application/json")]
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] PostLoginRequest request)
        {
            await _userService.LoginUserAsync(request);
            return Ok("User logged in successfully"); //TODO - return JWT token instead of success message
        }
    }
}
