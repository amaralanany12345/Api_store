using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using StoreService.ResponseModel;
using StoreDataBase.AppContexts;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using StoreDomain.Models;

namespace StoreWebApi.Actions
{
    public class IdempotentAttribute: IAsyncActionFilter
    {
        private readonly AppDbContext _context;

        public IdempotentAttribute(AppDbContext context)
        {
            _context = context;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var key = context.HttpContext.Request.Headers["Idempotency-Key"].FirstOrDefault();
            if (key==null)
            {
                Console.WriteLine("key is not found");
                return;
            }
            var existingIdempotency=await _context.IdempotencyRecords.Where(a=>a.Key==key).FirstOrDefaultAsync();
            if(existingIdempotency!=null)
            {
                context.Result = new ContentResult
                {
                    StatusCode=existingIdempotency.StatusCode,
                    Content=existingIdempotency.Value,
                    ContentType="application/json",
                };
                return;
            }
            else
            {
                var executedContext = await next();
                if(executedContext.Result is ObjectResult result)
                {
                    var response=JsonSerializer.Serialize(result.Value);
                    await _context.IdempotencyRecords.AddAsync(new IdempotencyRecord
                    {
                        CreatedAt=DateTime.Now,
                        StatusCode=200,
                        Key=key,
                        Value=response
                    });
                    await _context.SaveChangesAsync();
                }
            }

        }
    }
}
