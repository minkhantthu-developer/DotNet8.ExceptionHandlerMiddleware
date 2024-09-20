namespace DotNet8.ExceptionHandlerMiddleware.Api.Middlewares;

public class ExceptionHandlerMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;
    private readonly IWebHostEnvironment _env;
    private readonly string UnhandleExceptionMessage =
        "An unhandled error was occoured while executing the result.";

    public ExceptionHandlerMiddleware(
        ILogger<ExceptionHandlerMiddleware> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex) when (context.RequestAborted.IsCancellationRequested)
        {
            _logger.LogDebug(ex, "Request was cancelled.");
            context.Response.StatusCode = StatusCodes.Status499ClientClosedRequest;
            context.Response.ContentType = "application/json";
            var problemDetail = GetProblemDetail(context, ex);
            await context.Response.WriteAsync(problemDetail.ToJson());
        }
        catch (Exception ex)
        {
            ex.AddErrorCode();
            _logger.LogError(ex, UnhandleExceptionMessage);
            context.Response.Clear();
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            var problemDetail=GetProblemDetail(context,ex); 
            var jsonStr= problemDetail.ToJson();
            await context.Response.WriteAsync(jsonStr);
        }
    }

    public ProblemDetails GetProblemDetail(
        in HttpContext context,
        in Exception exception)
    {
        var errorCode = exception.GetErrorCode();
        var statusCode = context.Response.StatusCode;
        var reasonPhase = ReasonPhrases.GetReasonPhrase(statusCode);
        if (string.IsNullOrEmpty(reasonPhase))
        {
            reasonPhase = UnhandleExceptionMessage;
        }
        var problemDetail = new ProblemDetails
        {
            Status = statusCode,
            Title = reasonPhase,
            Extensions =
            {
               [nameof(errorCode)] = errorCode,
            }

        };
        if (!_env.IsDevelopment())
        {
            return problemDetail;
        }
        problemDetail.Detail = exception.Message;
        problemDetail.Extensions["treceId"] = context.TraceIdentifier;
        problemDetail.Extensions["data"] = exception.Data;

        return problemDetail;
    }
}
