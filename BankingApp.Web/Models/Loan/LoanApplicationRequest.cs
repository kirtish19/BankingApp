using BankingApp.Web.Constants;
using Microsoft.AspNetCore.Components.Forms;
namespace BankingApp.Web.Models.Loan;
public class LoanApplicationRequest
{
    public LoanType LoanType { get; set; }

    public decimal LoanAmount { get; set; }

    public int TenureMonths { get; set; }

    public IBrowserFile? SalarySlip { get; set; }

    public IBrowserFile? BankStatement { get; set; }

    public IBrowserFile? EmploymentLetter { get; set; }
}