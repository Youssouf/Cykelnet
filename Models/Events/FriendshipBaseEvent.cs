using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cykelnet.Models.Events
{
    public abstract class FriendshipBaseEvent : IEvent
    {
        // Table attributes
        public int mEventId;
        public int mFriendRelationId;
        public Guid mUser1;
        public Guid mUser2;
        //public DateTime mEventTime;

        protected FriendshipBaseEvent()
        {
        }

        public FriendshipBaseEvent(Friend f)
        {
            this.mFriendRelationId = f.FriendRelationID;
            this.mUser1 = f.User1;
            this.mUser2 = f.User2;
            this.mEventTime = f.FriendsSince;
        }

        //public abstract void save();
    }
}