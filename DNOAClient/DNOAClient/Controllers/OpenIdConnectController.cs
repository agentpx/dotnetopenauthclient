using DNOAClient.Code;
using DotNetOpenAuth.OAuth2;
using Microsoft.IdentityModel.Tokens.JWT;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using DotNetOpenAuth.Messaging;
using System.Net.Http.Headers;
using DNOAClient.Models;
using System.Text;

namespace DNOAClient.Controllers
{
    public class OpenIdConnectController : Controller
    {
        // private IAuthorizationState authorizationState = null;

        private static readonly AlhambraWebServerClient client = new AlhambraWebServerClient
        {
            ClientIdentifier = ConfigurationManager.AppSettings["alhambraIdentifier"],
            ClientCredentialApplicator = ClientCredentialApplicator.NetworkCredential(ConfigurationManager.AppSettings["alhambraIdentifier"])
            // ClientCredentialApplicator = ClientCredentialApplicator.PostParameter(ConfigurationManager.AppSettings["login123"]),
        };




        //  [Authorize]
        public ActionResult InitAuthAlhambra()
        {

            var state = new AuthorizationState();

            state.Callback = new Uri(Config.CLIENT_ADDRESS + "/OpenIdConnect/AlhambraCallback");
            state.Scope.Add(OpenIdConnectScopes.OpenId);
        //    state.Scope.Add(OpenIdConnectScopes.OfflineAccess);
            state.Scope.Add(OpenIdConnectScopes.Profile);

            var r = client.PrepareRequestUserAuthorization(state);

            return r.AsActionResult();
        }



        private static string RemoveQueryStringFromUri(string uri)
        {
            int index = uri.IndexOf("?");
            if (index > -1)
            {
                uri = uri.Substring(0, index);
            }
            return uri;
        }

        //  [Authorize]
        public ActionResult AlhambraCallback(AlhambraCallbackInput input)
        {
            //System.Web.HttpContext.Current.Application["Authorization"]  = (AuthorizationState) client.ProcessUserAuthorization(this.Request);
            //AuthorizationState auth = (AuthorizationState)client.ProcessUserAuthorization(this.Request);
            // System.Web.HttpContext.Current.Application["Authorization"] = auth;


            // CurrentAuthorizationState = auth;

            //string code = Request.QueryString["code"];
            //System.Web.HttpContext.Current.Application.Add("Code",Request.QueryString["code"]);

            //string accessToken = Request.QueryString["access_token"];
            //authorizationState = client.ProcessUserAuthorization(this.Request);


            var tokenInfoUrl = Config.SERVER_ADDRESS + "/OAuth2/Token";

            var httpClient = new HttpClient();

            string decodedNetworkCredentials = string.Format("{0}:{1}", ConfigurationManager.AppSettings["alhambraIdentifier"], ConfigurationManager.AppSettings["alhambraSecret"]);
            string encodedNetworkCredentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(decodedNetworkCredentials));
             
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedNetworkCredentials);
             
            Dictionary<string, string> formVals = new Dictionary<string, string>();
            formVals.Add("grant_type", "authorization_code");
            formVals.Add("code", input.code);

            formVals.Add("redirect_uri", Config.CLIENT_ADDRESS + "/OpenIdConnect/AlhambraCallback");


            HttpRequestMessage postRequest = new HttpRequestMessage(HttpMethod.Post, tokenInfoUrl);
            postRequest.Content = new FormUrlEncodedContent(formVals);

            HttpResponseMessage postResponse = httpClient.SendAsync(postRequest).Result;



            //in the form of an actionresult instead of a function
            //because the httpclient provides the authorization header 
            //by the time it performs the request

            //string tokenInfo = httpClient.GetAsync(tokenInfoUrl).Result.Content.ReadAsStringAsync().Result;

            // System.Web.HttpContext.Current.Application["Token"]= tokenInfo;


            //var tv = new AlhambraTokenValidator();
            //tv.ValidateToken(tokenInfo, "NATURE");

            // string userInfoUrl = CLIENT_ADDRESS + "/UserInfo";

            // OAuth2Graph userInfo = httpClient.GetAsync(userInfoUrl).Result.Content.ReadAsAsync<OAuth2Graph>().Result;


            // string userInfo = httpClient.GetAsync(userInfoUrl).Result.Content.ReadAsStringAsync().Result;

            // OAuth2Graph userinfo = client.GetUserInfo(auth.AccessToken);
            //string result = JsonConvert.SerializeObject(userinfo);


            OpenIdConnectToken result = postResponse.Content.ReadAsAsync<OpenIdConnectToken>().Result;

            JWTSecurityToken token = new JWTSecurityToken(result.id_token);


            string jwtDecoded = AlhambraJwtTokenManager.DecodeJWT(token);


            return Content("access_token: " + result.access_token +
                "<br/>refresh_token: " + result.refresh_token +
                "<br/>expires_in: " + result.expires_in +
                "<br/>id_token: " + result.id_token +
                "<br/>issuer: " + token.Issuer +
                "<br/>Audience: " + token.Audience +
                "<br/>Valid From: " + token.ValidFrom.ToString("yyyy-MM-ddThh:mm:ssZ") +
                "<br/>Valid To: " + token.ValidTo.ToString("yyyy-MM-ddThh:mm:ssZ"));

        }

        //[AllowAnonymous]
        //public ActionResult TokenInfo()
        //{

        //   // IAuthorizationState auth = client.ProcessUserAuthorization(this.Request);

        //    var auth = System.Web.HttpContext.Current.Application["Authorization"] as AuthorizationState;



        //    var signCert = LoadCert(Config.ALHAMBRA_AUTHORIZATION);
        //    var encryptCert = LoadCert(Config.ALHAMBRA_RESOURCES);

        //    var analyzer = new StandardAccessTokenAnalyzer(
        //                            (RSACryptoServiceProvider)signCert.PublicKey.Key,
        //                            (RSACryptoServiceProvider)encryptCert.PrivateKey);

        //    var resourceServer = new ResourceServer(analyzer);


        //    //List<string> scopes = new List<string>();

        //    //foreach (string scope in auth.Scope)
        //    //{
        //    //    scopes.Add(scope);
        //    //}

        //    AccessToken token = resourceServer.GetAccessToken(Request, auth.Scope.ToArray());
        //   // string tempResult = Json(token).ToString();
        //    return Json(token , JsonRequestBehavior.AllowGet);
        //}

        [Authorize]
        public ActionResult UserInfo()
        {
            var auth = System.Web.HttpContext.Current.Application["Authorization"] as AuthorizationState;
            OAuth2Graph graph = client.GetUserInfo(auth.AccessToken);
            return View(graph);
        }

    }
}
