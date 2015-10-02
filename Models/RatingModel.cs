using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace Cykelnet.Models
{
    public class RatingModel
    {
        static CykelnetDBDataContext _db = new CykelnetDBDataContext();

        private Guid userID;
        private int routeID;
        private bool like;

           public Guid UserID
        {
            get { return userID; }
            set { userID = value; }
        }
        
        public int RouteID
        {
            get { return routeID; }
            set { routeID = value; }
        }
          public bool Like
        {
            get { return like; }
            set { like = value; }
        }
        
        public RatingModel()
        {
        }

        public RatingModel(Rating r)
        {
            this.userID = r.User_ID;
            this.routeID = r.Route_ID;
            this.like = r.Rate;
        }

        public RatingModel(Guid user, int route, bool rating)
        {
            this.userID = user;
            this.routeID = route;
            this.like = rating;
        }

        public static void insertRating(RatingModel rm)
        {
            // Check if rating for the user already exists
            Rating rating = (from r in _db.Ratings
                             where r.User_ID == rm.userID
                             && r.Route_ID == rm.routeID
                             select r).SingleOrDefault();

            // None exists
            if (rating == null)
            {
                rating = new Rating();
                rating.User_ID = rm.userID;
                rating.Route_ID = rm.routeID;
                rating.Rate = rm.like;
                _db.Ratings.InsertOnSubmit(rating);
            }
            else
            {
                rating.Rate = rm.like;
            }

            _db.SubmitChanges();        
        }

        public static void DeleteRating(int ratingID)
        {
            Rating rating = (from r in _db.Ratings
                             where r.Rate_ID == ratingID
                             select r).Single();
            _db.Ratings.DeleteOnSubmit(rating);
            _db.SubmitChanges();
        }

        public static List<RatingModel> getRating(int routeId)
        {
            var ratings = (from r in _db.Ratings
                           where r.Route_ID == routeId
                           select r);

            List<RatingModel> result = new List<RatingModel>();
            foreach (Rating r in ratings)
            {
                result.Add(new RatingModel(r));
            }

            return result;
        }
    }
}