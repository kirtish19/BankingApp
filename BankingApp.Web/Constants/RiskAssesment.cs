using System.Text.Json.Serialization;

namespace BankingApp.Web.Constants
{
    [JsonConverter(typeof(JsonStringEnumConverter<RiskAssesment>))]
    public enum RiskAssesment
    {
        Low,
        Medium,
        High,
        VeryHigh
    }
}
