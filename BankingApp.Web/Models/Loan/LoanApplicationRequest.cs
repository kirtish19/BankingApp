using BankingApp.Web.Constants;
using Microsoft.AspNetCore.Components.Forms;
namespace BankingApp.Web.Models.Loan;
public class LoanApplicationRequest
{
    public LoanType LoanType { get; set; }

    public decimal LoanAmount { get; set; }

    public int TenureMonths { get; set; }

    public LoanDocument? SalarySlip { get; set; }
    public LoanDocument? BankStatement { get; set; }
    public LoanDocument? EmploymentLetter { get; set; }
}