namespace DotNet8.ExceptionHandlerMiddleware.Api.Extensions;

public static class DevCode
{
    private const string ErrorCodeKey = "errorCode";

    public static Exception AddErrorCode(this Exception exception)
    {
        using var sha1=SHA1.Create();
        var hash=sha1.ComputeHash(Encoding.UTF8.GetBytes(exception.Message));
        var errorCode = string.Concat(hash[..5].Select(b => b.ToString("x")));
        exception.Data[ErrorCodeKey] = errorCode;
        return exception;
    }

    public static string GetErrorCode(this Exception exception)
    {
        return (string)exception.Data[ErrorCodeKey]!;
    }

    public static string ToJson(this object obj)
    {
        return JsonConvert.SerializeObject(obj);
    }
}
