
using BankingApp.Web.Constants;
using BankingApp.Web.Models.Authentication;

namespace BankingApp.Web.Services.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private bool _isAuthenticated;
    private UserType? _currentUserType;
    private string? _currentUserName;

    public bool IsAuthenticated => _isAuthenticated;

    public UserType? CurrentUserType => _currentUserType;

    public string? CurrentUserName => _currentUserName;


    public async Task<LoginResult> LoginAsync(LoginRequest request)
    {
        await Task.Delay(300);

        // ==============================
        // TEMPORARY STAFF LOGIN
        // ==============================
        var username = request.UserName;
        var password = request.Password;
        if (request.UserName.Equals(
                "staff",
                StringComparison.OrdinalIgnoreCase)
            && request.Password == "Staff@123")
        {
            _isAuthenticated = true;
            _currentUserType = UserType.Staff;
            _currentUserName = request.UserName;

            return new LoginResult
            {
                IsAuthenticated = true,
                IsActive = true,
                UserType = UserType.Staff,
                UserName = request.UserName
            };
        }


        // ==============================
        // TEMPORARY CUSTOMER LOGIN
        // ==============================

        if (request.UserName.Equals(
                "customer",
                StringComparison.OrdinalIgnoreCase)
            && request.Password == "Customer@123")
        {
            _isAuthenticated = true;
            _currentUserType = UserType.Customer;
            _currentUserName = request.UserName;

            return new LoginResult
            {
                IsAuthenticated = true,
                IsActive = true,
                UserType = UserType.Customer,
                UserName = request.UserName
            };
        }


        // ==============================
        // INVALID LOGIN
        // ==============================

        return new LoginResult
        {
            IsAuthenticated = false,
            IsActive = false,
            ErrorMessage = "Invalid username or password."
        };
    }


    public Task LogoutAsync()
    {
        _isAuthenticated = false;
        _currentUserType = null;
        _currentUserName = null;

        return Task.CompletedTask;
    }
}

