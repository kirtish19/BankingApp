using BankingApp.Web.Constants;
using BankingApp.Web.Models.Loan;
using BankingApp.Web.Services.Authentication;
using BankingApp.Web.Services.Staff;
using Microsoft.AspNetCore.Components;

namespace BankingApp.Web.Components.Pages.StaffDashboard;

public partial class LoanReview
{
    [Parameter]
    public Guid LoanId { get; set; }

    [Inject]
    private IStaffLoanService StaffLoanService
    { get; set; } = null!;

    [Inject]
    private IAuthStorageService AuthStorageService
    { get; set; } = null!;

    [Inject]
    private NavigationManager Navigation
    { get; set; } = null!;

    private LoanApplicationsDto? Loan;

    private string CustomerName = "-";

    private string ReviewComments = string.Empty;

    private bool IsLoading = true;
    private bool IsUpdating;

    private string? ErrorMessage;

    private bool _initialized;


    // =========================================
    // LOAD LOAN
    // =========================================

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        Console.WriteLine(
            $"[LoanReview] OnAfterRenderAsync called. FirstRender: {firstRender}");

        if (!firstRender || _initialized)
        {
            return;
        }

        _initialized = true;

        Console.WriteLine(
            "[LoanReview] Starting LoadLoanAsync...");

        await LoadLoanAsync();

        Console.WriteLine(
            "[LoanReview] LoadLoanAsync completed.");

