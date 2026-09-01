using System.Text.Json.Serialization;

namespace BankingApp.Web.Constants
{
    [JsonConverter(typeof(JsonStringEnumConverter<UserType>))]
    public enum UserType
    {
        Customer,
        Staff
    }
}
