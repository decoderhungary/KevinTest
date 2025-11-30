using MediatR;
using SessionApi.Models.Results;

namespace SessionApi.Models.GetSession;

public class GetSessionRequest:IRequest<GetSessionResult>
{
    public required string SessionId { get; set; }
}