using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cykelnet.Models.Algorithms;
using System.Web.Security;
namespace Cykelnet.Controllers
{
    public class RecommenderController : Controller
    {
        //
        // GET: /Recommender/

        public ActionResult Index()
        {
            MembershipUserCollection users = Membership.GetAllUsers();
            List<Guid> idList = new List<Guid>();
            foreach (MembershipUser user in users)
            {
                idList.Add((Guid)user.ProviderUserKey);
            }

            Recommender.recommendAllUsers(idList);

            return RedirectToAction("Index", "Home");
        }

    }
}
