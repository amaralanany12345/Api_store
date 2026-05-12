using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using StoreWebApi.Models;

namespace StoreWebApi.Actions
{
    public class IdempotentAttribute: ActionFilterAttribute
    {
            private const string idempotentAttribute = "X-IdempotentAttribute-Key";
            private const string inProgressStatus = "inProgress";
            public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                if (!context.HttpContext.Request.Headers.TryGetValue(idempotentAttribute, out var header) || string.IsNullOrEmpty(idempotentAttribute))
                {
                    context.Result = new BadRequestObjectResult("missing idempotent attribute");
                    return;
                }
                var cacheKey = $"idempotent: {idempotentAttribute}";
                var cache = context.HttpContext.RequestServices.GetService<IDistributedCache>();
                var cachedResponseJson = await cache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cachedResponseJson))
                {
                    if (cachedResponseJson == inProgressStatus)
                    {
                        context.Result = new StatusCodeResult(StatusCodes.Status409Conflict);
                    }
                    var cachedResponse = System.Text.Json.JsonSerializer.Deserialize<CacheResponse>(cachedResponseJson);
                    if (cachedResponse != null)
                    {
                        context.Result = new ObjectResult(cachedResponse.Value)
                        {
                            StatusCode = cachedResponse.StatusCode,
                        };
                        return;
                    }
                }
                else
                {
                    await cache.SetStringAsync(cacheKey, "inProgressStatus", new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
                    });
                    var executedContext = await next();
                    if (executedContext.Result is ObjectResult objectResult)
                    {
                        if (objectResult.StatusCode >= 200 && objectResult.StatusCode < 300)
                        {
                            var cacheResponse = new CacheResponse
                            {
                                StatusCode = objectResult.StatusCode ?? 200,
                                Value = objectResult.Value,
                            };
                            var cacheResponseJson = System.Text.Json.JsonSerializer.Serialize(cacheResponse);
                            await cache.SetStringAsync(cacheKey, cacheResponseJson, new DistributedCacheEntryOptions
                            {
                                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2),
                            });
                        }
                        else
                        {
                            await cache.RemoveAsync(cacheKey);
                        }
                    }
                }
            }
        }
    }
