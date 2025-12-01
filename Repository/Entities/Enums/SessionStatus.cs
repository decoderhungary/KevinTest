using System.Text.Json.Serialization;

namespace Repository.Entities.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SessionStatus
{
    Active = 1,
    InActive = 2,
}