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
            TokenEndpoint = new Uri("https://localhost:44300/OAuth2/Token"),
            AuthorizationEndpoint = new Uri("https://localhost:44300/OAuth2/Auth"),
            //// RevokeEndpoint = new Uri("https://localhost:44300/OAuth2/Revoke"),
            ProtocolVersion = ProtocolVersion.V20
        };

        public AlhambraWebServerClient()
            : base(AlhambraDescription)
        {
            ClientIdentifier = ConfigurationManager.AppSettings["alhambraIdentifier"];
            ClientCredentialApplicator.PostParameter(ConfigurationManager.AppSettings["alhambraSecret"]);
        }

        public OAuth2Graph GetUserInfo(string authToken)
        {
            var userInfoUrl = "https://localhost:44300/OAuth2/UserInfo";
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);
            var response = httpClient.GetAsync(userInfoUrl).Result;
            OAuth2Graph userInfo = response.Content.ReadAsAsync<OAuth2Graph>().Result;
            return userInfo;
        }

        public OAuth2TokenInfo GetTokenInfo(string accessToken)
        {
            var verificationUri = "https://localhost:44300/OAuth2/TokenInfo?access_token=" + accessToken;
            var httpClient = new HttpClient();

            var response = httpClient.GetAsync(verificationUri).Result;
            OAuth2TokenInfo tokenInfo = response.Content.ReadAsAsync<OAuth2TokenInfo>().Result;
            return tokenInfo;
        }

        public static class Scopes
        {
            /// <summary>
            /// Scopes that cover queries for user data.
            /// </summary>
            public static class ConnectId
            {
                /// <summary>
                /// Complies with the OpenId Connect Protocol
                /// </summary>
                public const string OpenId = "openId";


                public const string Subject = "sub";

                public const string FullName = "name";

                public const string FirstName = "given_name";

                public const string LastName = "family_name";

                /// <summary>
                /// Gain read-only access to basic profile information.
                /// </summary>
                public const string Profile = "profile";

                /// <summary>
                /// Gain read-only access to the user's email address.
                /// </summary>
                public const string Email = "email";
            }
        }

        

    }
}