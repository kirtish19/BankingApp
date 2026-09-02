namespace BankingApp.Shared.Constants.Enums
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
