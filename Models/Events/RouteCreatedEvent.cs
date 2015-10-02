using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Cykelnet.Models.Events
{
    public class RouteCreatedEvent : RouteBaseEvent
    {
        private CykelnetDBDataContext _db = new CykelnetDBDataContext();

        public RouteCreatedEvent(Route r)
        {
            this.mRouteId = r.RouteID;
            this.mEventCreator = r.User_ID;
            this.mEventTime = (DateTime)r.ValidFrom;
            
        }

        public RouteCreatedEvent(EventsRouteCreated e)
        {
            this.mEventId = e.EventID;
            this.mRouteId = e.EventRouteID;
            this.mEventCreator = e.EventCreator;
            this.mEventTime = e.EventTime;
            
        }

        public override void save()
        {
            EventsRouteCreated e = new EventsRouteCreated();
            e.EventCreator = this.mEventCreator;
            e.EventRouteID = this.mRouteId;
            e.EventTime = this.mEventTime;

            _db.EventsRouteCreateds.InsertOnSubmit(e);
            _db.SubmitChanges();

            this.mEventId = e.EventID;
        }
    }
}