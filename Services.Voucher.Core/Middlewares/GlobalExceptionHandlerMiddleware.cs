using Microsoft.AspNetCore.Http;
using Services.Voucher.Core.Exceptions;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.Voucher.Core.Middlewares
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                response.StatusCode = ex switch
                {
                    CustomServiceException e => (int)HttpStatusCode.BadRequest,
                    _ => (int)HttpStatusCode.InternalServerError,// unhandled exception
                };
                var result = JsonSerializer.Serialize(new { message = ex?.Message });
                await response.WriteAsync(result);
            }
        }
    }
}
