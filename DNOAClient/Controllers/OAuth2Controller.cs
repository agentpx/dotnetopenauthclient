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

namespace DNOAClient.Controllers
{
    public class OAuth2Controller : Controller
    {
        private AlhambraWebServerClient client = new AlhambraWebServerClient();

        public ActionResult OAuth()
        {
            if (string.IsNullOrEmpty(Request.QueryString["code"]))
            {
                return InitAuth();
            }
            else
            {
                return Callback();
            }
        }

        public ActionResult InitAuth()
        {
            var state = new AuthorizationState();
            var uri = Request.Url.AbsoluteUri;
            uri = RemoveQueryStringFromUri(uri);
            state.Callback = new Uri(uri);

            state.Scope.Add(AlhambraWebServerClient.Scopes.ConnectId.OpenId);

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

        public ActionResult Callback()
        {
            var auth = client.ProcessUserAuthorization(this.Request);
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth.AccessToken);

            var tokenInfoUrl = "http://localhost/oauth2/TokenInfo";

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

            var token = resourceServer.GetAccessToken(Request, new[] { AlhambraWebServerClient.Scopes.ConnectId.OpenId });

            return Json(new {audience =token.ClientIdentifier, User=token.User }, JsonRequestBehavior.AllowGet);
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
