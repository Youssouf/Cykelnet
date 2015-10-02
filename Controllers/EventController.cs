using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cykelnet.Models;
using System.Web.Security;
using Cykelnet.Models.Events;

namespace Cykelnet.Controllers
{
    public class EventController : Controller
    {
        //
        // GET: /Event/

        /*
        [Authorize]
        public ActionResult Index()
        {
            List<BaseEvent> evts = EventModel.getFriendsEvents(AuthenticationHelper.getCurrentUserID(), DateTime.Now.AddDays(-7));
            string stuffz = "";
            foreach(BaseEvent e in evts)
            {
                stuffz += e.mEventText;
            }

            return Content(stuffz);
        }*/

        [Authorize]
        public ActionResult EventList()
        {
            // Get events for current user
            List<IEvent> result = EventModel.getEvents((Guid)Membership.GetUser().ProviderUserKey);
            // Only show 25 most recent events
            result = result.Take(25).ToList();
            return PartialView("Events/_PersonalEventList", result);
        }

        /*
        [Authorize]
        public ActionResult FriendEventList()
        {
            return PartialView("Events/_PersonalEventList", EventModel.getFriendsEvents(AuthenticationHelper.getCurrentUserID(), DateTime.Now.AddDays(-7)));
        }

        [Authorize]
        public ActionResult Test()
        {
            DefaultEvent e = new DefaultEvent();

            e.EventText = "Teadsasdst Event";
            e.EventUser = new Guid("E409B4D7-A6E2-4C8A-B342-272FE865B5BE");
            EventModel.save(e);

            //EventModel.eventViewed((Event)e,AuthenticationHelper.getCurrentUserID());

            return new EmptyResult();
        }*/

    }
}
