using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Services.AppAuthentication;
using SqlCredFreeMvcApp001.Models;
using System.Data.SqlClient;
using System.Diagnostics;

namespace SqlCredFreeMvcApp001.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private List<CountryCapitalInfo> caitalInfo = new List<CountryCapitalInfo>();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            //            var connectingString = @"Server=tcp:credfreedemosqlsrv001.database.windows.net,1433;
            //Initial Catalog=credfreedemosqldb001;
            //Persist Security Info=False;
            //User ID=credfreeadmin;
            //Password=Pa55wo$d;
            //MultipleActiveResultSets=False;
            //Encrypt=True;
            //TrustServerCertificate=False;
            //Connection Timeout=30;";
            var connectingString = @"Server=tcp:credfreedemosqlsrv001.database.windows.net,1433;
Initial Catalog=credfreedemosqldb001;
Persist Security Info=False;
MultipleActiveResultSets=False;
Encrypt=True;
TrustServerCertificate=False;
Connection Timeout=30;";

            var capitals = new Dictionary<string, string>();

            using (var sqlConnection = new SqlConnection(connectingString))
            {
                var sqlCommand = new SqlCommand("SELECT Country, Capital FROM CountryInfo", sqlConnection);

                var accessToken = (new AzureServiceTokenProvider()).GetAccessTokenAsync("https://database.windows.net/").Result;
                sqlConnection.AccessToken = accessToken;

                sqlConnection.Open();

                var reader = sqlCommand.ExecuteReader();

                if (reader != null)
                {
                    while (reader.Read())
                    {
                        this.caitalInfo.Add(new CountryCapitalInfo()
                        {
                            Country = !reader.IsDBNull(0) ? reader["Country"].ToString() : "",
                            Capital = !reader.IsDBNull(0) ? reader["Capital"].ToString() : ""
                        });
                    }
                }

                sqlConnection.Close();
            }

            ////return capitals;
            return View(caitalInfo);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}