namespace SessionApi.Validators;

public interface IValidatedResult<TRequest>
{
    bool IsValid { get; }
    string? ErrorMessage { get; init; }
}

public class ValidatedResult<TRequest> : IValidatedResult<TRequest>
{
    public bool IsValid => ErrorMessage is null;
    public string? ErrorMessage { get; init; }
}