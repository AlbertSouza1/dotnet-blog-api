using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Blog.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        private readonly IConfiguration _configuration;

        public ApiKeyAttribute([FromServices] IConfiguration configuration) => _configuration = configuration;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Query.TryGetValue(_configuration.GetValue<string>("ApiKeyName"), out var extractedApiKey))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "ApiKey not found."
                };

                return;
            }

            if (!IsValidApiKey(extractedApiKey))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 403,
                    Content = "Unauthorized."
                };

                return;
            }

            await next();
        }

        private bool IsValidApiKey(StringValues extractedApiKey)
            => _configuration.GetValue<string>("ApiKey").Equals(extractedApiKey);
    }
}