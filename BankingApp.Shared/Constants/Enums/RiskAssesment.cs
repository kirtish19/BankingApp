namespace BankingApp.Shared.Constants.Enums
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
