using DNOAClient.Code;
using DotNetOpenAuth.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotNetOpenAuth.Messaging;
using System.Net.Http;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
using System.Configuration;

namespace DNOAClient.Controllers
{
    public class OAuth2Controller : Controller
    {

     

        private const string CLIENT_ADDRESS = "https://localhost:44301";
        private const string SERVER_ADDRESS = "https://localhost:44300";

       // private IAuthorizationState authorizationState = null;

        private static readonly AlhambraWebServerClient client = new AlhambraWebServerClient
        {
            ClientIdentifier = ConfigurationManager.AppSettings["alhambraIdentifier"],
            ClientCredentialApplicator = ClientCredentialApplicator.PostParameter(ConfigurationManager.AppSettings["alhambraSecret"]),
        };


        //[Authorize]
        public ActionResult Alhambra()
        {
            if (string.IsNullOrEmpty(Request.QueryString["code"]))
            {
                return InitAuthAlhambra();
            }
            else
            {
                return AlhambraCallback();
            }
        }

      //  [Authorize]
        public ActionResult InitAuthAlhambra()
        {

            var state = new AuthorizationState();
      
            state.Callback = new Uri(CLIENT_ADDRESS + "/OAuth2/AlhambraCallback");
            state.Scope.Add(OpenIdConnectScopes.OpenId);
            state.Scope.Add(OpenIdConnectScopes.OfflineAccess);
            state.Scope.Add(OpenIdConnectScopes.Email);
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
        public ActionResult AlhambraCallback()
        {
          //System.Web.HttpContext.Current.Application["Authorization"]  = (AuthorizationState) client.ProcessUserAuthorization(this.Request);
          AuthorizationState auth = (AuthorizationState)client.ProcessUserAuthorization(this.Request);
          System.Web.HttpContext.Current.Application["Authorization"]= auth;
           
          
          // CurrentAuthorizationState = auth;

           //string code = Request.QueryString["code"];
           //System.Web.HttpContext.Current.Application.Add("Code",Request.QueryString["code"]);
           
           //string accessToken = Request.QueryString["access_token"];
            //authorizationState = client.ProcessUserAuthorization(this.Request);

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth.AccessToken);

           // var tokenInfoUrl =  CLIENT_ADDRESS + "/OAuth2/TokenInfo";

            //in the form of an actionresult instead of a function
            //because the httpclient provides the authorization header 
            //by the time it performs the request

           // string tokenInfo = httpClient.GetAsync(tokenInfoUrl).Result.Content.ReadAsStringAsync().Result;
             
           // System.Web.HttpContext.Current.Application["Token"]= tokenInfo;
           

            //var tv = new AlhambraTokenValidator();
            //tv.ValidateToken(tokenInfo, "NATURE");

           // string userInfoUrl = CLIENT_ADDRESS + "/UserInfo";

           // OAuth2Graph userInfo = httpClient.GetAsync(userInfoUrl).Result.Content.ReadAsAsync<OAuth2Graph>().Result;


           // string userInfo = httpClient.GetAsync(userInfoUrl).Result.Content.ReadAsStringAsync().Result;

            OAuth2Graph userinfo = client.GetUserInfo(auth.AccessToken);
            //string result = JsonConvert.SerializeObject(userinfo);
            //return Content(result);

           string valuesOfForm =null;

            foreach(string s in Request.Form.AllKeys)
            {
                valuesOfForm = valuesOfForm + ", " + s + ":"  +  Request.Form[s];
            }

            return Content("Access Token: " + auth.AccessToken + 
                "<br/>Refresh Token: " + auth.RefreshToken +
                "<br/>Expires In: " + auth.AccessTokenExpirationUtc +
                "<br/>Issued At: " + auth.AccessTokenIssueDateUtc +
                "<br/>Values: " + valuesOfForm +
                "<br/> Content Type: " + Request.ContentType +
                "<br/>Input Stream: " + Request.InputStream.ToString());

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
           var auth= System.Web.HttpContext.Current.Application["Authorization"] as AuthorizationState;
            OAuth2Graph graph = client.GetUserInfo(auth.AccessToken);
            return View(graph);
        }

        #region Helpers

        private static X509Certificate2 LoadCert(string thumbprint)
        {
            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);

            var certs = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
       
            if (certs.Count == 0) throw new Exception("Could not find cert");
            var cert = certs[0];
            return cert;
        }

        #endregion

    }
}
