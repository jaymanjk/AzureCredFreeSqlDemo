using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;

namespace ZeroCredKeyValutDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IConfiguration _config;

        public WeatherForecastController(IConfiguration config, ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
            _config = config;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("GetMySecret", Name = "GetMySecret")]
        public IActionResult GetMySecret()
        {
            try
            {
                bool useKeyValut = this._config.GetValue<bool>("AppSettings:ConnectToKeyValut");
                if (useKeyValut)
                {

                    var kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessToken));

                    // Get the secret

                    var secretUrl = "https://testkeyvalut001.vault.azure.net/secrets/MySecret/e645a5c8d0564c93a5f55793c040df06";
                    var secret = kv.GetSecretAsync(secretUrl).Result;

                    var myName = secret.Value;

                    return myName != null
                        ? (ActionResult)new OkObjectResult($"Hello, {myName}")
                        : new OkObjectResult("Hello, My static secret from API Controller");
                }
                else
                {
                    return new OkObjectResult("Hello, My static secret from API Controller");
                }
            }
            catch (Exception ex)
            {
                return (ActionResult)new OkObjectResult(ex.Message);
            }
        }

        private static async Task<string> GetAccessToken(string authority, string resource, string scope)
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            string accessToken = await azureServiceTokenProvider.GetAccessTokenAsync("https://vault.azure.net");

            return accessToken;

            //var clientId = "1389cdbd-8c73-42df-80f9-0cda965863ed";
            //var clientSecret = "9gF8UK8m.ZCGlsweo3ls@YF.lxk@/S2J";

            //var authenticationContext = new AuthenticationContext(authority);

            //var cCreds = new ClientCredential(clientId, clientSecret);

            //AuthenticationResult result = await authenticationContext.AcquireTokenAsync(resource, cCreds);

            //return result.AccessToken;
        }
    }
}