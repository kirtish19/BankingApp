using BankingApp.Web.Constants;
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
    IAuthStorageService authStorageService, IAccessTokenService accessTokenService) : IStaffLoanService
{
    private readonly HttpClient _httpClient = httpClient;

    private readonly IAccessTokenService _accessTokenService = accessTokenService;
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
    // GET CUSTOMER DETAILS
    // =========================================

    public async Task<CustomerDetailsDto?>
        GetCustomerDetailsAsync(Guid customerId)
    {
        var isSessionValid =
            await _authStorageService.IsSessionValidAsync();

        Console.WriteLine(
            $"[Staff API] Customer details session valid: {isSessionValid}");

        if (!isSessionValid)
        {
            Console.WriteLine(
                "[Staff API] Customer details session is invalid.");

            return null;
        }

        var token =
            await _accessTokenService.GetAccessTokenAsync();

        if (string.IsNullOrWhiteSpace(token))
        {
            Console.WriteLine(
                "[Staff API] Customer details token is missing.");

            return null;
        }

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);

        var url =
            $"user/api/User/GetCustomerDetails/{customerId}";

        Console.WriteLine(
            $"[Staff API] Calling customer details API: {url}");

        var response =
            await _httpClient.GetAsync(url);

        Console.WriteLine(
            $"[Staff API] Customer details status: " +
            $"{(int)response.StatusCode} {response.StatusCode}");

        var responseContent =
            await response.Content.ReadAsStringAsync();

        Console.WriteLine(
            $"[Staff API] Customer details response: {responseContent}");

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine(
                "[Staff API] Failed to get customer details.");

            return null;
        }

        return JsonSerializer.Deserialize<CustomerDetailsDto>(
            responseContent,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
    }


    // =========================================
    // UPDATE LOAN STATUS
    // =========================================

    public async Task<bool> UpdateLoanStatusAsync(
      Guid loanId,
      Guid customerId,
      LoanStatus status,
      string statusDescription,
      string reviewComments)
    {
        try
        {
            Console.WriteLine();
            Console.WriteLine(
                "==================================================");

            Console.WriteLine(
                "[Staff API] UPDATE LOAN STATUS STARTED");

            Console.WriteLine(
                "==================================================");


            // =========================================
            // 1. GET CUSTOMER DETAILS
            // =========================================

            Console.WriteLine(
                $"[Staff API] Customer ID: {customerId}");

            Console.WriteLine(
                "[Staff API] Getting customer details...");

            var customerDetails =
                await GetCustomerDetailsAsync(customerId);

            if (customerDetails is null)
            {
                Console.WriteLine(
                    "[Staff API] Customer details not found.");

                return false;
            }


            // =========================================
            // 2. GET FULL NAME AND EMAIL
            // =========================================

            var fullName =
                $"{customerDetails.FirstName} {customerDetails.LastName}"
                    .Trim();

            var email =
                customerDetails.Email;

            Console.WriteLine(
                $"[Staff API] Customer FullName: {fullName}");

            Console.WriteLine(
                $"[Staff API] Customer Email: {email}");


            // =========================================
            // 3. CREATE APIM REQUEST BODY
            // =========================================
            //
            // IMPORTANT:
            // LoanStatus remains an enum in C#.
            // Only the JSON value sent to APIM is int.
            //

            var requestBody = new
            {
                loanId = loanId,

                status = (int)status,

                statusDescription = statusDescription,

                email = email,

                fullName = fullName,

                reviewComments = reviewComments
            };


            // =========================================
            // 4. CREATE APIM URL
            // =========================================

            var apimBaseUrl =
                _httpClient.BaseAddress?.ToString()
                ?? string.Empty;

            var url =
                $"{apimBaseUrl}UpdateLoanStatus/paths/invoke";

            Console.WriteLine(
                $"[Staff API] PATCH URL: {url}");


            // =========================================
            // 5. SERIALIZE REQUEST
            // =========================================

            var json =
                JsonSerializer.Serialize(requestBody);

            Console.WriteLine(
                $"[Staff API] PATCH Request: {json}");


            // =========================================
            // 6. CREATE PATCH REQUEST
            // =========================================

            using var client =
               new HttpClient();
            using var request =
                new HttpRequestMessage(
                    HttpMethod.Patch,
                    url);

            request.Content =
                new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json");


            // =========================================
            // 7. SEND PATCH REQUEST
            // =========================================

            Console.WriteLine(
                "[Staff API] Sending PATCH request...");

            var response =
                await client.SendAsync(request);


            // =========================================
            // 8. READ RESPONSE
            // =========================================

            var responseContent =
                await response.Content.ReadAsStringAsync();

            Console.WriteLine(
                $"[Staff API] Status Code: " +
                $"{(int)response.StatusCode} {response.StatusCode}");

            Console.WriteLine(
                "[Staff API] Response:");

            Console.WriteLine(
                responseContent);


            // =========================================
            // 9. CHECK RESPONSE
            // =========================================

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine();
                Console.WriteLine(
                    "[Staff API] PATCH FAILED.");

                return false;
            }

            Console.WriteLine(
                "[Staff API] PATCH SUCCESSFUL.");

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine(
                "[Staff API] UPDATE LOAN STATUS ERROR.");

            Console.WriteLine(
                $"Message: {ex.Message}");

            Console.WriteLine(
                $"Details: {ex}");

            return false;
        }
    }

    // =========================================
    // GET LOAN DOCUMENTS
    // =========================================

    public async Task<List<LoanDocumentsDto>> GetLoanDocumentsAsync(
        Guid loanId)
    {
        var isSessionValid =
            await _authStorageService.IsSessionValidAsync();

        Console.WriteLine(
            $"[Staff API] Loan documents session valid: {isSessionValid}");

        if (!isSessionValid)
        {
            Console.WriteLine(
                "[Staff API] Loan documents session is invalid.");

            return [];
        }

        var token =
            await _authStorageService.GetTokenAsync();

        if (string.IsNullOrWhiteSpace(token))
        {
            Console.WriteLine(
                "[Staff API] Loan documents token is missing.");

            return [];
        }

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);

        var url =
            $"loan/api/LoanApplication/GetLoanDocuments/{loanId}";

        Console.WriteLine(
            $"[Staff API] Calling loan documents API: {url}");

        var response =
            await _httpClient.GetAsync(url);

        Console.WriteLine(
            $"[Staff API] Loan documents status: " +
            $"{(int)response.StatusCode} {response.StatusCode}");

        var responseContent =
            await response.Content.ReadAsStringAsync();

        Console.WriteLine(
            $"[Staff API] Loan documents response: " +
            $"{responseContent}");

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine(
                "[Staff API] Failed to get loan documents.");

            return [];
        }

        var documents =
            JsonSerializer.Deserialize<List<LoanDocumentsDto>>(
                responseContent,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        return documents ?? [];
    }

    // =========================================
    // GET KYC DOCUMENTS
    // =========================================

    public async Task<List<KycDocumentDto>> GetKycDocumentsAsync(
        Guid customerId)
    {
        try
        {
            Console.WriteLine(
                $"[Staff API] Getting KYC documents for customer: {customerId}");

            // KYC API uses a different token.
            var token =
                await _accessTokenService.GetAccessTokenAsync();

            if (string.IsNullOrWhiteSpace(token))
            {
                Console.WriteLine(
                    "[Staff API] KYC access token is missing.");

                return [];
            }

            // Set Authorization header
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token);

            var url =
                $"user/api/User/GetKycDocuments/{customerId}";

            Console.WriteLine(
                $"[Staff API] Calling KYC documents API: {url}");

            var response =
                await _httpClient.GetAsync(url);

            Console.WriteLine(
                $"[Staff API] KYC documents status: " +
                $"{(int)response.StatusCode} {response.StatusCode}");

            var responseContent =
                await response.Content.ReadAsStringAsync();

            Console.WriteLine(
                $"[Staff API] KYC documents response: " +
                $"{responseContent}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(
                    "[Staff API] Failed to get KYC documents.");

                return [];
            }

            var documents =
                JsonSerializer.Deserialize<List<KycDocumentDto>>(
                    responseContent,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            Console.WriteLine(
                $"[Staff API] KYC documents received: " +
                $"{documents?.Count ?? 0}");

            return documents ?? [];
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[Staff API] Error getting KYC documents: {ex}");

            return [];
        }
    }

}

