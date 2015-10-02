using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using Cykelnet.Models.Events;

namespace Cykelnet.Models
{
    public class FavoriteModel
    {
        static CykelnetDBDataContext _db = new CykelnetDBDataContext();

        private Guid userID;
        private int routeID;


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


        public FavoriteModel()
        {
        }

        public FavoriteModel(Guid user, int route)
        {
            this.userID = user;
            this.routeID = route;
        }

        public static void insertFavorite(FavoriteModel fm)
        {
            // Check if already favored
            Favorite favorite = (from f in _db.Favorites
                                 where f.Route_ID == fm.routeID
                                 && f.User_ID == fm.userID
                                 select f).SingleOrDefault();

            if (favorite == null)
            {
                favorite = new Favorite();
                favorite.User_ID = fm.userID;
                favorite.Route_ID = fm.routeID;
                _db.Favorites.InsertOnSubmit(favorite);
                _db.SubmitChanges();

                // Create event that this route has been favored
                RouteFavoredEvent rfe = new RouteFavoredEvent(favorite);
                rfe.save();
            }
        }

        public static void deleteFavorite(FavoriteModel fm)
        {
            Favorite favorite = (from r in _db.Favorites
                          where r.Route_ID == fm.routeID
                          && r.User_ID == fm.userID
                          select r).SingleOrDefault();

            if (favorite != null)
            {
                _db.Favorites.DeleteOnSubmit(favorite);
                _db.SubmitChanges();
            }
        }
    }
}