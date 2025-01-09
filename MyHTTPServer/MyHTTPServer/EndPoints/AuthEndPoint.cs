using System.Net;
using System.Web;
using HttpServerLibrary;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.HttpResponce;
using Microsoft.Data.SqlClient;
using MyHTTPServer.Sessions;
using MyHttttpServer.Models;
using MyORMLibrary;

namespace MyHTTPServer.EndPoints;

public class AuthEndPoint : BaseEndPoint
{
    [Get("login")]
    public IHttpResponceResult AuthGet()
    {
        var file = File.ReadAllText(@"Templates/Pages/Auth/login.html");
        if (IsAuthorized(Context)) return Redirect("dashboard");
        return Html(file);
    }

    [Post("login")]
    public IHttpResponceResult AuthPost(string email, string password)
    {
        // Подключение к базе данных
        string connectionString = @"Data Source=localhost; User ID=sa;Password=P@ssw0rd; TrustServerCertificate=true;";
        var connection = new SqlConnection(connectionString);
        var dBcontext = new ORMContext<User>(connection);
        var user = dBcontext.FirstOrDefault(u => u.Email == email && u.Password == password);

        // Если пользователь не найден, перенаправляем на страницу входа
        if (user == null || user.Password != password) // Добавлена проверка пароля
        {
            Console.WriteLine("Authentication failed for email: " + email);
            return Redirect("login");
        }

        // Генерация уникального токена
        string token = Guid.NewGuid().ToString();

        // Сохранение токена в сессии
        SessionStorage.SaveSession(token, user.Id);
        Console.WriteLine($"User {user.Email} authenticated successfully with token: {token}");

        // Установка cookie для хранения токена
        Cookie sessionCookie = new Cookie("session-token", token)
        {
            HttpOnly = true,
            Path = "/",
            Expires = DateTime.Now.AddHours(5)
        };
        Context.Response.SetCookie(sessionCookie);

        // Перенаправление на dashboard
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