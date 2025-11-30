using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SessionApi.Helpers;
using SessionApi.Models.GetSession;
using SessionApi.Models.Results.SessionStart;
using SessionApi.Models.SessionStart;

namespace SessionApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SessionsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Route("start")]
    [Idempotent<StartSessionResult>]
    public async Task<ActionResult> Start(SessionStartRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult =
            await mediator.Send(new ValidateSessionStartRequest { Payload = request }, cancellationToken);

        if (!validationResult.IsValid) return BadRequest(validationResult);

        var result = await mediator.Send(request, cancellationToken);

        if (!result.IsSuccessful)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("{sessionId}")]
    public async Task<ActionResult> OnGet(string sessionId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(sessionId)) return BadRequest();

        var result = await mediator.Send(new GetSessionRequest { SessionId = sessionId }, cancellationToken);

        if (!result.IsSuccessful)
        {
            return BadRequest(result);
        }

        Response.Headers.Add("X-Cache", result.HitCache ? "Hit" : "Miss");

        return Ok(result);
    }
}