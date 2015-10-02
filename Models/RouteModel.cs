using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.SqlServer.Types;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using Cykelnet.Models.Events;
using System.Data.SqlTypes;
using System.Text;


namespace Cykelnet.Models
{
    public class RouteModel
    {
        static CykelnetDBDataContext _db = new CykelnetDBDataContext();

        public int routeID;
        public SqlGeography routeLineString;
        public Guid userID;
        public String name;
        public String city;
        public int heightmeters;
        public String description;
        public List<TagModel> tags;
        public List<RatingModel> ratings;

        public RouteModel()
        {
        }

        public RouteModel(int id)
        {
            Route r             = RouteModel.getRoute(id);

            this.routeID        = r.RouteID;
            this.userID         = r.User_ID;
            this.city           = r.City;
            this.name           = r.Name;
            this.heightmeters   = r.Heightmeters;
            this.description    = r.Description;
            this.routeLineString = RouteModel.getLineString(id);
            this.tags           = TagModel.getRouteTags(this);
            this.ratings        = RatingModel.getRating(id);
        }

        public RouteModel(Route r)
        {
            this.routeID        = r.RouteID;
            this.userID         = r.User_ID;
            this.city           = r.City;
            this.name           = r.Name;
            this.heightmeters   = r.Heightmeters;
            this.description    = r.Description;
            this.routeLineString = RouteModel.getLineString(r.RouteID);
            this.tags           = TagModel.getRouteTags(this);
            this.ratings        = RatingModel.getRating(r.RouteID);
        }

        public RouteModel(IList<RoutePointModel> pointList)
        {
            this.routeLineString = this.generateLineString(pointList);
        }

        public static string getRouteName(int RouteID)
        {
            string name = (from n in _db.Routes
                           where n.RouteID == RouteID
                           select n.Name).SingleOrDefault();
            return name;
        }

        public bool isFavorite(Guid user)
        {
            var favquery = from f in _db.Favorites
                           where f.User_ID == user
                           && f.Route_ID == this.routeID
                           select f;

            if (favquery.Count() > 0)
                return true;
            else
                return false;
        }

        public bool isUpvoted(Guid user)
        {
            var upvote = (from ra in _db.Ratings
                          where ra.User_ID == user
                              && ra.Route_ID == this.routeID
                              && ra.Rate == true
                          select ra);
            if (upvote.Count() > 0)
                return true;
            else
                return false;
        }

        public bool isDownvoted(Guid user)
        {
            var upvote = (from ra in _db.Ratings
                          where ra.User_ID == user
                              && ra.Route_ID == this.routeID
                              && ra.Rate == false
                          select ra);
            if (upvote.Count() > 0)
                return true;
            else
                return false;
        }


        public int getRatingLikes()
        {
            int likeCount = 0;
            foreach (RatingModel r in this.ratings)
            {
                if (r.Like)
                    likeCount++;
            }
            return likeCount;
        }

        public double getUpvotePercentage()
        {
            if (this.ratings.Count == 0)
                return 50;
            int likes = this.getRatingLikes();
            double result = (double)(((double)likes / (double)this.ratings.Count) * (double)100);

            if (result < (double)1.0)
                return (double)1.0;

            return result;
        }

        public double getDownvotePercentage()
        {
            double uvp = this.getUpvotePercentage();
            return (double)100 - uvp;
        }

        public static List<RouteModel> getPersonalRoutes(Guid id)
        {
            List<Route> query = (from r in _db.Routes
                                  where r.User_ID == id
                                  && r.ValidTo == null
                                  select r).ToList();

            List<RouteModel> result = new List<RouteModel>();
            foreach (Route r in query)
            {
                result.Add(new RouteModel(r));
            }

            return result;
        }

        public static List<RouteModel> getFavoriteRoutes(Guid id)
        {
            List<Route> rl = (from F in _db.Favorites
                              join R in _db.Routes on F.Route_ID equals R.RouteID
                              where F.User_ID == id && R.ValidTo == null
                              select R).ToList();

            List<RouteModel> result = new List<RouteModel>();
            foreach (Route r in rl)
            {
                result.Add(new RouteModel(r));
            }

            return result;
        }

        public String linestringToJson()
        {
            List<RoutePointModel> pointList = this.linestringToRoutePointModel();
            StringBuilder builder = new StringBuilder();
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();

            jsSerializer.Serialize(pointList, builder);
            
            return builder.ToString();
        }

        public List<RoutePointModel> linestringToRoutePointModel()
        {
            List<RoutePointModel> rpmList = new List<RoutePointModel>();
            for(int i=0; i<this.routeLineString.STNumPoints(); i++)
            {
                RoutePointModel rpm = new RoutePointModel(this.routeLineString.STPointN(i+1));
                
                rpmList.Add(rpm);
            }
            return rpmList;            
        }

