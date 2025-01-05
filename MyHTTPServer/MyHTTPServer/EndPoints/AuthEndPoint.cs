using System.Web;
using HttpServerLibrary;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.HttpResponce;
using MyHTTPServer.models;

namespace MyHTTPServer.EndPoints;

public class AuthEndPoint : BaseEndPoint
{
    [Get("login")]
    public IHttpResponceResult AuthGet()
    {
        var file = File.ReadAllText(
            @"Templates/Pages/Auth/login.html"); //$"{Directory.GetCurrentDirectory()}\\{AppConfig.StaticDirectoryPath}\\theme\\templates\\admin\\login.html");
        return Html(file);
    }

    [Post("login")] // string email. string password
    public IHttpResponceResult AuthPost(string email, string password)
    {
        Console.WriteLine($"!!!!!!!!!!!!!{email} || {password}");
        string connectionString =
            @"Server=localhost; Database=myDB; User Id=sa; Password=P@ssw0rd;TrustServerCertificate=true;";

        var dBcontext = new ORMContext(connectionString);
        var user = dBcontext.ReadByEmail<User>(email);

        if (user != null)
        {
            Console.WriteLine($"User {user} was found");
            var file = File.ReadAllText(
                @"Templates/Pages/Dashboard/index.html"); //$"{Directory.GetCurrentDirectory()}\\{AppConfig.StaticDirectoryPath}\\theme\\templates\\admin\\login.html");
            return Html(file);
        }

        Console.WriteLine($"User with email: {email} was not found");
        return Html("<h1>404</h1>");


        // connection string
        // var connection 
        // vat context = new ORMContext(connectionString);

        // if (user == null) return redirect("login")

        // 
    }
}