namespace DotNet8.ExceptionHandlerMiddleware.Api;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IWebHostEnvironment _env;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IWebHostEnvironment env
        )
    {
        _logger = logger;
        _env = env;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        exception.AddErrorCode();
        _logger.LogError(exception, "An unhandled error occoured while executing the request.");
        httpContext.Response.Clear();
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/json";
        var problemDetails =  GetProblemDetails(httpContext,exception);
        await httpContext.Response.WriteAsync(problemDetails.ToJson(),cancellationToken);

        return true;
    }

    private ProblemDetails GetProblemDetails(
        in HttpContext context,
        in Exception exception
        )
    {
        var errorCode = exception.GetErrorCode();
        var statusCode=context.Response.StatusCode;
        var reasonphrase = ReasonPhrases.GetReasonPhrase(statusCode);
        if(string.IsNullOrEmpty(reasonphrase))
        {
            reasonphrase = "An unhandled error was occured while executing the request.";
        }
        var problemDetail = new ProblemDetails
        {
            Status = statusCode,
            Title = reasonphrase,
            Extensions =
            {
                [nameof(errorCode)]=errorCode
            }
        };
        if (!_env.IsDevelopment())
        {
            return problemDetail;
        }
        problemDetail.Detail = exception.Message;
        problemDetail.Extensions["traceId"] = context.TraceIdentifier;
        problemDetail.Extensions["data"] = exception.Data;
        return problemDetail;
    }
}
