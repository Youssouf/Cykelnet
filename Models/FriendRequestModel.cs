using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security;

namespace Cykelnet.Models
{
    public static class FriendRequestModel
    {
        private static CykelnetDBDataContext _db = new CykelnetDBDataContext();

        public static void save(FriendRequest req)
        {
            try
            {
                _db.FriendRequests.InsertOnSubmit(req);
            }
            catch { }
            
            _db.SubmitChanges();
        }

        public static void acceptRequest(Guid FromUser, Guid ToUser)
        {
            FriendRequest req = FriendRequestModel.getFriendRequest(FromUser, ToUser);
            req.Accepted = true;
            req.AcceptedTime = DateTime.Now;

            FriendRequestModel.save(req);

            FriendsModel.createFriendship(FromUser, ToUser);
        }

        public static void rejectRequest(Guid FromUser, Guid ToUser)
        {
            FriendRequest req = FriendRequestModel.getFriendRequest(FromUser, ToUser);
            req.Accepted = false;
            req.AcceptedTime = DateTime.Now;

            FriendRequestModel.save(req);
        }
        public static void createFriendRequest(Guid FromUser, Guid ToUser)
        {
            FriendRequest req = new FriendRequest();
            req.FromUser = FromUser;
            req.ToUser = ToUser;
            req.RequestTime = DateTime.Now;

            FriendRequestModel.save(req);
        }

        public static FriendRequest getFriendRequest(Guid FromUser, Guid ToUser)
        {
            IList<FriendRequest> req = (from r in _db.FriendRequests
                                        where r.FromUser == FromUser && r.ToUser == ToUser && r.Accepted == null && r.AcceptedTime == null
                                        orderby r.RequestTime descending
                                        select r).ToList();

            if (req.Count == 0)
            {
                return null;
            }

            return req.First();
        }

        public static List<FriendRequest> getFriendRequests(Guid ToUser)
        {
            List<FriendRequest> req = (from r in _db.FriendRequests
                                       where r.ToUser == ToUser && r.Accepted == null && r.AcceptedTime == null
                                       orderby r.RequestTime descending
                                       select r).ToList();

            return req;
        }

        public static bool isRequested(Guid FromUser, Guid ToUser)
        {
            List<FriendRequest> rel = (from r in _db.FriendRequests
                                where r.FromUser == FromUser && r.ToUser == ToUser && r.Accepted == null && r.AcceptedTime == null
                                select r).ToList();

            if (rel.Count > 0)
                return true;
            else
                return false;
        }

        public static void deleteRequest(Guid FromUser, Guid ToUser)
        {
            List<FriendRequest> rel = (from r in _db.FriendRequests
                                       where r.FromUser == FromUser && r.ToUser == ToUser && r.Accepted == null && r.AcceptedTime == null
                                       select r).ToList();

            foreach (FriendRequest r in rel)
            {
                _db.FriendRequests.DeleteOnSubmit(r);
            }

            _db.SubmitChanges();
        }
    }
}