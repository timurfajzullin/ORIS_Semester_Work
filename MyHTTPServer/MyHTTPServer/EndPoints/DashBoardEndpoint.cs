using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection.Metadata;
using HttpServerLibrary;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.HttpResponce;
using Microsoft.Data.SqlClient;
using MyHTTPServer.Sessions;
using MyHtttpServer.Core.Templator;
using MyHttttpServer.Models;
using MyORMLibrary;

namespace MyHTTPServer.EndPoints
{
    public class DashBoardEndpoint : BaseEndPoint
    {
        [Get("dashboard")]
        public IHttpResponceResult DashboardGet()
        {
            var templator = new CustomTemplator();
            var dashboardHtml = File.ReadAllText(@"Templates/Pages/Dashboard/index.html");
            
            if (IsAuthorized(Context))
            {
                return Html(templator.GetUserHtml(dashboardHtml));
            }
            return Html(dashboardHtml);
        }

        /// <summary>
        /// Проверяет, авторизован ли пользователь.
        /// </summary>
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
            return Json(isAuthorized);
        }
        
        [Get("dashboard/GetUrls")]
        public IHttpResponceResult GetUrls(string? country, string? genre, string? year, string? privacy)
        {
            string connectionString = @"Data Source=localhost; User ID=sa;Password=P@ssw0rd; TrustServerCertificate=true;";
            var connection = new SqlConnection(connectionString);
            var dBcontext = new ORMContext<Movie>(connection);

            // Начинаем с базового запроса
            var query = "SELECT PosterURL FROM DBWithAllMovies";

            List<string> conditions = new List<string>();
            
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
                conditions.Add($"Year = {int.Parse(year)}");
            }
            
            if (!string.IsNullOrEmpty(privacy))
            {
                conditions.Add($"Privacy = N'{privacy}'");
            }
            
            // Если есть условия, добавляем их к запросу
            if (conditions.Count > 0)
            {
                query += " WHERE " + string.Join(" AND ", conditions);
                query += ';';
            }
            
            var urls = PutURLToTemplate(dBcontext.TakeURLByData(query));
    
            return Json(urls);
        }
        

        private List<string> PutURLToTemplate(List<string> urls)
        {
            var result = new List<string>();
            foreach (var url in urls)
            {
                
                result.Add(CustomTemplator.GetHtmlByTemplateWithURL(url));
            }

            return result;
        }
    }
}
