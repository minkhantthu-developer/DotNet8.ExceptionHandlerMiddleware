<h3>Exception Handling Wiht Middleware </h3>

- [Source](https://medium.com/@AntonAntonov88/how-processing-unhandled-exceptions-in-asp-net-core-web-api-14cd8e871229)

```cs
public class ProblemDetailModel
{
    public int StatusCode { get; set; }

    public string Title { get; set; }

    public string Detail { get; set; }

    public IDictionary<string, object> Extenstions { get; set; } =new Dictionary<string, object>();
}
```