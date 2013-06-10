using DotNetOpenAuth.OAuth2;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace DNOAClient.Code
{
    public class AlhambraWebServerClient : WebServerClient
    {

       

        private static readonly AuthorizationServerDescription AlhambraDescription = new AuthorizationServerDescription
        {
<<<<<<< HEAD
            TokenEndpoint = new Uri(SERVER_ADDRESS + "/OpenIdConnect/Token"),
            AuthorizationEndpoint = new Uri(SERVER_ADDRESS + "/OpenIdConnect/Auth"),
=======
            TokenEndpoint = new Uri(Config.SERVER_ADDRESS + "/OpenIdConnect/Token"),
            AuthorizationEndpoint = new Uri(Config.SERVER_ADDRESS + "/OpenIdConnect/Auth"),
>>>>>>> completed openidconnect validations
            //// RevokeEndpoint = new Uri( SERVER_ADDRESS +  "/OAuth2/Revoke"),
            ProtocolVersion = ProtocolVersion.V20
        };

        public AlhambraWebServerClient()
            : base(AlhambraDescription)
        {
            //ClientIdentifier = ConfigurationManager.AppSettings["alhambraIdentifier"];
            //ClientCredentialApplicator = ClientCredentialApplicator.NetworkCredential(ConfigurationManager.AppSettings["alhambraSecret"]);
            //ClientCredentialApplicator = ClientCredentialApplicator.PostParameter(ConfigurationManager.AppSettings["alhambraSecret"]);
        }

        public OAuth2Graph GetUserInfo(string authToken)
        {
<<<<<<< HEAD
            var userInfoUrl = SERVER_ADDRESS + "/OpenIdConnect/UserInfo";
=======
            var userInfoUrl = Config.SERVER_ADDRESS + "/OpenIdConnect/UserInfo";
>>>>>>> completed openidconnect validations
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer2", authToken);
            var response = httpClient.GetAsync(userInfoUrl).Result;
            OAuth2Graph userInfo = response.Content.ReadAsAsync<OAuth2Graph>().Result;
            return userInfo;
        }

        //public OAuth2TokenInfo GetTokenInfo(string accessToken)
        //{
        //    var verificationUri = SERVER_ADDRESS + "/OAuth2/TokenInfo?access_token=" + accessToken;
        //    var httpClient = new HttpClient();

        //    var response = httpClient.GetAsync(verificationUri).Result;
        //    OAuth2TokenInfo tokenInfo = response.Content.ReadAsAsync<OAuth2TokenInfo>().Result;
        //    return tokenInfo;
        //}

 

        

    }
}