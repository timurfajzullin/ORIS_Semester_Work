using System.Net;
using HttpServerLibrary;
using HttpServerLibrary.HttpResponce;

public class RedirectResult //: IHttpResponceResult
{
    private readonly string _location;
    public RedirectResult(string location)
    {
        _location = location;
    }
 
    public void Execute(HttpListenerResponse context)
    {
        //var response = context.Response;
        context.StatusCode = 302;
        context.Headers.Add("Location", _location); // Заголовок для указания пути
    }
}