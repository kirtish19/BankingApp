using BankingApp.Web.Models.Loan;
using BankingApp.Web.Services.Authentication;
using System.Globalization;
using System.Net.Http.Headers;

namespace BankingApp.Web.Services.Loan;

public class LoanService(
    HttpClient httpClient,
    IAuthStorageService authStorageService) : ILoanService
{
    private readonly HttpClient _httpClient = httpClient;

    private readonly IAuthStorageService _authStorageService =
        authStorageService;


    public async Task<bool> SubmitLoanApplicationAsync(
        LoanApplicationRequest request)
    {
        // Check 10-minute frontend session.
        var isSessionValid =
            await _authStorageService.IsSessionValidAsync();

        if (!isSessionValid)
        {
            Console.WriteLine(
                "Loan API: Frontend session expired.");

            await _authStorageService.ClearAuthenticationAsync();

            return false;
        }


        // Get JWT from browser localStorage.
        var token =
            await _authStorageService.GetTokenAsync();


        // Get CustomerId from browser localStorage.
        var customerId =
            await _authStorageService.GetCustomerIdAsync();


        if (string.IsNullOrWhiteSpace(token))
        {
            Console.WriteLine(
                "Loan API: Business JWT is missing.");

            return false;
        }


        if (customerId is null)
        {
            Console.WriteLine(
                "Loan API: CustomerId is missing.");

            return false;
        }


        Console.WriteLine(
            $"Loan API: JWT found = " +
            $"{!string.IsNullOrWhiteSpace(token)}");

        Console.WriteLine(
            $"Loan API: CustomerId = " +
            $"{customerId}");


        using var formData =
            new MultipartFormDataContent();


        formData.Add(
            new StringContent(
                customerId.Value.ToString()),
            "CustomerId");


        formData.Add(
            new StringContent(
                request.LoanType.ToString()),
            "LoanType");


        formData.Add(
            new StringContent(
                request.LoanAmount.ToString(
                    CultureInfo.InvariantCulture)),
            "LoanAmount");


        formData.Add(
            new StringContent(
                request.TenureMonths.ToString(
                    CultureInfo.InvariantCulture)),
            "TenureMonths");


        AddFile(
            formData,
            request.SalarySlip,
            "SalarySlip");


        AddFile(
            formData,
            request.BankStatement,
            "BankStatement");


        AddFile(
            formData,
            request.EmploymentLetter,
            "EmploymentLetter");


        // Set business JWT.
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);


        var response =
            await _httpClient.PostAsync(
                "loan/api/LoanApplication/Submit",
                formData);


        var responseBody =
            await response.Content
                .ReadAsStringAsync();


        Console.WriteLine(
            $"Loan API Status Code: " +
            $"{(int)response.StatusCode}");


        Console.WriteLine(
            $"Loan API Response: " +
            $"{responseBody}");


        if (!response.IsSuccessStatusCode)
        {
            return false;
        }


        return true;
    }

    public async Task<List<LoanApplicationsDto>> GetLoansForCustomerAsync(
    Guid customerId)
    {
        var isSessionValid =
            await _authStorageService.IsSessionValidAsync();

        if (!isSessionValid)
        {
            Console.WriteLine(
                "Loan API: Frontend session expired.");

            await _authStorageService.ClearAuthenticationAsync();

            return [];
        }

        var token =
            await _authStorageService.GetTokenAsync();

        if (string.IsNullOrWhiteSpace(token))
        {
            Console.WriteLine(
                "Loan API: Token is missing.");

            return [];
        }

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);

        var response =
            await _httpClient.GetAsync(
                $"loan/api/LoanApplication/GetLoansForCustomer/{customerId}");

        var responseBody =
            await response.Content.ReadAsStringAsync();

        Console.WriteLine(
            $"Loan API Get Loans Status Code: {(int)response.StatusCode}");

        Console.WriteLine(
            $"Loan API Get Loans Response: {responseBody}");

        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        var loanApplications =
            await response.Content
                .ReadFromJsonAsync<List<LoanApplicationsDto>>();

        return loanApplications ?? [];
    }

    private static void AddFile(
        MultipartFormDataContent formData,
        LoanDocument? document,
        string fieldName)
    {
        if (document is null)
        {
            return;
        }


        var content =
            new ByteArrayContent(
                document.Content);


        content.Headers.ContentType =
            new MediaTypeHeaderValue(
                document.ContentType);


        formData.Add(
            content,
            fieldName,
            document.Name);
    }
}