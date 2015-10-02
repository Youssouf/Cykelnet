using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using System.Web.Routing;
using Cykelnet.Models;
using System.Globalization;
using System.IO;


namespace Cykelnet.Models.Events
{
    public abstract class IEvent
    {
        //public int mEventID;
        //public string mEventText;
        //public Guid mEventUser;
        public DateTime mEventTime;
        //public int mEventTypeID;

        /*
        public const int EVENTTYPE_ROUTE_CREATED = 1;
        public const int EVENTTYPE_ROUTE_FAVORED = 2;
        public const int EVENTTYPE_FRIENDSHIP_CREATED = 3;
        public const int EVENTTYPE_ROUTE_SHARED = 4;
        */

        /*
        public BaseEvent(string eventText, Guid eventUser, int eventType)
        {
            //this.mEventID = e.EventID;
            this.mEventText = eventText;
            this.mEventUser = eventUser;
            this.mEventTime = DateTime.Now;
            this.mEventTypeID = eventType;
        }

        public BaseEvent()
        {
        }
        */
        /*
        [Authorize]
        public String getUserName()
        {
            MembershipUser currentUser = Membership.GetUser(this.EventUser);
            return currentUser.UserName;
        }
        */
         
        public abstract void save();
    }
}