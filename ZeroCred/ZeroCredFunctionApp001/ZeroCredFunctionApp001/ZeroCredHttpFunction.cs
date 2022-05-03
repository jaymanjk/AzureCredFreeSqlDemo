using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;
using System;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace ZeroCredFunctionApp001
{
    public static class ZeroCredHttpFunction
    {
        [FunctionName("ZeroCredHttpFunction001")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                var kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessToken));

                // Get the secret                
                var secretUrl = "https://functionappkeyvault001.vault.azure.net/secrets/ZeroCredFunctionApp001Secret/314314583dc84ba299f2d846b5e6fee9";
                var secret = await kv.GetSecretAsync(secretUrl);

                var myName = secret.Value;

                return myName != null
                    ? (ActionResult)new OkObjectResult($"The secret value for Jayssecret1 is {myName}")
                    : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
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

            //var clientId = "ae7fa6a3-ddde-42f7-b1b2-ae814fecd711";
            //var clientSecret = "cx_8Q~axz9lwmDNT21xKDMAuGeoA8dJ~JLAWrciy";

            //var authenticationContext = new AuthenticationContext(authority);

            //var cCreds = new ClientCredential(clientId, clientSecret);

            //AuthenticationResult result = await authenticationContext.AcquireTokenAsync(resource, cCreds);

            //return result.AccessToken;
        }
    }
}
