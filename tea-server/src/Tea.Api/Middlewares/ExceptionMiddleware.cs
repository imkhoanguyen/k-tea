using System.Text.Json;
using Tea.Domain.Common;
using Tea.Domain.Exceptions;

namespace Tea.Api.Middlewares
{
    public class ExceptionMiddleware(RequestDelegate next, IHostEnvironment env)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                string message;
                context.Response.ContentType = "application/json";

                switch (ex)
                {
                    case NotFoundException:
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        message = ex.Message;
                        break;
                    case BadRequestException:
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        message = ex.Message;
                        break;
                    default:
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        message = ex.Message;
                        break;
                }

                var response = env.IsDevelopment()
                ? new ApiException(context.Response.StatusCode, message, ex.StackTrace)
                : new ApiException(context.Response.StatusCode, message, "Internal server error");

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(response, options);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
