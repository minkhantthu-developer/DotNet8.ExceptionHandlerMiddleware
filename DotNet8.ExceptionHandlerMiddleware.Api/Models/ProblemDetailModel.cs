namespace DotNet8.ExceptionHandlerMiddleware.Api.Models
{
    public class ProblemDetailModel
    {
        public int StatusCode { get; set; }

        public string Title { get; set; }

        public string Detail { get; set; }

        public IDictionary<string, object> Extenstions { get; set; } =new Dictionary<string, object>();
    }
}
