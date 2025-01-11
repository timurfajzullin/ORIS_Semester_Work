using HttpServerLibrary;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.HttpResponce;
using Microsoft.Data.SqlClient;
using MyHTTPServer.Sessions;
using MyHtttpServer.Core.Templator;
using MyHttttpServer.Models;
using MyORMLibrary;

namespace MyHTTPServer.EndPoints;

public class MovieEndPoint : BaseEndPoint
{
    [Get("movie")]
    public IHttpResponceResult GetMovie()
    {
        
        var templator = new CustomTemplator();
        var moviePage = File.ReadAllText(@"Templates/Pages/MoviePage/MoviePage.html");
        string connectionString = @"Data Source=localhost; User ID=sa;Password=P@ssw0rd; TrustServerCertificate=true;";
        var connection = new SqlConnection(connectionString);
        var dBcontext = new ORMContext<Movie>(connection);
        var result = dBcontext.ReadByAll<Movie>();
        
        
        if (IsAuthorized(Context))
        {
            return Html(templator.GetMovieHtml(templator.GetHtmlByTemplateFilmPageData(result, moviePage)));
        }
        return Html(templator.GetHtmlByTemplateFilmPageData(result, moviePage));
    }
    
    public bool IsAuthorized(HttpRequestContext context)
    {
        // Проверка наличия Cookie с session-token
        if (context.Request.Cookies.Any(c => c.Name == "session-token"))
        {
            var cookie = context.Request.Cookies["session-token"];
            return SessionStorage.ValidateToken(cookie.Value);
        }

        return false;
    }
    [Get("dashboard/CheckAuthorization")]
    public IHttpResponceResult CheckAuthorization()
    {
        bool isAuthorized = IsAuthorized(Context);
        return Json(new { isAuthorized });
    }
    [Get("movie/GetUserLogin")]
    public IHttpResponceResult GetUserLogin()
    {
        string connectionString = @"Data Source=localhost; User ID=sa;Password=P@ssw0rd; TrustServerCertificate=true;";
        var connection = new SqlConnection(connectionString);
        var dBcontext = new ORMContext<Movie>(connection);
        var query = "SELECT name FROM users";
        var result = dBcontext.TakeFieldByData(query);
        return Json(result);
    }
}