using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using StoreWebApi.Models;

namespace StoreWebApi.Actions
{
    public class IdempotentAttribute: ActionFilterAttribute
    {
            private const string idempotentAttribute = "X-IdempotentAttribute-Key";
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
                    Console.WriteLine("there is cached response in the header");
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
                Console.WriteLine("cached response is not found");
                var executedContext = await next();
                    if (executedContext.Result is ObjectResult objectResult &&
                    objectResult.StatusCode >= 200 && 
                    objectResult.StatusCode < 300)
                    {
                            var cacheResponse = new CacheResponse
                            {
                                StatusCode = objectResult.StatusCode ?? 200,
                                Value = objectResult.Value,
                            };
                            var cacheResponseJson = System.Text.Json.JsonSerializer.Serialize(cacheResponse);
                            await cache.SetStringAsync(cacheKey, cacheResponseJson, new DistributedCacheEntryOptions
                            {
                                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(20),
                            });
                        }
                }
            }
        }
    }
