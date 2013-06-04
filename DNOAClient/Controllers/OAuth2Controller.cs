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
      

        private IAuthorizationState authorizationState = null;

        private static readonly AlhambraWebServerClient client = new AlhambraWebServerClient
        {
            ClientIdentifier = ConfigurationManager.AppSettings["alhambraIdentifier"],
            ClientCredentialApplicator = ClientCredentialApplicator.PostParameter(ConfigurationManager.AppSettings["alhambraSecret"]),
        };


        //[AllowAnonymous]
        //public ActionResult Authorize()
        //{

        //    authorizationState = client.ProcessUserAuthorization(this.Request);

        //    if (authorizationState == null)
        //    {
        //        client.RequestUserAuthorization(new string[] { AlhambraWebServerClient.Scopes.ConnectId.OpenId, AlhambraWebServerClient.Scopes.ConnectId.OfflineAccess });
        //    }
        //    else
        //    {
        //        var httpClient = new HttpClient();
        //        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authorizationState.AccessToken);

        //        var tokenInfoUrl = "https://localhost:44301/OAuth2/TokenInfo";

        //        OAuth2TokenInfo tokenInfo = httpClient.GetAsync(tokenInfoUrl).Result.Content.ReadAsAsync<OAuth2TokenInfo>().Result;

        //        var tv = new AlhambraTokenValidator();
        //        tv.ValidateToken(tokenInfo, "NATURE");

        //        string info = tokenInfo.User + ", " + tokenInfo.Audience;

        //        return Content(info);
        //    }
        //    return null;
        //}


        [AllowAnonymous]
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

        [AllowAnonymous]
        public ActionResult InitAuthAlhambra()
        {

            var state = new AuthorizationState();
            //var uri = Request.Url.AbsoluteUri;
            //uri = RemoveQueryStringFromUri(uri);
            //state.Callback = new Uri(uri);
            state.Callback = new Uri("https://localhost:44301/OAuth2/AlhambraCallback");
            state.Scope.Add(AlhambraWebServerClient.Scopes.ConnectId.OpenId);
            state.Scope.Add(AlhambraWebServerClient.Scopes.ConnectId.OfflineAccess);
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

        [AllowAnonymous]
        public ActionResult AlhambraCallback()
        {
            var auth = client.ProcessUserAuthorization(this.Request);
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth.AccessToken);

            var tokenInfoUrl = "https://localhost:44301/OAuth2/TokenInfo";

            OAuth2TokenInfo tokenInfo = httpClient.GetAsync(tokenInfoUrl).Result.Content.ReadAsAsync<OAuth2TokenInfo>().Result;

            var tv = new AlhambraTokenValidator();
            tv.ValidateToken(tokenInfo, "NATURE");

            string info = tokenInfo.User + ", " + tokenInfo.Audience;

            return Content(info);
        }

        public ActionResult TokenInfo()
        {
            var signCert = LoadCert(Config.STS_CERT);
            var encryptCert = LoadCert(Config.SERVICE_CERT);

            var analyzer = new StandardAccessTokenAnalyzer(
                                    (RSACryptoServiceProvider)signCert.PublicKey.Key,
                                    (RSACryptoServiceProvider)encryptCert.PrivateKey);

            var resourceServer = new ResourceServer(analyzer);

            var token = resourceServer.GetAccessToken(Request, new[] { AlhambraWebServerClient.Scopes.ConnectId.OpenId, AlhambraWebServerClient.Scopes.ConnectId.OfflineAccess });

            return Json(new {audience=token.ClientIdentifier, User=token.User }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UserInfo()
        {
            return View();
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
