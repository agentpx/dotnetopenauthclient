using DotNetOpenAuth.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

/*
GET /oauth2/v3/userinfo HTTP/1.1
Host: www.googleapis.com
Content-length: 0
Authorization: OAuth ya29.AHES6ZTpUruN2eK1hHhMa-Dv0FC0deakE8JF4icjjumCOXYTQPkgfg
 * 
 * 
 * 
HTTP/1.1 200 OK
Content-length: 411
X-xss-protection: 1; mode=block
Content-location: https://www.googleapis.com/oauth2/v3/userinfo
X-content-type-options: nosniff
X-google-cache-control: remote-fetch
-content-encoding: gzip
Server: GSE
Via: HTTP/1.1 GWA
Pragma: no-cache
Cache-control: no-cache, no-store, max-age=0, must-revalidate
Date: Sun, 02 Jun 2013 08:52:07 GMT
X-frame-options: SAMEORIGIN
Content-type: application/json; charset=UTF-8
Expires: Fri, 01 Jan 1990 00:00:00 GMT
 * 
 * 
 * 
{
 "sub": "106351706100172337502",
 "name": "Jorge Alvarado",
 "given_name": "Jorge",
 "family_name": "Alvarado",
 "profile": "https://plus.google.com/106351706100172337502",
 "picture": "https://lh5.googleusercontent.com/-fAw41YO7RAk/AAAAAAAAAAI/AAAAAAAAAAA/4sIxPR_mfOI/photo.jpg",
 "email": "alvaradojl1986@gmail.com",
 "email_verified": true,
 "gender": "male",
 "birthdate": "0000-10-08",
 "locale": "en"
}
 */



namespace GoogleOAuth2Client
{



    public class GoogleClient : WebServerClient
    {
        private static readonly AuthorizationServerDescription GoogleDescription = new AuthorizationServerDescription
        {
            TokenEndpoint = new Uri("https://accounts.google.com/o/oauth2/token"),
            AuthorizationEndpoint = new Uri("https://accounts.google.com/o/oauth2/auth"),
            //// RevokeEndpoint = new Uri("https://accounts.google.com/o/oauth2/revoke"),
            ProtocolVersion = ProtocolVersion.V20
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleClient"/> class.
        /// </summary>
        public GoogleClient()
            : base(GoogleDescription)
        {
        }

        public IOAuth2Graph GetGraph(IAuthorizationState authState, string[] fields = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if ((authState != null) && (authState.AccessToken != null))
            {
                HttpClient httpClient = new HttpClient(this.CreateAuthorizingHandler(authState));
              
                using (var response = httpClient.GetAsync("https://www.googleapis.com/oauth2/v1/userinfo", cancellationToken).Result)
                {
                    response.EnsureSuccessStatusCode();
                    using (var responseStream = response.Content.ReadAsStreamAsync().Result)
                    {
                        return GoogleGraph.Deserialize(responseStream);
                    }
                }
            }

            return null;
        }


        public static class Scopes
        {
            /// <summary>
            /// Scopes that cover queries for user data.
            /// </summary>
            public static class UserInfo
            {
                /// <summary>
                /// Gain read-only access to basic profile information, including a user identifier, name, profile photo, profile URL, country, language, timezone, and birthdate.
                /// </summary>
                public const string Profile = "https://www.googleapis.com/auth/userinfo.profile";

                /// <summary>
                /// Gain read-only access to the user's email address.
                /// </summary>
                public const string Email = "https://www.googleapis.com/auth/userinfo.email";
            }
        }

        

    }
}