        StateHasChanged();
    }


    // =========================================
    // LOAD LOAN APPLICATION
    // =========================================

    private async Task LoadLoanAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;

            Console.WriteLine(
                "========================================");

            Console.WriteLine(
                $"[LoanReview] LoanId: {LoanId}");

            Console.WriteLine(
                "[LoanReview] Checking session...");

            var isSessionValid =
                await AuthStorageService.IsSessionValidAsync();

            Console.WriteLine(
                $"[LoanReview] Session valid: {isSessionValid}");

            if (!isSessionValid)
            {
                Console.WriteLine(
                    "[LoanReview] Session expired. Redirecting to login.");

                Navigation.NavigateTo("/login");

                return;
            }

            Console.WriteLine(
                "[LoanReview] Calling StaffLoanService.GetLoanByIdAsync...");

            Loan =
                await StaffLoanService.GetLoanByIdAsync(LoanId);

            Console.WriteLine(
                "[LoanReview] GetLoanByIdAsync completed.");

            if (Loan is null)
            {
                Console.WriteLine(
                    "[LoanReview] Loan was NULL.");

                ErrorMessage =
                    "Loan application was not found.";

                return;
            }

            Console.WriteLine(
                $"[LoanReview] Loan received: {Loan.Id}");

            Console.WriteLine(
                $"[LoanReview] Customer ID: {Loan.CustomerId}");


            // =========================================
            // GET CUSTOMER DETAILS
            // =========================================

            var customer =
                await StaffLoanService.GetCustomerDetailsAsync(
                    Loan.CustomerId);

            if (customer is not null)
            {
                CustomerName =
                    $"{customer.FirstName} {customer.LastName}".Trim();

                Console.WriteLine(
                    $"[LoanReview] Customer Name: {CustomerName}");
            }
            else
            {
                Console.WriteLine(
                    "[LoanReview] Customer details not found.");

                CustomerName = "-";
            }


            ReviewComments =
                Loan.ReviewComments ?? string.Empty;

            Console.WriteLine(
                "[LoanReview] Loan loaded successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                "========== LOAN REVIEW ERROR ==========");

            Console.WriteLine(
                $"Message: {ex.Message}");

            Console.WriteLine(
                $"Type: {ex.GetType().FullName}");

            Console.WriteLine(
                $"StackTrace: {ex.StackTrace}");

            if (ex.InnerException is not null)
            {
                Console.WriteLine(
                    $"Inner Exception: {ex.InnerException.Message}");
            }

            Console.WriteLine(
                "========================================");

            ErrorMessage =
                $"Unable to load the loan application. {ex.Message}";
        }
        finally
        {
            Console.WriteLine(
                "[LoanReview] Setting IsLoading = false.");

            IsLoading = false;
        }
    }


    // =========================================
    // UPDATE LOAN STATUS
    // =========================================

    private async Task UpdateStatus(string status)
    {
        if (Loan is null || IsUpdating)
        {
            Console.WriteLine(
                "[Loan Review] Update ignored.");

            return;
        }

        try
        {
            IsUpdating = true;
            ErrorMessage = null;

            Console.WriteLine();
            Console.WriteLine(
                "==================================================");

            Console.WriteLine(
                "[Loan Review] STATUS UPDATE STARTED");

            Console.WriteLine(
                "==================================================");


            // -----------------------------------------
            // 1. Loan information
            // -----------------------------------------

            Console.WriteLine(
                $"[Loan Review] Loan ID        : {Loan.Id}");

            Console.WriteLine(
                $"[Loan Review] Customer ID    : {Loan.CustomerId}");

            Console.WriteLine(
                $"[Loan Review] Current Status : {Loan.Status}");

            Console.WriteLine(
                $"[Loan Review] Selected Status: {status}");

            Console.WriteLine(
                $"[Loan Review] Review Comments: {ReviewComments}");


            // -----------------------------------------
            // 2. Validate session
            // -----------------------------------------

            var isSessionValid =
                await AuthStorageService.IsSessionValidAsync();

            Console.WriteLine(
                $"[Loan Review] Session Valid: {isSessionValid}");

            if (!isSessionValid)
            {
                Console.WriteLine(
                    "[Loan Review] Session invalid. Redirecting to login.");

                Navigation.NavigateTo("/login");

                return;
            }


            // -----------------------------------------
            // 3. Convert UI status to API status
            // -----------------------------------------

            var statusValue =
                status switch
                {
                    "Approved" => "1",

                    "Rejected" => "2",

                    "ManualReview" => "3",

                    _ => string.Empty
                };

            if (string.IsNullOrWhiteSpace(statusValue))
            {
                Console.WriteLine(
                    "[Loan Review] Invalid status.");

                ErrorMessage =
                    "Invalid loan status.";

                return;
            }

            Console.WriteLine(
                $"[Loan Review] API Status Value: {statusValue}");


            // -----------------------------------------
            // 4. Create status description
            // -----------------------------------------

            var statusDescription =
                status switch
                {
                    "Approved" => "Approved",

                    "Rejected" => "Rejected",

                    "ManualReview" => "ManualReview",

                    _ => string.Empty
                };

            Console.WriteLine(
                $"[Loan Review] Status Description: {statusDescription}");


            // -----------------------------------------
            // 5. Call Staff Loan Service
            // -----------------------------------------

            Console.WriteLine();
            Console.WriteLine(
                "[Loan Review] Calling UpdateLoanStatusAsync...");

            var success =
                await StaffLoanService.UpdateLoanStatusAsync(
                    Loan.Id,
                    Loan.CustomerId,
                    statusValue,
                    statusDescription,
                    ReviewComments);


            // -----------------------------------------
            // 6. Check result
            // -----------------------------------------

            Console.WriteLine();
            Console.WriteLine(
                $"[Loan Review] Update Result: {success}");

            if (!success)
            {
                Console.WriteLine(
                    "[Loan Review] STATUS UPDATE FAILED.");

                ErrorMessage =
                    "Unable to update the loan application status.";

                return;
            }

            Console.WriteLine(
                "[Loan Review] STATUS UPDATE SUCCESSFUL.");

            Console.WriteLine(
                "[Loan Review] Redirecting to Staff Dashboard...");

            Console.WriteLine(
                "==================================================");

            Console.WriteLine(
                "[Loan Review] STATUS UPDATE COMPLETED");

            Console.WriteLine(
                "==================================================");

            Console.WriteLine();


            // -----------------------------------------
            // 7. Redirect
            // -----------------------------------------

            Navigation.NavigateTo(
                "/staff-dashboard");
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine(
                "==================================================");

            Console.WriteLine(
                "[Loan Review] STATUS UPDATE ERROR");

            Console.WriteLine(
                "==================================================");

            Console.WriteLine(
                $"[Loan Review] Exception Type : {ex.GetType().Name}");

            Console.WriteLine(
                $"[Loan Review] Message        : {ex.Message}");

            Console.WriteLine(
                $"[Loan Review] Details        : {ex}");

            Console.WriteLine(
                "==================================================");

            Console.WriteLine();

            ErrorMessage =
                $"Unable to update the loan application. {ex.Message}";
        }
        finally
        {
            IsUpdating = false;
        }
    }


    // =========================================
    // GO BACK
    // =========================================

    private void GoBack()
    {
        Navigation.NavigateTo(
            "/staff-dashboard");
    }


    // =========================================
    // LOAN TYPE
    // =========================================

    private string GetLoanTypeName(
        LoanType loanType)
    {
        return loanType switch
        {
            LoanType.Personal =>
                "Personal Loan",

            LoanType.Home =>
                "Home Loan",

            LoanType.Education =>
                "Education Loan",

            LoanType.Vehicle =>
                "Vehicle Loan",

            _ =>
                "Loan"
        };
    }


    // =========================================
    // LOAN STATUS CSS
    // =========================================

    private string GetStatusClass(
        LoanStatus status)
    {
        return status switch
        {
            LoanStatus.Approved =>
                "badge bg-success",

            LoanStatus.Rejected =>
                "badge bg-danger",

            LoanStatus.ManualReview =>
                "badge bg-warning text-dark",

            LoanStatus.Submitted =>
                "badge bg-primary",

            _ =>
                "badge bg-secondary"
        };
    }


    // =========================================
    // RISK CSS
    // =========================================

    private string GetRiskClass(
        RiskAssesment? risk)
    {
        return risk switch
        {
            RiskAssesment.Low =>
                "badge bg-success",

            RiskAssesment.Medium =>
                "badge bg-warning text-dark",

            RiskAssesment.High =>
                "badge bg-danger",

            RiskAssesment.VeryHigh =>
                "badge bg-dark",

            _ =>
                "badge bg-secondary"
        };
    }


    // =========================================
    // RISK TEXT
    // =========================================

    private string GetRiskText(
        RiskAssesment? risk)
    {
        return risk?.ToString() ?? "-";
    }
}

