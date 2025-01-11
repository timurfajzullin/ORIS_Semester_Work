using System.Net;
using System.Text.Encodings.Web;
using System.Web;
using HttpServerLibrary;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.HttpResponce;
using Microsoft.Data.SqlClient;
using MyHTTPServer.Sessions;
using MyHttttpServer.Models;
using MyORMLibrary;
namespace MyHTTPServer.EndPoints;

public class RegistrationEndPoint : BaseEndPoint
{
    [Get("registration")]
    public IHttpResponceResult GetRegistration()
    {
        var file = File.ReadAllText(@"Templates/Pages/Registration/registrationPage.html");
        if (IsAuthorized(Context)) return Redirect("dashboard");
        return Html(file);
    }

    
    [Post("registration")]
    public IHttpResponceResult PostRegistration(string name, string email, string password)
    {
        string connectionString = @"Data Source=localhost; User ID=sa;Password=P@ssw0rd; TrustServerCertificate=true;";
        var connection = new SqlConnection(connectionString);
        var dBcontext = new ORMContext<User>(connection);
        var user = new User
        {
            Name = name,
            Email = email,
            Password = password
        };
        var u = dBcontext.Create(user);
        
        string token = Guid.NewGuid().ToString();

        // Сохранение токена в сессии
        SessionStorage.SaveSession(token, user.Id);
        Console.WriteLine($"User {user.Name} {user.Email} authenticated successfully with token: {token}");

        // Установка cookie для хранения токена
        Cookie sessionCookie = new Cookie("session-token", token)
        {
            HttpOnly = true,
            Path = "/",
            Expires = DateTime.Now.AddHours(5)
        };
        Context.Response.SetCookie(sessionCookie);
        
        return Redirect("dashboard");
    }

    public bool IsAuthorized(HttpRequestContext context)
    {
        // Проверка наличия Cookie с session-token
        if (context.Request.Cookies.Any(c=> c.Name == "session-token"))
        {
            var cookie = context.Request.Cookies["session-token"];
            return SessionStorage.ValidateToken(cookie.Value);
        }
         
        return false;
    }
}