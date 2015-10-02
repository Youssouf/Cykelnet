using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Cykelnet.Models.Events
{
    public class FriendshipEstablishedEvent : FriendshipBaseEvent
    {
        private CykelnetDBDataContext _db = new CykelnetDBDataContext();

        public FriendshipEstablishedEvent(Friend f) : base(f)
        {
        }

        public FriendshipEstablishedEvent(EventsFriendshipEstablished f)
        {
            this.mEventId = f.EventID;
            this.mFriendRelationId = f.FriendRelationID;
            this.mUser1 = f.User1;
            this.mUser2 = f.User2;
            this.mEventTime = f.EventTime;
        }

        public override void save()
        {
            EventsFriendshipEstablished e = new EventsFriendshipEstablished();
            e.FriendRelationID = this.mFriendRelationId;
            e.User1 = this.mUser1;
            e.User2 = this.mUser2;
            e.EventTime = this.mEventTime;
            
            _db.EventsFriendshipEstablisheds.InsertOnSubmit(e);
            _db.SubmitChanges();

            this.mEventId = e.EventID;
        }
    }
}