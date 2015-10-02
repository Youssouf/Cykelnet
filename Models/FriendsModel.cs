using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cykelnet.Controllers;
using Cykelnet.Models.Events;
using System.Web.Mvc;
using System.Web.Security;

namespace Cykelnet.Models
{
    public static class FriendsModel
    {
        private static CykelnetDBDataContext _db = new CykelnetDBDataContext();
        
        public static void save(Friend f)
        {
            try
            {
                _db.Friends.InsertOnSubmit(f);
            }
            catch { }

            _db.SubmitChanges();
        }

        public static void createFriendship(Guid user1, Guid user2)
        {
            // Create Friend relationship
            Friend f = new Friend();
            f.FriendsSince = DateTime.Now;
            f.User1 = user1;
            f.User2 = user2;
            FriendsModel.save(f);

            // Create event that this friendship is established
            FriendshipEstablishedEvent fee = new FriendshipEstablishedEvent(f);
            fee.save();
        }

        public static List<Friend> getFriends(Guid id)
        {
            List<Friend> req = (from r in _db.Friends
                                where (r.User1 == id || r.User2 == id) && r.FriendsTil == null
                                select r).ToList();

            return req;
        }

        public static List<VersionedUser> getFriendsAsVersionedUser(Guid id)
        {
            List<Guid> guids = FriendsModel.getFriendsGuid(id);
            List<VersionedUser> result = new List<VersionedUser>();
            foreach (Guid guid in guids)
            {
                result.Add(AuthenticationHelper.getVersionedUser(guid, false));
            }

            return result;
        }

        /// <summary>
        /// Converts a list of friends (likely from getFriends(Guid), such that
        /// user1 is the current user, and
        /// user2 is the friend
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize]
        public static List<Friend> convertForDisplay(List<Friend> input)
        {
            List<Friend> result = new List<Friend>();
            Guid currentUser = (Guid)Membership.GetUser().ProviderUserKey;
            foreach (Friend f in input)
            {
                if (f.User1 == currentUser)
                {
                    result.Add(f);
                }
                else
                {
                    f.User2 = f.User1;
                    f.User1 = currentUser;
                    result.Add(f);
                }
            }
            return result;
        }

        public static List<Guid> getFriendsGuid(Guid id)
        {
            List<Friend> friendList = getFriends(id);

            List<Guid> result = new List<Guid>();
            foreach (Friend f in friendList)
            {
                if (f.User1 == id)
                    result.Add(f.User2);
                else
                    result.Add(f.User1);
            }
            return result;
        }

        public static bool isFriends(Guid User1, Guid User2)
        {
            List<Friend> rel = (from r in _db.Friends
                                where ((r.User1 == User1 && r.User2 == User2) || (r.User1 == User2 && r.User2 == User1)) && r.FriendsTil == null
                                select r).ToList();

            if (rel.Count > 0)
                return true;
            else
                return false;
        }

        public static void deleteFriend(Guid User1, Guid User2)
        {
            List<Friend> rel = (from r in _db.Friends
                                where ((r.User1 == User1 && r.User2 == User2) || (r.User1 == User2 && r.User2 == User1)) && r.FriendsTil == null
                                select r).ToList();

            foreach (Friend f in rel)
            {
                f.FriendsTil = DateTime.Now;

                // Create event that friendship is deleted
                FriendshipRemovedEvent fre = new FriendshipRemovedEvent(f);
                fre.save();
            }

            _db.SubmitChanges();
        }

        public static List<Cykelnet.Controllers.FriendController.JsonFriend> getJsonFriendsList(Guid id)
        {
            List<Friend> friendList = FriendsModel.getFriends(id);

            List<FriendController.JsonFriend> jsonFL = new List<FriendController.JsonFriend>();
            foreach (Friend friend in friendList)
            {
                FriendController.JsonFriend jf = new FriendController.JsonFriend();
                if (id.Equals(friend.User1))
                {
                    jf.User = friend.User2;
                }
                else
                {
                    jf.User = friend.User1;
                }
                jf.FriendsSince = friend.FriendsSince.ToString("yyyy-MM-dd HH:mm:ss");
                jsonFL.Add(jf);
            }

            return jsonFL;
        }
    }
}