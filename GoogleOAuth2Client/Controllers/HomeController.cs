using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotNetOpenAuth;
using DotNetOpenAuth.OAuth2;
using System.Configuration;
using DotNetOpenAuth.Messaging;
using System.Threading.Tasks;
using System.Net;




//http://www.tkglaser.net/2012/03/single-sign-on-using-facebook-in-asp.html
namespace GoogleOAuth2Client.Controllers
{
    public class HomeController : Controller
    {

        private static readonly GoogleClient googleClient = new GoogleClient
        {
            ClientIdentifier = ConfigurationManager.AppSettings["googleClientID"],
            ClientCredentialApplicator = ClientCredentialApplicator.PostParameter(ConfigurationManager.AppSettings["googleClientSecret"]),
        };

       
        private IAuthorizationState authorizationState = null;
     

        public ActionResult Index()
        {
            ViewBag.Message = "Choose a demo:";

            return View();
        }

        public ActionResult GoogleAuthentication()
        {
            ViewBag.IsAuthorized = false;
            authorizationState = googleClient.ProcessUserAuthorization();

            Dictionary<string, string> result = new Dictionary<string, string>();

            if (authorizationState == null)
            {
                googleClient.RequestUserAuthorization(new string[] { GoogleClient.Scopes.UserInfo.Email, GoogleClient.Scopes.UserInfo.Profile }, new Uri("http://localhost:59941/Home/GoogleAuthentication"));
                

            }
            else
            {
                IOAuth2Graph oauth2Graph = googleClient.GetGraph(authorizationState);
                //result.Add("AvatarUrl", oauth2Graph.AvatarUrl.ToString());
                result.Add("Birthday", oauth2Graph.BirthdayDT.ToString());
                result.Add("Email", oauth2Graph.Email);
                result.Add("FirstName", oauth2Graph.FirstName);
                result.Add("Gender", oauth2Graph.Gender.ToString());
                result.Add("Id", oauth2Graph.Id);
                result.Add("LastName", oauth2Graph.LastName);
                result.Add("Link", oauth2Graph.Link.ToString());
                result.Add("Locale", oauth2Graph.Locale);
                result.Add("Name", oauth2Graph.Name);
                //result.Add("UpdatedTime", oauth2Graph.UpdatedTime);

                ViewBag.Result = result;

                ViewBag.IsAuthorized = true;
                return View();
            }

          
            return View();
        }

   

    }
}
