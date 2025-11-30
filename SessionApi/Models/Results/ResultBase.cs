using System.Text.Json.Serialization;

namespace SessionApi.Models.Results;

public class ResultBase
{
    [JsonIgnore]
    public bool IsSuccessful => ErrorMessage is null;
    public string? ErrorMessage { get; init; }
}