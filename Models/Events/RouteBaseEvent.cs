using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cykelnet.Models.Events
{
    public abstract class RouteBaseEvent : IEvent
    {
        // Table attributes
        public int mEventId;
        public int mRouteId;
        public Guid mEventCreator;
        //public DateTime mEventTime;
        /*
        public RouteModel mRouteModel;
        {
            get
            {
                if (this.mRouteModel == null)
                    this.mRouteModel = new RouteModel(this.mRouteId);
                return this.mRouteModel;
            }
            protected set
            {
                this.mRouteModel = value;
            }
        }*/


        //public abstract void save();
    }
}