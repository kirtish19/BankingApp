using BankingApp.Web.Constants;

namespace BankingApp.Web.Components.Pages.ApplyLoan
{
    public partial class ApplyLoan
    {
        private readonly List<LoanOption> LoanTypes =
                [
                    new(
                            LoanType.Personal,
                            "Personal Loan",
                            "For your personal financial needs.",
                            "💰"),

                        new(
                            LoanType.Home,
                            "Home Loan",
                            "Finance your dream home.",
                            "🏠"),

                        new(
                            LoanType.Vehicle,
                            "Vehicle Loan",
                            "Finance your new or used vehicle.",
                            "🚗"),

                        new(
                            LoanType.Education,
                            "Education Loan",
                            "Invest in your education and future.",
                            "🎓")
                ];


        private void SelectLoan(LoanType loanType)
        {
            Navigation.NavigateTo(
                $"/loan-application/{loanType}");
        }


        private void GoBack()
        {
            Navigation.NavigateTo(
                "/customer-dashboard");
        }


        private record LoanOption(
            LoanType Type,
            string Name,
            string Description,
            string Icon);
    }
}
