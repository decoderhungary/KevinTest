using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Repository.Services;
using SessionApi.Models.GetSession;

namespace SessionApi.Controllers;

[ApiController]
[Route("[controller]")]
public class DiagnosticsController(IMediator mediator, IServiceBase<Session> service) : ControllerBase
{
    [HttpGet]
    [Route("perf-test")]
    public async Task<ActionResult> OnGetAsync([FromQuery] int iterations,
        CancellationToken cancellationToken = default)
    {
        if (iterations <= 0) return BadRequest("Iterations must be greater than 0");
        
        var sampleSession = await service.GetAsQueryable().FirstOrDefaultAsync(cancellationToken);

        if (sampleSession is null) return new NotFoundResult();

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        for (var i = 0; i < iterations; i++)
        {
            await mediator.Send(new GetSessionRequest { SessionId = sampleSession.SessionId }, cancellationToken);
        }

        stopwatch.Stop();

        var response = new
        {
            Iterations = iterations,
            TotalMilliseconds = stopwatch.Elapsed.TotalMilliseconds,
            AverageMilliseconds = stopwatch.Elapsed.TotalMilliseconds / iterations
        };

        return Ok(response);
    }
}