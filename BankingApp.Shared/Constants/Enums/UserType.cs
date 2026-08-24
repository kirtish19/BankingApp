using System.Text.Json.Serialization;

namespace BankingApp.Shared.Constants.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter<UserType>))]
    public enum UserType
    {
        Customer,
        Staff
    }
}
