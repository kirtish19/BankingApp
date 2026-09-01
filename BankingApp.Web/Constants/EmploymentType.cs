using System.Text.Json.Serialization;

namespace BankingApp.Web.Constants
{
    [JsonConverter(typeof(JsonStringEnumConverter<EmploymentType>))]
    public enum EmploymentType
    {
        SelfEmployed,
        Salaried,
        Unemployed
    }
}
