using System.Text.Encodings.Web;
using HttpServerLibrary;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.HttpResponce;
using Microsoft.Data.SqlClient;
using MyHttttpServer.Models;
using MyORMLibrary;

namespace MyHTTPServer.EndPoints;

public class AdminEndPoint : BaseEndPoint
{
    [Get("admin")]
    public IHttpResponceResult GetAdmin()
    {
        var file = File.ReadAllText(@"Templates/Pages/AdminPanel/AdminPanel.html");
        return Html(file);
    }
    [Get("admin/GetInformation")]
    public IHttpResponceResult GetInformation(string name, string posterurl, string? country, string? genre, string? year, string? privacy)
    {
        string connectionString = @"Data Source=localhost; User ID=sa;Password=P@ssw0rd; TrustServerCertificate=true;";
        var connection = new SqlConnection(connectionString);
        var dBcontext = new ORMContext<Movie>(connection);

        // Список условий для WHERE
        List<string> conditions = new List<string>();

        if (!string.IsNullOrEmpty(name))
        {
            conditions.Add($"Name = N'{name}'");
        }
        if (!string.IsNullOrEmpty(posterurl))
        {
            conditions.Add($"PosterURL = N'{posterurl}'");
        }
        if (!string.IsNullOrEmpty(country))
        {
            conditions.Add($"Country = N'{country}'");
        }
        if (!string.IsNullOrEmpty(genre))
        {
            conditions.Add($"Genre = N'{genre}'");
        }
        if (!string.IsNullOrEmpty(year))
        {
            if (int.TryParse(year, out int parsedYear))
            {
                conditions.Add($"Year = {parsedYear}");
            }
        }
        if (!string.IsNullOrEmpty(privacy))
        {
            conditions.Add($"Privacy = N'{privacy}'");
        }

        // Формируем запрос
        string query = "SELECT * FROM DBWithAllMovies";
        if (conditions.Count > 0)
        {
            query += " WHERE " + string.Join(" AND ", conditions);
        }
        var result = dBcontext.GetMovies(query);
        return Json(result);
    }
    
    [Get("admin/AddInformation")]
    public IHttpResponceResult AddInformation(string name, string posterurl, string country, string year, string privacy, string genre)
    {
        string connectionString = @"Data Source=localhost; User ID=sa;Password=P@ssw0rd; TrustServerCertificate=true;";
        var connection = new SqlConnection(connectionString);
        var dBcontext = new ORMContext<Movie>(connection);
        int.TryParse(year, out int parsedYear);
        string query = $"INSERT INTO DBWithAllMovies (name, posterurl, country, year, genre, privacy) VALUES (N'{name}', N'{posterurl}', N'{country}', {parsedYear} , N'{genre}',  N'{privacy}');";
        Console.WriteLine(query);
        var result = dBcontext.AddMovie(query);
        return Json(result);
    }

    [Get("admin/DeleteInformation")]
    public IHttpResponceResult DeleteInformation(string name, string posterurl, string country, string year, string privacy, string genre)
    {
        string connectionString = @"Data Source=localhost; User ID=sa;Password=P@ssw0rd; TrustServerCertificate=true;";
        var connection = new SqlConnection(connectionString);
        var dBcontext = new ORMContext<Movie>(connection);
        List<string> conditions = new List<string>();

        if (!string.IsNullOrEmpty(name))
        {
            conditions.Add($"Name = N'{name}'");
        }
        if (!string.IsNullOrEmpty(posterurl))
        {
            conditions.Add($"PosterURL = N'{posterurl}'");
        }
        if (!string.IsNullOrEmpty(country))
        {
            conditions.Add($"Country = N'{country}'");
        }
        if (!string.IsNullOrEmpty(genre))
        {
            conditions.Add($"Genre = N'{genre}'");
        }
        if (!string.IsNullOrEmpty(year))
        {
            if (int.TryParse(year, out int parsedYear))
            {
                conditions.Add($"Year = {parsedYear}");
            }
        }
        if (!string.IsNullOrEmpty(privacy))
        {
            conditions.Add($"Privacy = N'{privacy}'");
        }

        // Формируем запрос
        string query = $"DELETE FROM DBWithAllMovies";
        if (conditions.Count > 0)
        {
            query += " WHERE " + string.Join(" AND ", conditions);
        }
        
        var result = dBcontext.DeleteMovie(query);

        return Json(result);
    }

    [Get("admin/AddMoviePageInformation")]
    public void GetMoviePageInformation(string name, string posterurl, string production, string description, string starring)
    {
        string connectionString = @"Data Source=localhost; User ID=sa;Password=P@ssw0rd; TrustServerCertificate=true;";
        var connection = new SqlConnection(connectionString);
        var dBcontext = new ORMContext<Movie>(connection);
        var query = $"INSERT INTO MoviePageInformation (Name, PosterURL, Production, Description, Starring) VALUES (N'{name}', '{posterurl}', N'{production}', N'{description}', N'{starring}');";
        dBcontext.AddMovieInfo(query);
    }

    [Get("admin/AddComment")]
    public IHttpResponceResult AddComment(string username, string textarea, string datetime)
    {
        string connectionString = @"Data Source=localhost; User ID=sa;Password=P@ssw0rd; TrustServerCertificate=true;";
        var connection = new SqlConnection(connectionString);
        var dBcontext = new ORMContext<Movie>(connection);
        var query = $"INSERT INTO Comments (UserName, Text, DateTime) VALUES (N'{username}', N'{textarea}', '{datetime}');";
        dBcontext.AddComment(query);
        return Json(null);
    }
}