        public SqlGeography generateLineString(IList<RoutePointModel> points)
        {
            SqlGeographyBuilder builder = new SqlGeographyBuilder();
            builder.SetSrid(4326);
            builder.BeginGeography(OpenGisGeographyType.LineString);
            for (int i = 0; i < points.Count; i++)
            {
                if (i == 0)
                {
                    // First element, create figure
                    builder.BeginFigure(points.ElementAt(0).latitude, points.ElementAt(0).longitude);
                }
                else
                {
                    builder.AddLine(points.ElementAt(i).latitude, points.ElementAt(i).longitude);
                }
            }
            builder.EndFigure();
            builder.EndGeography();

            return builder.ConstructedGeography;
        }

        public static SqlGeography getLineString(int routeID)
        {
            SqlConnection conn = new SqlConnection(_db.Connection.ConnectionString);
            // Select the SPROC to use
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "GetRouteLineString";
            cmd.CommandType = CommandType.StoredProcedure;

            // Add arguments to the SPROC
            cmd.Parameters.Add(new SqlParameter("@ID", routeID));

            conn.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            // Prepare for the result
            SqlGeography routeLineString = new SqlGeography();

            // Read output from SPROC
            while (reader.Read())
            {
                // Read the "RouteLineString" attribute
                routeLineString = (SqlGeography)reader["RouteLineString"];                
            }

            reader.Close();
            conn.Close();
            return routeLineString;
        }

        public String getLength()
        {
            // Meters
            double len = (double)this.routeLineString.STLength();

            String result = "";

            // Make the numbers "nice"
            // More than 1km length
            if(len >= 1000)
            {
                len = len / 1000;
                len = Math.Round(len, 2);
                result = len.ToString() + " " + "km";
            }
            else
            {
                len = Math.Round(len, 2);
                result = len.ToString() + " " + "m";
            }

            return result;
        }

        public static int insertRoute(RouteModel rm)
        {
            // Create route tuple (meta data)
            Route r = new Route();
            r.User_ID = rm.userID;
            r.City = rm.city;
            r.Name = rm.name;
            r.Heightmeters = rm.heightmeters;
            r.Description = rm.description;
            r.ValidFrom = DateTime.Now;
            _db.Routes.InsertOnSubmit(r);
            _db.SubmitChanges();

            // Create event, that route was created
            RouteCreatedEvent rce = new RouteCreatedEvent(r);
            rce.save();

            // Add tags
            List<Tag> insTags = TagModel.insertTags(rm.tags);
            if (insTags != null)
            {

                List<Routetag> rtags = new List<Routetag>();
                foreach (Tag t in insTags)
                {
                    Routetag rt = new Routetag();
                    rt.Route_ID = r.RouteID;
                    rt.Tag_ID = t.Tag_ID;
                    rtags.Add(rt);
                }
                _db.Routetags.InsertAllOnSubmit(rtags);
                _db.SubmitChanges();
            }

            // Update inputted routemodel with id given by DB
            rm.routeID = r.RouteID;

            // Select the SPROC to use
            SqlConnection conn = new SqlConnection(_db.Connection.ConnectionString);
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "InsertRouteLineString";
            cmd.CommandType = CommandType.StoredProcedure;

            // Add arguments to the SPROC
            SqlParameter param = new SqlParameter("@LINESTRING", SqlDbType.Udt);
            param.SqlDbType = SqlDbType.Udt;
            param.UdtTypeName = "geography";
            param.Value = rm.routeLineString;

            cmd.Parameters.Add(param);

            cmd.Parameters.Add(new SqlParameter("@ROUTEID", r.RouteID));

            cmd.ExecuteNonQuery();

            return r.RouteID;
        }

        /// <summary>
        /// Deletes a route from DB
        /// </summary>
        /// <param name="routeID">ID of the route to be deleted</param>
        public static void deleteRoute(int routeID)
        {
            // Find route in db, and mark it as deleted
            Route route = (from r in _db.Routes
                           where r.RouteID == routeID
                           select r).Single();

            route.ValidTo = DateTime.Now;
            _db.SubmitChanges();

            // Delete routetags for the route
            TagModel.deleteAllTags(routeID);

            // Delete events for the route
            EventModel.deleteAllEvents(routeID);
        }

        public static List<RouteModel> getAllRoutes()
        {
            List<Route> rList = (from r in _db.Routes
                                 where r.ValidTo == null
                                       select r).ToList();

            List<RouteModel> rmList = new List<RouteModel>();
            foreach (Route r in rList)
            {
                rmList.Add(new RouteModel(r));
            }
            return rmList;
        }

        private static Route getRoute(int routeId)
        {
            return (from r in _db.Routes
                           where r.RouteID == routeId
                           select r).Single();
        }

        public void setTags(List<String> st)
        {
            this.tags = TagModel.createTags(st);
        }

        public void setTags(List<TagModel> t)
        {
            this.tags = t;
        }

