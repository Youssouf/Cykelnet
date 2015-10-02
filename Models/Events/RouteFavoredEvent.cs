using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Cykelnet.Models.Events
{
    public class RouteFavoredEvent : RouteBaseEvent
    {
        private CykelnetDBDataContext _db = new CykelnetDBDataContext();

        public RouteFavoredEvent(Favorite f)
        {
            this.mRouteId = f.Route_ID;
            this.mEventCreator = f.User_ID;
            this.mEventTime = DateTime.Now;
            
        }

        public RouteFavoredEvent(EventsRouteFavored e)
        {
            this.mEventId = e.EventID;
            this.mRouteId = e.EventRouteID;
            this.mEventCreator = e.EventCreator;
            this.mEventTime = e.EventTime;
            
        }

        public override void save()
        {
            EventsRouteFavored e = new EventsRouteFavored();
            e.EventCreator = this.mEventCreator;
            e.EventRouteID = this.mRouteId;
            e.EventTime = this.mEventTime;

            _db.EventsRouteFavoreds.InsertOnSubmit(e);
            _db.SubmitChanges();

            this.mEventId = e.EventID;
        }
    }
}