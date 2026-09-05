namespace BankingApp.Shared.Constants.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter<LoanStatus>))]
    public enum LoanStatus
    {
        Submitted,
        Approved,
        Rejected,
        ManualReview
    }
}
