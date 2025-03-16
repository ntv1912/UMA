
using Microsoft.AspNetCore.Diagnostics;
using UMA.Exceptions;
using UMA.Models;

namespace UMA.Handler
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public GlobalExceptionHandler()
        {

        }
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var res = new Response
            {
                IsError = true,
                Message = exception.Message
            };
            switch (exception)
            {
                case NotFoundException:
                    res.Exception = nameof(NotFoundException);
                    res.StatusCode = StatusCodes.Status404NotFound;
                    break;
                case DuplicateDataException:
                    res.Exception = nameof(DuplicateDataException);
                    res.StatusCode = StatusCodes.Status409Conflict;
                    break;
                case InvalidLoginException:
                    res.Exception = nameof(InvalidLoginException);
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    break;
                case UnauthorizedException:
                    res.Exception = nameof(UnauthorizedException);
                    res.StatusCode = StatusCodes.Status403Forbidden;
                    break;
                case UnAuthenticaionException:
                    res.Exception = nameof(UnAuthenticaionException);
                    res.StatusCode = StatusCodes.Status401Unauthorized;
                    break;
                default: 
                    res.Exception="Something went wrong";
                    res.StatusCode= StatusCodes.Status500InternalServerError;
                    break;
            }
            httpContext.Response.StatusCode = StatusCodes.Status200OK;
            await httpContext.Response.WriteAsJsonAsync(res);
            return true;
        }
    }
}
