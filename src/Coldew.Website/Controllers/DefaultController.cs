using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Coldew.Api;
using Coldew.Website.Api.Models;

namespace Coldew.Website.Controllers
{
    public class DefaultController : Controller
    {
        //
        // GET: /Default/

        public ActionResult Index()
        {
#if DEBUG
            string token = WebHelper.AuthenticationService.SignIn("admin", "123456", "1270.0.0.1");
            WebHelper.SetCurrentUserToken(token, false);
            List<ColdewObjectWebModel> objects = WebHelper.WebsiteColdewObjectService.GetObjects(WebHelper.CurrentUserAccount);
            return this.RedirectToAction("Index", "Metadata", new { objectId = objects[0].id });
#else
            return this.RedirectToAction("Index", "Login");
#endif


        }

    }
}
