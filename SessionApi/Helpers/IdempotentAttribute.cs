using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace SessionApi.Helpers;

[AttributeUsage(AttributeTargets.Method)]
public class IdempotentAttribute<TResponse> : Attribute, IAsyncActionFilter where TResponse : class
{
    private const int DefaultCacheTimeInSeconds = 180;
    
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue("Idempotence-Key", out var idempotenceKey))
        {
            context.Result = new BadRequestObjectResult("Invalid or missing Idempotence-Key header");
        }
        
        if (string.IsNullOrEmpty(idempotenceKey)) throw new InvalidOperationException("Idempotence-Key header is required");
        
        try
        {
            var memoryCache = context.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();
            
            var cacheKey = $"IDMPT-{idempotenceKey}";
            memoryCache.TryGetValue(cacheKey, out var result);

            if (result != null)
            {
                context.Result =(OkObjectResult)result;
                return;
            }
            
            var executedContext = await next();

            if (executedContext.Result is OkObjectResult okResult)
            {
                memoryCache.Set(cacheKey, okResult, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(DefaultCacheTimeInSeconds) });
            }
        }
        catch (Exception e)
        {
           context.Result = new BadRequestObjectResult(e);
        }
    }
}