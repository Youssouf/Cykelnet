using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cykelnet.Models;
using System.Web.Security;
using System.Web.Script.Serialization;

namespace Cykelnet.Controllers
{
    public class FriendController : Controller
    {
        public class JsonFriendRequest
        {
            public Guid User;
            public string RequestTime;
        }
        public class JsonFriend
        {
            public Guid User;
            public string FriendsSince;
        }

        [Authorize]
        public ActionResult Request(Guid? id) 
        {
            if(!AccountModel.checkUserID(id)) throw new HttpException(500, "Invalid parameters");

            FriendRequestModel.createFriendRequest((Guid)Membership.GetUser().ProviderUserKey, (Guid)id);

            return new EmptyResult();
        }

        [Authorize]
        public ActionResult Accept(Guid? id)
        {
            if (!AccountModel.checkUserID(id)) throw new HttpException(500, "Invalid parameters");

            FriendRequestModel.acceptRequest((Guid)id, (Guid)Membership.GetUser().ProviderUserKey);
            return PartialView("Friends/_Friend", AuthenticationHelper.getVersionedUser((Guid)id, false));
        }

        [Authorize]
        public ActionResult Reject(Guid? id)
        {
            if (!AccountModel.checkUserID(id)) throw new HttpException(500, "Invalid parameters");

            FriendRequestModel.rejectRequest((Guid)id, (Guid)Membership.GetUser().ProviderUserKey);
            
            return new EmptyResult();
        }

        [Authorize]
        public ActionResult FriendRequests()
        {
            Guid id = (Guid)Membership.GetUser().ProviderUserKey;
            List<FriendRequest> reqList = FriendRequestModel.getFriendRequests(id);

            List<JsonFriendRequest> jsonFRL = new List<JsonFriendRequest>();
            foreach (FriendRequest friendReq in reqList)
            {
                JsonFriendRequest jfr = new JsonFriendRequest();
                jfr.User = friendReq.FromUser;
                jfr.RequestTime = friendReq.RequestTime.ToString("yyyy-MM-dd HH:mm:ss");
                jsonFRL.Add(jfr);
            }

            string js = new JavaScriptSerializer().Serialize(jsonFRL);
            return Content(js);
        }

        [Authorize]
        public ActionResult List()
        {
            Guid id = (Guid)Membership.GetUser().ProviderUserKey;
            List<Friend> friendList = FriendsModel.getFriends(id);

            List<JsonFriend> jsonFL = new List<JsonFriend>();
            foreach (Friend friend in friendList)
            {
                JsonFriend jf = new JsonFriend();
                if(id.Equals(friend.User1))
                {
                    jf.User = friend.User2;
                } else
                {
                    jf.User = friend.User1;
                }
                jf.FriendsSince = friend.FriendsSince.ToString("yyyy-MM-dd HH:mm:ss");
                jsonFL.Add(jf);
            }

            string js = new JavaScriptSerializer().Serialize(jsonFL);
            return Content(js);
        }

        public ActionResult RemoveFriend(Guid? id)
        {
            if (!AccountModel.checkUserID(id)) throw new HttpException(500, "Invalid parameters");

            FriendsModel.deleteFriend((Guid)Membership.GetUser().ProviderUserKey, (Guid)id);

            return new EmptyResult();
        }

        public ActionResult RemoveRequest(Guid? id)
        {
            if (!AccountModel.checkUserID(id)) throw new HttpException(500, "Invalid parameters");

            FriendRequestModel.deleteRequest((Guid)Membership.GetUser().ProviderUserKey, (Guid)id);

            return new EmptyResult();
        }

        public ActionResult PendingRequests()
        {
            List<FriendRequest> reqList = FriendRequestModel.getFriendRequests((Guid)Membership.GetUser().ProviderUserKey);

            return View(reqList);
        }
    }
}
