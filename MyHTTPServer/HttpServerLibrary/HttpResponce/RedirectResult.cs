using System.Net;
using HttpServerLibrary.HttpResponce;

namespace HttpServerLibrary.HttpResponse;

public class RedirectResponse: IHttpResponceResult
{
    private readonly string _redirectUrl;

    public RedirectResponse(string redirectUrl)
    {
        _redirectUrl = redirectUrl;
    }

    public void Execute(HttpListenerResponse response)
    {
        response.Redirect(_redirectUrl);
        response.StatusCode = (int)HttpStatusCode.Redirect;
        response.OutputStream.Close();
    }
}