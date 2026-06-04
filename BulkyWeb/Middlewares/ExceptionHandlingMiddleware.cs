using BulkyWeb.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;








namespace BulkyWeb.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {

            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            string message = exception.Message;
            switch (exception)
            {
                case ValidationException:
                    statusCode = HttpStatusCode.BadRequest; // 400
                    break;

                case NotFoundException:
                    statusCode = HttpStatusCode.NotFound; // 404
                    break;

                case BusinessException:
                    statusCode = HttpStatusCode.Conflict; // 409 یا 400
                    break;

                // بقیه Exception های EF
                case DbUpdateException:
                    statusCode = HttpStatusCode.InternalServerError; // 500
                    message = "Database update failed@@@@@@.";
                    break;
                case KeyNotFoundException:
                    statusCode = HttpStatusCode.NotFound; // 404
                    message = "Item(s) not found@@@@@.";
                    break;

                case InvalidOperationException:
                    statusCode = HttpStatusCode.BadRequest; // 400
                    break;

                case ArgumentException:
                    statusCode = HttpStatusCode.BadRequest;
                    message = "Arguments have problems@@@@@.";
                    break;
                case TaskCanceledException:
                    statusCode = HttpStatusCode.RequestTimeout; // 408
                    break;
                case UnauthorizedAccessException:
                    statusCode = HttpStatusCode.Unauthorized;
                    break;
                case NullReferenceException:
                    statusCode = HttpStatusCode.InternalServerError;
                    message = "A required resource was not initialized@@@@.";
                    break;
                case OperationCanceledException:
                    statusCode = (HttpStatusCode)499;
                    message = "Request was cancelled by the client.";
                    break;

                    //case BadHttpRequestException:
                    //    statusCode = (HttpStatusCode)400;
                    //    message = "Request not found";
                    //    break;


            }


            var response = new
            {
                message = message,
                status = (int)statusCode,
                timestamp = DateTime.UtcNow
            };

            var payload = JsonSerializer.Serialize(response);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(payload);
        }


    }
}
