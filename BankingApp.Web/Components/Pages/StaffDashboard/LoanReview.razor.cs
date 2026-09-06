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

    private string ReviewComments = string.Empty;

    private bool IsLoading = true;
    private bool IsUpdating;

    private string? ErrorMessage;

    private bool _initialized;

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

    private async Task UpdateStatus(string status)
    {
        if (Loan is null || IsUpdating)
        {
            Console.WriteLine("[Loan Review] Update ignored.");
            return;
        }

        try
        {
            IsUpdating = true;
            ErrorMessage = null;

            Console.WriteLine();
            Console.WriteLine("==================================================");
            Console.WriteLine("[Loan Review] STATUS UPDATE STARTED");
            Console.WriteLine("==================================================");

            // --------------------------------------------------
            // 1. Current loan information
            // --------------------------------------------------

            Console.WriteLine($"[Loan Review] Loan ID           : {Loan.Id}");
            Console.WriteLine($"[Loan Review] Current Status    : {Loan.Status}");
            Console.WriteLine($"[Loan Review] Selected Status   : {status}");
            Console.WriteLine($"[Loan Review] Review Comments   : {ReviewComments}");

            // --------------------------------------------------
            // 2. Validate session
            // --------------------------------------------------

            var isSessionValid =
                await AuthStorageService.IsSessionValidAsync();

            Console.WriteLine(
                $"[Loan Review] Session Valid    : {isSessionValid}");

            if (!isSessionValid)
            {
                Console.WriteLine(
                    "[Loan Review] Session invalid. Redirecting to login.");

                Navigation.NavigateTo("/login");
                return;
            }

            // --------------------------------------------------
            // 3. Create status description
            // --------------------------------------------------

            var statusDescription =
                status switch
                {
                    "Approved" =>
                        "Loan application approved by staff.",

                    "Rejected" =>
                        "Loan application rejected by staff.",

                    "ManualReview" =>
                        "Loan application moved for manual review.",

                    _ =>
                        string.Empty
                };

            Console.WriteLine(
                $"[Loan Review] Status Description: {statusDescription}");

            // --------------------------------------------------
            // 4. Call Staff API
            // --------------------------------------------------

            Console.WriteLine();
            Console.WriteLine(
                "[Loan Review] Calling UpdateLoanStatusAsync...");
            Console.WriteLine(
                $"[Loan Review] Sending Status: {status}");

            var success =
                await StaffLoanService.UpdateLoanStatusAsync(
                    Loan.Id,
                    "1",
                   "" ,
                    "yashkashid2002@gmail.com",
                    "Yash",
                    ReviewComments);

            // --------------------------------------------------
            // 5. Check result
            // --------------------------------------------------

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

            Console.WriteLine("==================================================");
            Console.WriteLine("[Loan Review] STATUS UPDATE COMPLETED");
            Console.WriteLine("==================================================");
            Console.WriteLine();

            // --------------------------------------------------
            // 6. Redirect
            // --------------------------------------------------

            Navigation.NavigateTo("/staff-dashboard");
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine("==================================================");
            Console.WriteLine("[Loan Review] STATUS UPDATE ERROR");
            Console.WriteLine("==================================================");
            Console.WriteLine($"[Loan Review] Exception Type : {ex.GetType().Name}");
            Console.WriteLine($"[Loan Review] Message        : {ex.Message}");
            Console.WriteLine($"[Loan Review] Details        : {ex}");
            Console.WriteLine("==================================================");
            Console.WriteLine();

            ErrorMessage =
                $"Unable to update the loan application. {ex.Message}";
        }
        finally
        {
            IsUpdating = false;
        }
    }

    private void GoBack()
    {
        Navigation.NavigateTo("/staff-dashboard");
    }

    private string GetLoanTypeName(LoanType loanType)
    {
        return loanType switch
        {
            LoanType.Personal => "Personal Loan",
            LoanType.Home => "Home Loan",
            LoanType.Education => "Education Loan",
            LoanType.Vehicle => "Vehicle Loan",
            _ => "Loan"
        };
    }

    private string GetStatusClass(LoanStatus status)
    {
        return status switch
        {
            LoanStatus.Approved => "badge bg-success",
            LoanStatus.Rejected => "badge bg-danger",
            LoanStatus.ManualReview => "badge bg-warning text-dark",
            LoanStatus.Submitted => "badge bg-primary",
            _ => "badge bg-secondary"
        };
    }

    private string GetRiskClass(RiskAssesment? risk)
    {
        return risk switch
        {
            RiskAssesment.Low => "badge bg-success",
            RiskAssesment.Medium => "badge bg-warning text-dark",
            RiskAssesment.High => "badge bg-danger",
            RiskAssesment.VeryHigh => "badge bg-dark",
            _ => "badge bg-secondary"
        };
    }

    private string GetRiskText(RiskAssesment? risk)
    {
        return risk?.ToString() ?? "-";
    }
}