using BankingApp.Web.Constants;
using BankingApp.Web.Models.Loan;
using BankingApp.Web.Models.Staff;
using BankingApp.Web.Services.Authentication;
using BankingApp.Web.Services.Staff;
using Microsoft.AspNetCore.Components;

namespace BankingApp.Web.Components.Pages.StaffDashboard;

public partial class StaffDashboard
{
    // SERVICES
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


    // DATA
    private List<LoanApplicationsDto> AllLoans = [];
    private List<LoanApplicationsDto> PendingLoans = [];

    private Dictionary<Guid, CustomerDetailsDto> CustomerDetails = [];


    // UI STATE
    private bool IsLoading = true;
    private string? ErrorMessage;
    private bool _initialized;


    // STATUS FILTER
    private LoanStatus? SelectedStatus { get; set; }


    // FILTERED ALL LOANS
    private List<LoanApplicationsDto> FilteredAllLoans =>
        SelectedStatus is null
            ? AllLoans
            : AllLoans
                .Where(loan => loan.Status == SelectedStatus.Value)
                .ToList();


    // SUMMARY
    private int ApprovedLoanCount =>
        AllLoans.Count(
            loan =>
                loan.Status == LoanStatus.Approved);


    // PAGE INITIALIZATION
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender || _initialized)
        {
            return;
        }

        _initialized = true;

        await LoadDashboardAsync();

        StateHasChanged();
    }


    // LOAD DASHBOARD
    private async Task LoadDashboardAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;

            var isSessionValid =
                await AuthStorageService.IsSessionValidAsync();

            if (!isSessionValid)
            {
                Navigation.NavigateTo("/login");
                return;
            }

            AllLoans =
                await StaffLoanService.GetAllLoansAsync();

            PendingLoans =
                await StaffLoanService.GetPendingLoansAsync();

            await LoadCustomerDetailsAsync();
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


    // LOAD CUSTOMER DETAILS
    private async Task LoadCustomerDetailsAsync()
    {
        var customerIds =
            AllLoans
                .Concat(PendingLoans)
                .Select(loan => loan.CustomerId)
                .Distinct()
                .ToList();

        foreach (var customerId in customerIds)
        {
            if (CustomerDetails.ContainsKey(customerId))
            {
                continue;
            }

            var customer =
                await StaffLoanService
                    .GetCustomerDetailsAsync(customerId);

            if (customer is not null)
            {
                CustomerDetails[customerId] = customer;
            }
        }
    }


    // CUSTOMER NAME
    private string GetCustomerName(Guid customerId)
    {
        if (!CustomerDetails.TryGetValue(
                customerId,
                out var customer))
        {
            return "-";
        }

        return
            $"{customer.FirstName} {customer.LastName}"
                .Trim();
    }


    // CUSTOMER EMAIL
    private string GetCustomerEmail(Guid customerId)
    {
        if (!CustomerDetails.TryGetValue(
                customerId,
                out var customer))
        {
            return "-";
        }

        return customer.Email ?? "-";
    }


    // LOAN TYPE DISPLAY
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


    // STATUS DISPLAY
    private string GetStatusClass(LoanStatus status)
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


    // RISK DISPLAY
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


    // REVIEW LOAN
    private void ReviewLoan(Guid loanId)
    {
        Navigation.NavigateTo(
            $"/staff-dashboard/loan/{loanId}");
    }


    // LOGOUT
    private async Task Logout()
    {
        await AuthenticationService.LogoutAsync();

        Navigation.NavigateTo("/login");
    }
}