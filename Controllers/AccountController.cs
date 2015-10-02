using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Cykelnet.Models;
using System.Globalization;
using System.IO;
using System.Web.Script.Serialization;

namespace Cykelnet.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/LogOn

        public ActionResult LogOn()
        {
            return View();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            return View(new RegisterModel());
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            HttpPostedFileBase file = null;
            if (Request.Files[0].ContentLength > 0)
            {
                //If a picture is added, make sure it is not to large.
                file = Request.Files[0];
                if (file.ContentLength != 0 && file.ContentLength > 2097152)
                    ModelState.AddModelError("Avatar", "The avatar you selected is too large!");
                else if (file.ContentType != "image/png")
                    ModelState.AddModelError("Avatar", "The avatar must be a PNG");
            }
            //If a birthday is provided, check if it is valid.
            DateTime? date = DateTime.MinValue;
            if (!model.BirthdayDay.Equals("0") && !model.BirthdayMonth.Equals("0") && !model.BirthdayYear.Equals("0"))
            {
                try
                {
                    date = DateTime.Parse(model.BirthdayDay + "/" + model.BirthdayMonth + "/" + model.BirthdayYear, new CultureInfo("da-DK", false));
                }
                catch 
                {
                    ModelState.AddModelError("BirthdayDay", "Invalid birthday date.");
                    ModelState.AddModelError("BirthdayMonth", "Invalid birthday date.");
                    ModelState.AddModelError("BirthdayYear", "Invalid birthday date.");
                }
            }

            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;

                var user = ((VersionedMembershipProvider)Membership.Provider).CreateUser(model.UserName,
                    model.Password,
                    model.Email,
                    model.FullName,
                    model.Address1,
                    model.Address2,
                    model.Country,
                    model.CyclistType,
                    date != DateTime.MinValue ? date : null,
                    out createStatus);
                
                if (createStatus == MembershipCreateStatus.Success)
                {

                    FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
                    
                    if (file != null && file.ContentLength > 0)
                    {
                        
                        file.SaveAs(Request.MapPath("~/Content/Images/Avatars/" + ((Guid)user.ProviderUserKey).ToString() + ".png"));
                    }
                    
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        //
        // GET: /Account/PublicProfile
        [Authorize]
        public ActionResult PublicProfile(Guid? id)
        {
            if (!id.HasValue)
                throw new HttpException(404, "No profile found");

            var model = new PublicProfileModel();
            
            VersionedUser user = (VersionedUser) Membership.GetUser(id);
            
            model.id = (Guid) id;
            model.UserName = user.UserName;
            model.FullName = user.FullName;
            model.CyclistType = user.CyclistType;

            if (user.Birthday != DateTime.MinValue)
            {
                model.Age = Convert.ToString((int)DateTime.Now.Subtract(user.Birthday).TotalDays / 365); //TODO Make this calculation correct
                model.hasAge = true;
            }
            else
            {
                model.hasAge = false;
            }
                
                model.Events = EventModel.getEvents((Guid)user.ProviderUserKey);

            ViewBag.friends = FriendsModel.getJsonFriendsList((Guid)user.ProviderUserKey);

            List<FriendController.JsonFriend> debug = FriendsModel.getJsonFriendsList((Guid)user.ProviderUserKey);

            return View(model);
        }

        [Authorize]
        public ActionResult Test()
        {
            var vUser = (VersionedUser)Membership.GetUser();

            vUser.Address1 = "Address1";
            vUser.Address2 = null;
            vUser.Birthday = DateTime.MaxValue;
            vUser.Country = "Jylland";
            vUser.CyclistType = "Awesome";
            vUser.FullName = "FullName";

            Membership.UpdateUser(vUser);
            
            return new EmptyResult();
        }


        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
