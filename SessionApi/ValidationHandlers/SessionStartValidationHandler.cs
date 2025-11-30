using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SessionApi.Models.SessionStart;
using SessionApi.Validators;

namespace SessionApi.ValidationHandlers;

public class SessionStartValidationHandler:IRequestHandler<ValidateSessionStartRequest, ValidatedResult<ValidateSessionStartRequest>>
{
    public Task<ValidatedResult<ValidateSessionStartRequest>> Handle(ValidateSessionStartRequest request, CancellationToken cancellationToken)
    {
        return !request.Payload.PlayerId.StartsWith("P") ? 
            Task.FromResult(new ValidatedResult<ValidateSessionStartRequest>() {ErrorMessage = "PlayerId is invalid"}) : 
            Task.FromResult(!request.Payload.GameId.StartsWith("G") ? new ValidatedResult<ValidateSessionStartRequest>() {ErrorMessage = "GameId is invalid"} : 
                new ValidatedResult<ValidateSessionStartRequest>());
    }
}