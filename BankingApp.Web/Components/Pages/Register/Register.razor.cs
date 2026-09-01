using BankingApp.Web.Constants;
using BankingApp.Web.Models.Registration;
using BankingApp.Web.Services.Customer;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
namespace BankingApp.Web.Components.Pages.Register
{
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
            editContext = new EditContext(registrationRequest);
        }


        private void SelectCustomer(ChangeEventArgs args)
        {
            registrationRequest.UserType = UserType.Customer;

            ClearMessages();

            editContext = new EditContext(registrationRequest);
        }


        private void SelectStaff(ChangeEventArgs args)
        {
            registrationRequest.UserType = UserType.Staff;

            ClearMessages();

            editContext = new EditContext(registrationRequest);
        }


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
                }
                else
                {
                    errorMessage =
                        "Registration failed. Please try again.";
                }
            }
            catch (HttpRequestException)
            {
                errorMessage =
                    "Unable to connect to the banking service.";
            }
            catch (Exception)
            {
                errorMessage =
                    "Something went wrong during registration.";
            }
            finally
            {
                isSubmitting = false;
            }
        }


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


        private void AddKycDocument()
        {
            kycErrorMessage = null;

            if (selectedKycFile is null)
            {
                kycErrorMessage =
                    "Please select a document.";

                return;
            }

            if (registrationRequest.KycDocuments.Any(
                    x => x.Name.Equals(
                        selectedKycFile.Name,
                        StringComparison.OrdinalIgnoreCase)))
            {
                kycErrorMessage =
                    "This document has already been added.";

                return;
            }

            registrationRequest.KycDocuments.Add(
                selectedKycFile);

            selectedKycFile = null;
        }


        private void RemoveKycDocument(
            IBrowserFile document)
        {
            registrationRequest.KycDocuments.Remove(
                document);
        }


        private void ClearMessages()
        {
            errorMessage = null;

            registrationSuccessful = false;

            kycErrorMessage = null;

            selectedKycFile = null;
        }


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
}
