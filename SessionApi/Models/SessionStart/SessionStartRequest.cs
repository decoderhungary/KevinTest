using MediatR;
using SessionApi.Models.Results;
using SessionApi.Models.Results.SessionStart;
using SessionApi.Validators;

namespace SessionApi.Models.SessionStart;

public class SessionStartRequest:ValidatedResult<SessionStartRequest>, IRequest<StartSessionResult>
{
    public required string PlayerId { get; set; }
    public required string GameId { get; set; }
}