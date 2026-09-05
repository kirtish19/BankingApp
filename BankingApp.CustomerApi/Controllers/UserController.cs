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
            var loginResponseDto = await _userService.LoginUserAsync(request);

            if (!loginResponseDto.LoginSuccess)
            {
                return Unauthorized("Invalid username or password");
            }

            var token = await _userService.GetTokenAsync(loginResponseDto.User!);
            var response = new PostLoginResponse
            {
                Token = token,
                CustomerId = loginResponseDto.User!.Customer?.Id
            };
            return Ok(response);
        }
    }
}
