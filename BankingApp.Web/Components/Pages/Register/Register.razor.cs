using BankingApp.Web.Constants;
using BankingApp.Web.Models.Customer;
using BankingApp.Web.Models.Registration;
using BankingApp.Web.Services.Customer;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BankingApp.Web.Components.Pages.Register;

public partial class Register
{
    [Inject]
    private ICustomerService CustomerService { get; set; } = default!;

    private RegistrationRequest registrationRequest = new();

    private EditContext editContext = default!;

    private IBrowserFile? selectedKycFile;

    private string? kycErrorMessage;

    private string? errorMessage;

    private bool isSubmitting;

    private bool registrationSuccessful;


    protected override void OnInitialized()
    {
        editContext =
            new EditContext(registrationRequest);
    }


    // =========================================
    // User Type Selection
    // =========================================

    private void SelectCustomer(ChangeEventArgs args)
    {
        registrationRequest =
            new RegistrationRequest
            {
                UserType = UserType.Customer
            };

        editContext =
            new EditContext(registrationRequest);

        ClearMessages();
    }


    private void SelectStaff(ChangeEventArgs args)
    {
        registrationRequest =
            new RegistrationRequest
            {
                UserType = UserType.Staff
            };

        editContext =
            new EditContext(registrationRequest);

        ClearMessages();
    }


    // =========================================
    // Registration
    // =========================================


    private async Task HandleSubmit()
    {
        errorMessage = null;

        registrationSuccessful = false;

        isSubmitting = true;

        try
        {
            var result =
                await CustomerService.RegisterAsync(
                    registrationRequest);


            if (result)
            {
                registrationSuccessful = true;

                var registeredUserType =
                    registrationRequest.UserType;

                // =========================================
                // Reset Registration Form
                // Keep Selected Account Type
                // =========================================

                registrationRequest =
                    new RegistrationRequest
                    {
                        UserType = registeredUserType
                    };

                editContext =
                    new EditContext(registrationRequest);

                selectedKycFile = null;

                kycErrorMessage = null;
            }


        }
        catch (HttpRequestException)
        {
            errorMessage =
                "Unable to connect to the banking service.";
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Registration exception: {ex}");

            errorMessage =
                ex.Message;
        }
        finally
        {
            isSubmitting = false;
        }
    }




    // =========================================
    // KYC File Selection
    // =========================================

    private void HandleKycFileSelected(
        InputFileChangeEventArgs e)
    {
        kycErrorMessage = null;

        selectedKycFile = e.File;

        if (selectedKycFile is null)
        {
            return;
        }

        const long maxFileSize =
            10 * 1024 * 1024;

        if (selectedKycFile.Size > maxFileSize)
        {
            kycErrorMessage =
                "File size cannot exceed 10 MB.";

            selectedKycFile = null;
        }
    }


    // =========================================
    // Add KYC Document
    // =========================================

    private async Task AddKycDocument()
    {
        kycErrorMessage = null;

        if (selectedKycFile is null)
        {
            kycErrorMessage =
                "Please select a document.";

            return;
        }


        // =========================================
        // Check Duplicate File
        // =========================================

        if (registrationRequest.KycDocuments.Any(
                x => x.Name.Equals(
                    selectedKycFile.Name,
                    StringComparison.OrdinalIgnoreCase)))
        {
            kycErrorMessage =
                "This document has already been added.";

            return;
        }


        // =========================================
        // Read File Immediately
        // =========================================

        const long maxFileSize =
            10 * 1024 * 1024;

        await using var stream =
            selectedKycFile.OpenReadStream(
                maxFileSize);

        using var memoryStream =
            new MemoryStream();

        await stream.CopyToAsync(
            memoryStream);


        // =========================================
        // Create KYC Document
        // =========================================

        var document = new KycDocument
        {
            Name = selectedKycFile.Name,

            ContentType =
                selectedKycFile.ContentType,

            Size =
                selectedKycFile.Size,

            Content =
                memoryStream.ToArray()
        };


        // =========================================
        // Add Document
        // =========================================

        registrationRequest.KycDocuments.Add(
            document);


        // =========================================
        // Clear Selected File
        // =========================================

        selectedKycFile = null;
    }


    // =========================================
    // Remove KYC Document
    // =========================================

    private void RemoveKycDocument(
        KycDocument document)
    {
        registrationRequest.KycDocuments.Remove(
            document);
    }


    // =========================================
    // Clear Messages
    // =========================================

    private void ClearMessages()
    {
        errorMessage = null;

        registrationSuccessful = false;

        kycErrorMessage = null;

        selectedKycFile = null;
    }


    // =========================================
    // Format File Size
    // =========================================

    private static string FormatFileSize(long bytes)
    {
        if (bytes < 1024)
        {
            return $"{bytes} B";
        }

        if (bytes < 1024 * 1024)
        {
            return $"{bytes / 1024.0:F1} KB";
        }

        return $"{bytes / (1024.0 * 1024.0):F1} MB";
    }


    // =========================================
    // Employment Display Name
    // =========================================

    private static string GetEmploymentDisplayName(
        EmploymentType employmentType)
    {
        return employmentType switch
        {
            EmploymentType.SelfEmployed =>
                "Self Employed",

            EmploymentType.Salaried =>
                "Salaried",

            EmploymentType.Unemployed =>
                "Unemployed",

            _ =>
                employmentType.ToString()
        };
    }
}