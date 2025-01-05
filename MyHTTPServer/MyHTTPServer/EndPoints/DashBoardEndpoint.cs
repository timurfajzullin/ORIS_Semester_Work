using System.Net.Http.Headers;
using System.Text;
using System.Web;
using HttpServerLibrary;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.Configuration;
using HttpServerLibrary.HttpResponce;

namespace MyHTTPServer.EndPoints;

public class DashBoardEndpoint : BaseEndPoint
{
    [Get("dashboard")]
    public IHttpResponceResult DashBoardGet()
    {
        var file = File.ReadAllText(@"Templates/Pages/Dashboard/index.html"); //$"{Directory.GetCurrentDirectory()}\\{AppConfig.StaticDirectoryPath}\\theme\\templates\\admin\\login.html");
        return Html(file);
    }
}