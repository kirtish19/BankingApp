using System.Text.Json.Serialization;

namespace BankingApp.Web.Constants
{
    [JsonConverter(typeof(JsonStringEnumConverter<LoanType>))]
    public enum LoanType
    {
        Personal,
        Home,
        Education,
        Vehicle
    }
}
