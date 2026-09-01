using BankingApp.Shared.Constants.Enums;

namespace BankingApp.Shared.Helpers
{
    public static class CreditRiskAssesmentHelper
    {
        public static RiskAssesment CalculateCustomerRisk(int creditScore)
        {
            // Normalize input to common credit score bounds (300 - 900)
            if (creditScore < 300) creditScore = 300;
            if (creditScore > 900) creditScore = 900;

            // Common risk buckets:
            // 750 - 900 : Low risk
            // 650 - 749 : Medium risk
            // 550 - 649 : High risk
            // 300 - 549 : VeryHigh risk
            if (creditScore >= 750) return RiskAssesment.Low;
            if (creditScore >= 650) return RiskAssesment.Medium;
            if (creditScore >= 550) return RiskAssesment.High;
            return RiskAssesment.VeryHigh;
        }
    }
}
