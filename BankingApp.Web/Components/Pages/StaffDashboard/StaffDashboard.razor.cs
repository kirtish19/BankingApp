using BankingApp.Web.Constants;
using BankingApp.Web.Models.Loan;
using BankingApp.Web.Services.Authentication;
using BankingApp.Web.Services.Staff;
using Microsoft.AspNetCore.Components;

namespace BankingApp.Web.Components.Pages.StaffDashboard;

public partial class StaffDashboard
{
    // =========================================
    // SERVICES
    // =========================================

    [Inject]
    private IAuthenticationService AuthenticationService
    { get; set; } = null!;

    [Inject]
    private IStaffLoanService StaffLoanService
    { get; set; } = null!;

    [Inject]
    private IAuthStorageService AuthStorageService
    { get; set; } = null!;

    [Inject]
    private NavigationManager Navigation
    { get; set; } = null!;


    // =========================================
    // DATA
    // =========================================

    private List<LoanApplicationsDto> AllLoans = [];

    private List<LoanApplicationsDto> PendingLoans = [];


    // =========================================
    // UI STATE
    // =========================================

    private bool IsLoading = true;

    private string? ErrorMessage;

    private bool _initialized;


    // =========================================
    // SUMMARY
    // =========================================

    private int ApprovedLoanCount =>
        AllLoans.Count(
            loan =>
                loan.Status == LoanStatus.Approved);


    // =========================================
    // PAGE INITIALIZATION
    // =========================================

    protected override async Task OnAfterRenderAsync(
        bool firstRender)
    {
        if (!firstRender || _initialized)
        {
            return;
        }

        _initialized = true;

        await LoadDashboardAsync();

        StateHasChanged();
    }


    // =========================================
    // LOAD DASHBOARD
    // =========================================

    private async Task LoadDashboardAsync()
    {
        try
        {
            IsLoading = true;

            ErrorMessage = null;


            // Check frontend session.
            var isSessionValid =
                await AuthStorageService.IsSessionValidAsync();

            if (!isSessionValid)
            {
                Navigation.NavigateTo("/login");

                return;
            }


            // Load all loan applications.
            AllLoans =
                await StaffLoanService.GetAllLoansAsync();


            // Load pending loan applications.
            PendingLoans =
                await StaffLoanService.GetPendingLoansAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Staff Dashboard: {ex}");

            ErrorMessage =
                "Unable to load loan applications.";
        }
        finally
        {
            IsLoading = false;
        }
    }


    // =========================================
    // LOAN TYPE DISPLAY
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
    // STATUS DISPLAY
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
    // RISK DISPLAY
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


    private string GetRiskText(
        RiskAssesment? risk)
    {
        return risk?.ToString() ?? "-";
    }


    // =========================================
    // REVIEW LOAN
    // =========================================

    private void ReviewLoan(Guid loanId)
    {
        Navigation.NavigateTo(
            $"/staff-dashboard/loan/{loanId}");
    }


    // =========================================
    // LOGOUT
    // =========================================

    private async Task Logout()
    {
        await AuthenticationService.LogoutAsync();

        Navigation.NavigateTo(
            "/login");
    }
}