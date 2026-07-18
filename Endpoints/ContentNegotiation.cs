using System.Xml.Serialization;

namespace Shortly.Endpoints;

public class NegotiatedResult<T> : IResult
{
    private readonly T _data;
    private readonly int _statusCode;

    public NegotiatedResult(T data, int statusCode)
    {
        _data = data;
        _statusCode = statusCode;
    }

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        var accept = httpContext.Request.Headers.Accept.ToString();

        bool wantsXml = accept.Contains("application/xml") || accept.Contains("text/xml");
        bool wantsJson = string.IsNullOrWhiteSpace(accept)
                         || accept.Contains("*/*")
                         || accept.Contains("application/json");

        if (wantsJson)
        {
            httpContext.Response.StatusCode = _statusCode;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsJsonAsync(_data);
            return;
        }

        if (wantsXml)
        {
            httpContext.Response.StatusCode = _statusCode;
            httpContext.Response.ContentType = "application/xml";
            var serializer = new XmlSerializer(typeof(T));
            using var ms = new MemoryStream();
            serializer.Serialize(ms, _data);
            await httpContext.Response.Body.WriteAsync(ms.ToArray());
            return;
        }

        httpContext.Response.StatusCode = StatusCodes.Status406NotAcceptable;
    }
}

public static class NegotiationExtensions
{
    public static IResult Negotiated<T>(this T data, int statusCode = StatusCodes.Status200OK)
        => new NegotiatedResult<T>(data, statusCode);

    public static async Task<T?> ReadNegotiatedBody<T>(this HttpRequest request)
    {
        var contentType = request.ContentType ?? "";

        if (contentType.Contains("xml"))
        {
            var serializer = new XmlSerializer(typeof(T));
            return (T?)serializer.Deserialize(request.Body);
        }

        return await request.ReadFromJsonAsync<T>();
    }
}