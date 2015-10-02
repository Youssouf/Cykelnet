using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cykelnet.Models.Events;

namespace Cykelnet.Models
{
    public static class EventModel
    {
        private static CykelnetDBDataContext _db = new CykelnetDBDataContext();

        public static List<IEvent> getEvents(Guid UserID)
        {
            List<IEvent> result = new List<IEvent>();

            List<Guid> friendList = FriendsModel.getFriendsGuid(UserID);

            // Get route created events from friends
            var rce = from r in _db.EventsRouteCreateds
                      where friendList.Contains(r.EventCreator)
                      select r;            

            foreach (EventsRouteCreated item in rce)
            {
                result.Add(new RouteCreatedEvent(item));
            }

            // Get route favored events from friends
            var rfe = from r in _db.EventsRouteFavoreds
                      where friendList.Contains(r.EventCreator)
                      select r;

            foreach (EventsRouteFavored item in rfe)
            {
                result.Add(new RouteFavoredEvent(item));
            }

            // Get friendship established from friends
            var fee = from f in _db.EventsFriendshipEstablisheds
                      where (friendList.Contains(f.User1) || friendList.Contains(f.User2))
                      select f;

            foreach (EventsFriendshipEstablished item in fee)
            {
                result.Add(new FriendshipEstablishedEvent(item));
            }

            // Get friendship removed from friends
            var fre = from f in _db.EventsFriendshipRemoveds
                      where (friendList.Contains(f.User1) || friendList.Contains(f.User2))
                      select f;

            foreach (EventsFriendshipRemoved item in fre)
            {
                result.Add(new FriendshipRemovedEvent(item));
            }

            // Sort events, such that newest are showed first
            result.Sort(delegate(IEvent e1, IEvent e2) { return e2.mEventTime.CompareTo(e1.mEventTime); });



            return result;
        }

        /// <summary>
        /// Deletes all events related to a given route
        /// </summary>
        /// <param name="RouteID"></param>
        public static void deleteAllEvents(int RouteID)
        {
            // Route created events
            var rce = from r in _db.EventsRouteCreateds
                      where r.EventRouteID == RouteID
                      select r;

            _db.EventsRouteCreateds.DeleteAllOnSubmit(rce);

            // Route favored events
            var rfe = from r in _db.EventsRouteFavoreds
                      where r.EventRouteID == RouteID
                      select r;

            _db.EventsRouteFavoreds.DeleteAllOnSubmit(rfe);
        }

        /*
        public static List<BaseEvent> getEvents(Guid UserID, DateTime SinceDate)
        {
            List<Event> evts = (from e in _db.Events
                                where e.EventUser == UserID && e.EventTime > SinceDate
                                select e).ToList();

            return eventToBaseEvent(evts);
        }

        public static List<BaseEvent> getFriendsEvents(Guid UserID, DateTime SinceDate)
        {
            List<Event> evts = (from e in _db.Events
                                where (from f in _db.Friends
                                       where f.User2 == UserID
                                       select f.User1
                                       ).Union(
                                       from f in _db.Friends
                                       where f.User1 == UserID
                                       select f.User2).Contains(e.EventUser)
                                       && e.EventTime > SinceDate
                                select e).ToList();

            return eventToBaseEvent(evts);
        }

        public static List<BaseEvent> getFriendsEvents(Guid UserID, DateTime SinceDate, bool NotViewed)
        {
            if (!NotViewed)
            {
                return getFriendsEvents(UserID, SinceDate);
            }


            List<Event> evts = (from e in _db.Events
                                where (from f in _db.Friends
                                       where f.User2 == UserID
                                       select f.User1
                                       ).Union(
                                       from f in _db.Friends
                                       where f.User1 == UserID
                                       select f.User2).Contains(e.EventUser)
                                       && e.EventTime > SinceDate
                                       && !(from ev in _db.EventViews
                                            where ev.UserID == UserID
                                            select ev.EventID).Contains(e.EventID)
                                select e).ToList();

            return eventToBaseEvent(evts);
        }


        public static void save(BaseEvent be)
        {
            be.save();
            
        }

        private static Event getDBEvent(int eventID)
        {
            Event evt = (from e in _db.Events
                         where e.EventID == eventID
                         select e).Single();
            return evt;
        }
        
        private static BaseEvent getEvent(int eventID)
        {
            return eventToBaseEvent(getDBEvent(eventID));
        }

        public static void delete(int eventID)
        {
            List<Event> evts = (from e in _db.Events
                                where e.EventID == eventID
                                select e).ToList();

            foreach (Event e in evts)
            {
                e.DeleteTime = DateTime.Now;
            }

            _db.SubmitChanges();
        }

        private static void saveEventView(EventView ev)
        {
            _db.EventViews.InsertOnSubmit(ev);

            _db.SubmitChanges();
        }

        public static void eventViewed(int eventID, Guid userID)
        {
            var evt = (from e in _db.Events where e.EventID == eventID select e).First();

            eventViewed(evt, userID);
        }

        public static void eventViewed(Event e, Guid userID)
        {
            EventView ev = new EventView();

            ev.Event = e;
            ev.UserID = userID;
            ev.ViewTime = DateTime.Now;

            saveEventView(ev);
        }

        
        public static List<EventType> getEventTypes()
        {
            var evtTypes = (from et in _db.EventTypes select et).ToList();

            return evtTypes;
        }

        private static BaseEvent eventToBaseEvent(Event e)
        {
            //var eventType = e.GetType();

            if (e.EventTypeID == BaseEvent.EVENTTYPE_DEFAULT)
                return new DefaultEvent(e);
            else if (e.EventTypeID == BaseEvent.EVENTTYPE_FRIEND)
                return new FriendEvent(e);

            throw new ArgumentOutOfRangeException();
        }

        private static List<BaseEvent> eventToBaseEvent(List<Event> el)
        {
            List<BaseEvent> events = new List<BaseEvent>();
            
            foreach (Event e in el)
            {
                events.Add(eventToBaseEvent(e));
            }
            
            return events;
        }
         */
    }
}