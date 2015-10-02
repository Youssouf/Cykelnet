using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cykelnet.Models.Algorithms.Utils;

namespace Cykelnet.Models.Algorithms
{
    public class Recommender
    {
        static CykelnetDBDataContext _db = new CykelnetDBDataContext();
        private static String _connectionString = _db.Connection.ConnectionString;
        

        private int confidenceThreshold = 50;
        private int suggestionThreshold = 50;

        private Guid userId;

        //List of the ratings that the current user has made.
        private Dictionary<int, RatingModel> userRatings = new Dictionary<int,RatingModel>();

        public Recommender(Guid id)
        {
            this.userId = id;
        }

        public void updateRecommendations()
        {
            clearUserSuggestions();
            List<RatingModel> r = retrieveRatings();
            if (r != null && r.Count > 0)
            {
                List<RecUser> u = getAgreeingUsers(r);
                if (u != null && u.Count > 0)
                {
                    u = getRecUserRoutes(u);
                    if (u != null && u.Count > 0)
                    {
                        Dictionary<int, int> suggestions = routesSuggested(u);
                        if (suggestions != null && suggestions.Count > 0)
                        {
                            insertSuggestions(suggestions);
                        }
                    }
                }
            }

        }

        public static void recommendAllUsers(List<Guid> users) 
        {
            _db.ExecuteCommand("TRUNCATE table Recommendations");
            foreach (Guid user in users)
            {
                Recommender r = new Recommender(user);
                r.updateRecommendations();
            }
        }

        //Retrieve ratings from the database that concerns routes, the provided user has rated
        //Additionally, must set userratings
        private List<RatingModel> retrieveRatings ()
        {
            List<Rating> temp = (from r in _db.Ratings
                               where r.User_ID == userId
                               select r).ToList();
            
            if(temp.Count != 0)
            {
                String ratingString = " WHERE";
                foreach(Rating r in temp)
                {
                    if(ratingString.Length > 6)
                    {
                        ratingString += " OR";
                    }
                    this.userRatings.Add(r.Route_ID, new RatingModel(r));

                    ratingString += " Route_ID = " + r.Route_ID;
                }
                String query = "SELECT * FROM ratings" + ratingString;
                temp = _db.ExecuteQuery<Rating>(query).ToList<Rating>();

                if(temp.Count != 0)
                {
                    List<RatingModel> returnlist = new List<RatingModel>();
                    foreach(Rating r in temp)
                    {
                        returnlist.Add(new RatingModel(r));
                    }
                    return returnlist;
                }
            }
            return null;
        }

        //Get all users that are in agreement with the provided user.
        private List<RecUser> getAgreeingUsers (List<RatingModel> ratings)
        {
            Dictionary<Guid, List<RatingModel>> splitUsers = new Dictionary<Guid, List<RatingModel> >();
            foreach (RatingModel rm in ratings) 
            {
                if (rm.UserID != this.userId)
                {
                    List<RatingModel> l;
                    if (!splitUsers.TryGetValue(rm.UserID, out l))
                    {
                        splitUsers.Add(rm.UserID, l = new List<RatingModel>());
                    }
                    l.Add(rm);
                }
            }

            List<RecUser> users = new List<RecUser>();
            foreach (KeyValuePair<Guid, List<RatingModel>> element in splitUsers) 
            {
                RecUser temp = new RecUser(element.Key, 0);
                foreach (RatingModel rm in element.Value) 
                {
                    temp.addConfidence(userRatings[rm.RouteID].Like == rm.Like);
                }                
                if (temp.Confidence > confidenceThreshold)
                    users.Add(temp);
            }

            if (users.Count > 0)
                return users;
            
            return null;
        }

        //Retrieve ratings for routes agreeing users have rated, but the originalUser have not yet rated
        //Must utilize the userRatings list (instance variable)
        private List<RecUser> getRecUserRoutes (List<RecUser> recs)
        {
            string userRoutes = "";
            foreach (RatingModel rm in userRatings.Values)
            {
                if (userRoutes.Length > 0) userRoutes += " AND";
                userRoutes += " Route_ID != " + rm.RouteID;
            }

            List<RecUser> usable = new List<RecUser>();
            foreach (RecUser rec in recs)
            {
                string query = "SELECT * FROM ratings WHERE User_ID = '" + rec.UserId + "' AND " + userRoutes;
                List<Rating> temp = _db.ExecuteQuery<Rating>(query).ToList<Rating>();

                if (temp.Count != 0)
                {
                    foreach (Rating t in temp)
                    {
                        RatingModel rm = new RatingModel(t);
                        rec.AddRating(rm);
                    }
                    usable.Add(rec);
                }
            }
            return usable;
        }

        //List all routes the user might be interested in, sorted on its confidence rating
        private Dictionary<int, int> routesSuggested (List<RecUser> users)
        {
            Dictionary<int, List<int>> temp = new Dictionary<int, List<int>>();

            foreach (RecUser ruser in users)
            {
                foreach(RatingModel rm in ruser.Ratings)
                {
                    List<int> rates;
                    if (!temp.TryGetValue(rm.RouteID, out rates))
                    {
                        temp.Add(rm.RouteID, rates = new List<int>());
                    }
                    if (rm.Like)
                        rates.Add(ruser.Confidence);
                    else rates.Add( - ruser.Confidence);
                }
            }

            Dictionary<int, int> ret = new Dictionary<int, int>();
            foreach(KeyValuePair<int, List<int>> element in temp)
            {
                int counter = 0;
                double conf = 0;

                foreach(int i in element.Value)
                {
                    counter++;
                    if (i < 0)
                    {
                        int j = i * (-1);
                        conf -= ((4.0 / 405.0) * j * j) + ((-13.0 / 27.0) * j) + (4000.0 / 81.0);
                    } else conf += ((4.0/405.0)*i*i) + ((-13.0/27.0)*i) + (4000.0/81.0);
                }
                int final = (int) (conf/counter);
                if( final > suggestionThreshold )
                    ret.Add(element.Key, final);
            }

            return ret;
        }

        //Insert suggestions to database
        private void insertSuggestions(Dictionary<int, int> confidenceRatedSuggestions)
        {
            List<Recommendation> result = new List<Recommendation>();
            foreach (KeyValuePair<int, int> element in confidenceRatedSuggestions)
            {
                Recommendation r = new Recommendation();
                r.User_ID = userId;
                r.Route_ID = element.Key;
                r.Confidence = element.Value;
//                result.Add(r);
                _db.Recommendations.InsertOnSubmit(r);
                _db.SubmitChanges();
            }

//            _db.Recommendations.InsertAllOnSubmit(result);
//            _db.SubmitChanges();
        }

        private void clearUserSuggestions()
        {
            String query = "DELETE FROM recommendations WHERE User_ID = '" + userId + "'";
            _db.ExecuteCommand(query);
        }
    }
}