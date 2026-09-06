using BankingApp.Web.Models.Loan;
using BankingApp.Web.Models.Staff;
using BankingApp.Web.Services.Authentication;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace BankingApp.Web.Services.Staff;

public class StaffLoanService(
    HttpClient httpClient,
    IAuthStorageService authStorageService) : IStaffLoanService
{
    private readonly HttpClient _httpClient = httpClient;

    private readonly IAuthStorageService _authStorageService =
        authStorageService;


    // =========================================
    // GET ALL LOAN APPLICATIONS
    // =========================================

    public async Task<List<LoanApplicationsDto>> GetAllLoansAsync()
    {
        var isSessionValid =
            await _authStorageService.IsSessionValidAsync();

        Console.WriteLine(
            $"Staff API - Session valid: {isSessionValid}");

        if (!isSessionValid)
        {
            Console.WriteLine(
                "Staff API - Session is invalid.");

            return [];
        }

        var token =
            await _authStorageService.GetTokenAsync();

        Console.WriteLine(
            $"Staff API - Token exists: {!string.IsNullOrWhiteSpace(token)}");

        if (string.IsNullOrWhiteSpace(token))
        {
            Console.WriteLine(
                "Staff API - Token is missing.");

            return [];
        }

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);

        Console.WriteLine(
            $"Staff API - Calling: {_httpClient.BaseAddress}loan/api/LoanApplication");

        var response =
            await _httpClient.GetAsync(
                "loan/api/LoanApplication");

        Console.WriteLine(
            $"Staff API - Status: {(int)response.StatusCode} {response.StatusCode}");

        var responseContent =
            await response.Content.ReadAsStringAsync();

        Console.WriteLine(
            $"Staff API - Response: {responseContent}");

        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        var loans =
            JsonSerializer.Deserialize<
                List<LoanApplicationsDto>>(
                    responseContent,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

        return loans ?? [];
    }


    // =========================================
    // GET PENDING LOAN APPLICATIONS
    // =========================================

    public async Task<List<LoanApplicationsDto>> GetPendingLoansAsync()
    {
        var isSessionValid =
            await _authStorageService.IsSessionValidAsync();

        if (!isSessionValid)
        {
            return [];
        }

        var token =
            await _authStorageService.GetTokenAsync();

        if (string.IsNullOrWhiteSpace(token))
        {
            return [];
        }

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);

        var response =
            await _httpClient.GetAsync(
                "loan/api/LoanApplication/GetPendingLoans");

        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        var loans =
            await response.Content
                .ReadFromJsonAsync<List<LoanApplicationsDto>>();

        return loans ?? [];
    }


    // =========================================
    // GET LOAN APPLICATION BY ID
    // =========================================

    public async Task<LoanApplicationsDto?> GetLoanByIdAsync(
        Guid loanId)
    {
        var isSessionValid =
            await _authStorageService.IsSessionValidAsync();

        if (!isSessionValid)
        {
            return null;
        }

        var token =
            await _authStorageService.GetTokenAsync();

        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);

        var response =
            await _httpClient.GetAsync(
                $"loan/api/LoanApplication/{loanId}");

        if (response.StatusCode ==
            HttpStatusCode.NotFound)
        {
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content
            .ReadFromJsonAsync<LoanApplicationsDto>();
    }


    // =========================================
    // UPDATE LOAN STATUS
    // =========================================

    public async Task<bool> UpdateLoanStatusAsync(
     Guid loanId,
     string status,
     string statusDescription,
     string email,
     string fullName,
     string reviewComments)
    {
        var apimBaseUrl = _httpClient.BaseAddress?.ToString() ?? "";
        string url = $"{apimBaseUrl}UpdateLoanStatus/paths/invoke";

        var requestBody =
            new UpdateLoanStatusRequest
            {
                LoanId = loanId,
                Status = status,
                StatusDescription = statusDescription,
                Email = email,
                FullName = fullName,
                ReviewComments = reviewComments
            };

        using var client = new HttpClient();


        string json = JsonSerializer.Serialize(requestBody);

        var request = new HttpRequestMessage(
            HttpMethod.Patch,
            url);

        request.Content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        HttpResponseMessage response = await client.SendAsync(request);

        string responseContent = await response.Content.ReadAsStringAsync();

        Console.WriteLine($"Status Code: {(int)response.StatusCode}");
        Console.WriteLine("Response:");
        Console.WriteLine(responseContent);
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine();
            Console.WriteLine(
                "[Staff API] PATCH FAILED.");

            return false;
        }
        return true;

        
    }

}