        public static List<Route> getRoutesFromMeta(List<TagModel> searchtags)
        {
            String tagString = "";

            for (int i = 0; i < searchtags.Count; i++)
            {
                if (i > 0) tagString += " OR ";
                else tagString += " JOIN routetags ON routes.RouteID = RouteTags.Route_ID WHERE ";

                tagString += "Tag_ID = " + searchtags.ElementAt(i).TagId;
            }

            String query = "SELECT * FROM routes" + tagString;
            return _db.ExecuteQuery<Route>(query).ToList<Route>();
        }

        public static List<RouteModel> searchRoutesViaTags(String str)
        {
            List<String> lstr = new List<String>();
            lstr.Add(str);
            return searchRoutesViaTags(lstr);
        }

        public static List<RouteModel> searchRoutesViaTags(List<String> strs)
        {
            // The joins are there because it is not possible to select from
            // multiple tables
            // strs.Contains(T.TagName) is the same as the set membership operator 'IN'
            List<Route> routes = (from R in _db.Routes
                                  join TR in _db.Routetags on R.RouteID equals TR.Route_ID
                                  join T in _db.Tags on TR.Tag_ID equals T.Tag_ID
                                  where strs.Contains(T.TagName)
                                  select R).Distinct().ToList();

            List<RouteModel> result = new List<RouteModel>();
            foreach (Route r in routes)
            {
                result.Add(new RouteModel(r));
            }

            return result;
        }

        private static IQueryable<Routetag> SearchProducts(List<TagModel> searchtags)
        {
            IQueryable<Routetag> query = _db.Routetags;

            foreach (TagModel tm in searchtags)
            {
                int temp = tm.TagId;
                query = query.Where(p => p.Tag_ID.Equals(temp));
            }
            return query;
        }

        /// <summary>
        /// Retrieve the routes closest to the provided RoutePointModel
        /// </summary>
        /// <param name="rpm">The RPM to search near.</param>
        /// <param name="amountOfRoutes">The amount of routes to retrieve (at most).</param>
        /// <returns>The nearest routes, sorted by distance (shortest distance first).</returns>
        public static List<RouteModel> getNearestRoutes(RoutePointModel rpm, int amountOfRoutes)
        {
            SqlConnection conn = new SqlConnection(_db.Connection.ConnectionString);
            
            // Select the SPROC to use
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "NNSearch";
            cmd.CommandType = CommandType.StoredProcedure;

            // Add arguments to the SPROC

            // The amount of routes to return
            cmd.Parameters.Add(new SqlParameter("@AMOUNT", amountOfRoutes));

            // The point to find NN's for
            SqlParameter param = new SqlParameter("@POINT", SqlDbType.Udt);
            param.SqlDbType = SqlDbType.Udt;
            param.UdtTypeName = "geography";
            param.Value = rpm.asSqlGeography();
            cmd.Parameters.Add(param);

            // Prepare for the result
            List<RouteModel> result = new List<RouteModel>();
            
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            // Read output from SPROC
            while (reader.Read())
            {
                // Read the "RouteLineStringID" attribute
                result.Add(new RouteModel((int)reader["RouteID"]));
            }

            reader.Close();
            conn.Close();
            return result;
        }


        /* OLD IMPLEMENTATION
        /// <summary>
        /// Retrieve the routes closest to the provided RoutePointModel
        /// </summary>
        /// <param name="rpm">The RPM to search near.</param>
        /// <param name="amountOfRoutes">The amount of routes to retrieve (at most).</param>
        /// <returns>The nearest routes, sorted by distance (shortest distance first).</returns>
        public static List<RouteModel> getNearestRoutes(RoutePointModel rpm, int amountOfRoutes)
        {
            List<RouteModel> allRoutes = getAllRoutes();
            List<RouteModel> result = new List<RouteModel>();

            if (allRoutes.Count > 0)
            {
                while (result.Count < amountOfRoutes)
                {
                    RouteModel rm = findNearestRoute(allRoutes, rpm);
                    if (rm != null)
                    {
                        result.Add(rm);
                        allRoutes.Remove(rm);
                    }
                    else
                    {
                        // Could not find enough routes, return what we have
                        // Will only happen if we run out of routes in allRoutes
                        return result;
                    }
                }
            }

            return result;
        }

        private static RouteModel findNearestRoute(List<RouteModel> routes, RoutePointModel point)
        {
            RouteModel closestRoute = null;
            // Set initial closest distance to the circumferance of earth + ~10.000 km
            double closestDistance = 50000000;
            foreach (RouteModel r in routes)
            {
                // Find distance to our point
                double currentDistance = (double)r.routeLineString.STDistance(point.asSqlGeography());
                if (currentDistance < closestDistance)
                {
                    // New closest route found
                    closestRoute = r;
                    closestDistance = currentDistance;
                }
            }

            return closestRoute;
        }*/
    }
}
