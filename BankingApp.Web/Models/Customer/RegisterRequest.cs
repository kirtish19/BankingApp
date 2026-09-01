using BankingApp.Web.Constants;
using Microsoft.AspNetCore.Components.Forms;

namespace BankingApp.Web.Models.Registration;

public class RegistrationRequest
{
    // -----------------------------------------
    // User Information
    // -----------------------------------------

    public UserType UserType { get; set; } = UserType.Customer;

    public string UserName { get; set; } = string.Empty;

    public string LoginPassword { get; set; } = string.Empty;

    public string ConfirmLoginPassword { get; set; } = string.Empty;

    public string ProfilePassword { get; set; } = string.Empty;

    public string ConfirmProfilePassword { get; set; } = string.Empty;


    // -----------------------------------------
    // Customer Information
    // -----------------------------------------

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string MobileNumber { get; set; } = string.Empty;

    public EmploymentType EmploymentType { get; set; }

    public decimal AnnualIncome { get; set; }

    public int CreditScore { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public List<IBrowserFile> KycDocuments { get; set; } = [];
}

