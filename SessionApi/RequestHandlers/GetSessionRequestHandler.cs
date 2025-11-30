using System;
using System.Threading;
using System.Threading.Tasks;
using Facet.Extensions;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Repository.Entities;
using Repository.Services;
using SessionApi.Models.GetSession;
using SessionApi.Models.Results;

namespace SessionApi.RequestHandlers;

public class GetSessionRequestHandler : IRequestHandler<GetSessionRequest, GetSessionResult>
{
    private readonly IMemoryCache _memoryCache;
    private readonly IServiceBase<Session> _service;
    private readonly ILogger<GetSessionRequestHandler> _logger;

    public GetSessionRequestHandler(IMemoryCache memoryCache, IServiceBase<Session> service,
        ILogger<GetSessionRequestHandler> logger)
    {
        _memoryCache = memoryCache;
        _service = service;
        _logger = logger;
    }

    public async Task<GetSessionResult> Handle(GetSessionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            _memoryCache.TryGetValue(request.SessionId, out var session);

            if (session != null)
            {
                var result = session.ToFacet<GetSessionResult>();
                result.HitCache = true;
                return result;
            }

            session = await _service.GetByIdAsync(request.SessionId, cancellationToken);

            if (session == null) return new GetSessionResult { ErrorMessage = "Session not found" };

            _memoryCache.Set(request.SessionId, session,
                new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60) });

            return session.ToFacet<GetSessionResult>();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return new GetSessionResult { ErrorMessage = e.Message };
        }
    }
}