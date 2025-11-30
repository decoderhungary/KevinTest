using System;
using System.Threading;
using System.Threading.Tasks;
using Facet.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repository.Entities;
using Repository.Entities.Enums;
using Repository.Services;
using SessionApi.Models.Results.SessionStart;
using SessionApi.Models.SessionStart;

namespace SessionApi.RequestHandlers;

public class StartSessionRequestHandler : IRequestHandler<SessionStartRequest, StartSessionResult>
{
    private readonly IServiceBase<Session> _service;
    private readonly ILogger<StartSessionRequestHandler> _logger;

    public StartSessionRequestHandler(IServiceBase<Session> service, ILogger<StartSessionRequestHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<StartSessionResult> Handle(SessionStartRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var sessionId = Guid.NewGuid();

            var session = await _service.GetAsQueryable()
                .FirstOrDefaultAsync(x => x.PlayerId == request.PlayerId && x.Status == SessionStatus.Active,
                    cancellationToken);

            if (session != null) return session.ToFacet<StartSessionResult>();

            session = new Session
            {
                Status = SessionStatus.Active, PlayerId = request.PlayerId, GameId = request.GameId, SessionId = sessionId.ToString(),
                StartedAt = DateTime.UtcNow
            };

            var result = await _service.AddAsync(session, cancellationToken);

            return result.ToFacet<StartSessionResult>();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return new StartSessionResult { ErrorMessage = e.Message };
        }
    }
}