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
            return Json(new { isAuthorized });
        }
        
        [Get("dashboard/GetUrls")]
        public IHttpResponceResult GetUrls()
        {
            string connectionString = @"Data Source=localhost; User ID=sa;Password=P@ssw0rd; TrustServerCertificate=true;";
            var connection = new SqlConnection(connectionString);
            var dBcontext = new ORMContext<Movie>(connection);
            var query = "SELECT PosterURL FROM Movies"; // SQL-запрос для получения URL из таблицы Movies
            var urls = PutURLToTemplate(dBcontext.TakeURLByData(query));
            return Json(urls);
        }
        
        [Get("dashboard/GetMoviesWithCountryFilter")]
        public IHttpResponceResult GetMoviesWithCountryFilter(string country)
        {
            string connectionString = @"Data Source=localhost; User ID=sa;Password=P@ssw0rd; TrustServerCertificate=true;";
            var connection = new SqlConnection(connectionString);
            var dBcontext = new ORMContext<Movie>(connection);

            if (country == "Россия")
            {
                var queryR = "SELECT PosterURL FROM MoviesWithCountryFilter WHERE Country = 'russia';";
                var urlsR = PutURLToTemplate(dBcontext.TakeURLByData(queryR));
                return Json(urlsR);
            }
            
            var queryU = "SELECT PosterURL FROM MoviesWithCountryFilter WHERE Country = 'usa';";
            var urlsU = PutURLToTemplate(dBcontext.TakeURLByData(queryU));
            return Json(urlsU);
        }
        
        [Get("dashboard/GetMoviesWithPrivacyFilter")]
        public IHttpResponceResult GetMoviesWithPrivacyFilter(string privacy)
        {
            string connectionString = @"Data Source=localhost; User ID=sa;Password=P@ssw0rd; TrustServerCertificate=true;";
            var connection = new SqlConnection(connectionString);
            var dBcontext = new ORMContext<Movie>(connection);

            if (privacy == "free")
            {
                var queryR = "SELECT PosterURL FROM PrivacyMovies WHERE Privacy = 'free';";
                var urlsR = PutURLToTemplate(dBcontext.TakeURLByData(queryR));
                return Json(urlsR);
                
            }
            
            var queryU =  "SELECT PosterURL FROM PrivacyMovies WHERE Privacy = 'privacy';";
            var urlsU = PutURLToTemplate(dBcontext.TakeURLByData(queryU));
            return Json(urlsU);
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
