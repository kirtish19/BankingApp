using System.Text.Json.Serialization;

namespace BankingApp.Web.Constants
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
