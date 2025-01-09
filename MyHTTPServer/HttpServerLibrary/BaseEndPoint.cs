using System.Net;
using System.Text;
using HttpServerLibrary.HttpResponce;
using HttpServerLibrary.HttpResponse;


namespace HttpServerLibrary;

public class BaseEndPoint
{
    public HttpRequestContext Context { get; private set; }

    internal void SetContext(HttpRequestContext context)
    {
        Context = context;
    }

    public static IHttpResponceResult Html(string responceText) => new HTMLResult(responceText);
    public IHttpResponceResult Json(object data) => new JsonResult(data);
    
    public RedirectResponse Redirect(string route) => new RedirectResponse(route);
}