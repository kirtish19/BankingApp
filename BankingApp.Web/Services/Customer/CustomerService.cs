using BankingApp.Web.Constants;
using BankingApp.Web.Models.Registration;
using System.Globalization;
using System.Net.Http.Headers;

namespace BankingApp.Web.Services.Customer;

public class CustomerService(
    HttpClient httpClient) : ICustomerService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<bool> RegisterAsync(
        RegistrationRequest request)
    {
        using var formData = new MultipartFormDataContent();

        // =========================================
        // Common User Information
        // =========================================

        formData.Add(
            new StringContent(request.UserName),
            nameof(request.UserName));

        formData.Add(
            new StringContent(request.LoginPassword),
            nameof(request.LoginPassword));

        formData.Add(
            new StringContent(request.ProfilePassword),
            nameof(request.ProfilePassword));

        formData.Add(
            new StringContent(request.UserType.ToString()),
            nameof(request.UserType));


        // =========================================
        // Customer Information
        // =========================================

        if (request.UserType == UserType.Customer)
        {
            formData.Add(
                new StringContent(request.FirstName),
                nameof(request.FirstName));

            formData.Add(
                new StringContent(request.LastName),
                nameof(request.LastName));

            formData.Add(
                new StringContent(request.Email),
                nameof(request.Email));

            formData.Add(
                new StringContent(request.MobileNumber),
                nameof(request.MobileNumber));

            formData.Add(
                new StringContent(
                    request.EmploymentType.ToString()),
                nameof(request.EmploymentType));

            formData.Add(
                new StringContent(
                    request.AnnualIncome.ToString(
                        CultureInfo.InvariantCulture)),
                nameof(request.AnnualIncome));

            formData.Add(
                new StringContent(
                    request.CreditScore.ToString(
                        CultureInfo.InvariantCulture)),
                nameof(request.CreditScore));

            if (request.DateOfBirth.HasValue)
            {
                formData.Add(
                    new StringContent(
                        request.DateOfBirth.Value.ToString(
                            "yyyy-MM-dd",
                            CultureInfo.InvariantCulture)),
                    nameof(request.DateOfBirth));
            }


            // =========================================
            // KYC Documents
            // =========================================

            foreach (var file in request.KycDocuments)
            {
                var stream = file.OpenReadStream(
                    maxAllowedSize: 10 * 1024 * 1024);

                var fileContent =
                    new StreamContent(stream);

                fileContent.Headers.ContentType =
                    new MediaTypeHeaderValue(
                        file.ContentType);

                formData.Add(
                    fileContent,
                    nameof(request.KycDocuments),
                    file.Name);
            }
        }


        // =========================================
        // Call Customer API
        // =========================================

        var response = await _httpClient.PostAsync(
            "api/User/Register",
            formData);


        // =========================================
        // Error Handling
        // =========================================

        if (!response.IsSuccessStatusCode)
        {
            var error =
                await response.Content.ReadAsStringAsync();

            Console.WriteLine(
                $"Registration failed. " +
                $"Status: {(int)response.StatusCode}");

            Console.WriteLine(
                $"API Response: {error}");
        }

        return response.IsSuccessStatusCode;
    }
}

