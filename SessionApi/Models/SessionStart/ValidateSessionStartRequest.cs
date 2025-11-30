using MediatR;
using SessionApi.Validators;

namespace SessionApi.Models.SessionStart;

public class ValidateSessionStartRequest: IRequest<ValidatedResult<ValidateSessionStartRequest>>
{
    public required SessionStartRequest Payload { get; init; }
}

