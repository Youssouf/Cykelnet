using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cykelnet.Models.Events
{
    public class FriendshipRemovedEvent : FriendshipBaseEvent
    {
        private CykelnetDBDataContext _db = new CykelnetDBDataContext();



        public FriendshipRemovedEvent(Friend f) : base(f)
        {
        }

        public FriendshipRemovedEvent(EventsFriendshipRemoved f)
        {
            this.mEventId = f.EventID;
            this.mFriendRelationId = f.FriendRelationID;
            this.mUser1 = f.User1;
            this.mUser2 = f.User2;
            this.mEventTime = f.EventTime;
        }

        public override void save()
        {
            EventsFriendshipRemoved e = new EventsFriendshipRemoved();
            e.FriendRelationID = this.mFriendRelationId;
            e.User1 = this.mUser1;
            e.User2 = this.mUser2;
            e.EventTime = this.mEventTime;

            _db.EventsFriendshipRemoveds.InsertOnSubmit(e);
            _db.SubmitChanges();

            this.mEventId = e.EventID;
        }
    }
}