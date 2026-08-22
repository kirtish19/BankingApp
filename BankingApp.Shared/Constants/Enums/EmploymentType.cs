using System.Text.Json.Serialization;

namespace BankingApp.Shared.Constants.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter<EmploymentType>))]
    public enum EmploymentType
    {
        SelfEmployed,
        Salaried,
        Unemployed
    }
}
