using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Cykelnet.Models;
using System.Web.Security;

namespace Cykelnet.Controllers
{
    public class RouteController : Controller
    {
        [Authorize]
        public ActionResult GMap(int? id)
        {
            if (id != null)
            {
                RouteModel rm = new RouteModel((int)id);
                return PartialView("Routes/_GMap", rm);
            }

            return PartialView("Routes/_GMap");
        }

        [Authorize]
        public JsonResult RouteCoords(int id)
        {
            RouteModel rm = new RouteModel(id);
            return Json(rm.linestringToJson());
        }

        [Authorize]
        public ActionResult PersonalRoutes()
        {
            return PartialView("Routes/_ListRoutes", RouteModel.getPersonalRoutes((Guid)Membership.GetUser().ProviderUserKey));
        }
        
        //
        // GET: /Route/ListRoutes
        public ActionResult ListRoutes()
        {
            List<RouteModel> rmList = RouteModel.getAllRoutes();
            return PartialView("Routes/_ListRoutes", rmList);
        }

        public ActionResult FavoriteRoutes()
        {
            List<RouteModel> rmList = RouteModel.getFavoriteRoutes((Guid)Membership.GetUser().ProviderUserKey);
            return PartialView("Routes/_ListRoutes", rmList);
        }

        public PartialViewResult ListRouteElement(RouteModel rm)
        {
            return PartialView("Routes/_ListRoutesElement", rm);
        }

        public PartialViewResult RatingVote(int id, bool vote)
        {
            Guid userId = (Guid)Membership.GetUser().ProviderUserKey;
            RatingModel rm = new RatingModel(userId, id, vote);

            RatingModel.insertRating(rm);

            RouteModel routem = new RouteModel(id);
            return PartialView("Routes/_ListRoutesElement", routem);
        }

        [Authorize]
        public PartialViewResult Favorite(int routeID, bool makeFavorite)
        {
            FavoriteModel fm = new FavoriteModel((Guid)Membership.GetUser().ProviderUserKey, routeID);
            if (makeFavorite)
                FavoriteModel.insertFavorite(fm);
            else
                FavoriteModel.deleteFavorite(fm);

            RouteModel rm = new RouteModel(routeID);
            return PartialView("Routes/_ListRoutesElement", rm);
        }

        //
        // GET: /Route/Create
        [Authorize]
        [HttpPost]
        public ActionResult Create(String jsonRoute, String routeName, String routeCity, String routeDescription, String routeTags)
        {
            if (jsonRoute == null)
                throw new HttpException("No route was specified");
            if (routeName == null)
                throw new HttpException("No route name was specified");
            if (routeCity == null)
                throw new HttpException("No route city was specified");

            // Parse json to a list
            IList<RoutePointModel> temp = new JavaScriptSerializer().Deserialize<IList<RoutePointModel>>(jsonRoute);
            // The deserializer cannot directly create SqlGeography (Point) in RoutePointModel
            List<RoutePointModel> js = new List<RoutePointModel>();
            foreach (RoutePointModel r in temp)
            {
                js.Add(new RoutePointModel(r.latitude, r.longitude));
            }


            // Parse routeTags to a list of Strings
            IList<String> tags = new JavaScriptSerializer().Deserialize<IList<String>>(routeTags);

            // Make persistent
            RouteModel rm = new RouteModel(js);
            rm.userID       = (Guid)Membership.GetUser().ProviderUserKey;
            rm.city         = routeCity;
            rm.name         = routeName;
            rm.description  = routeDescription != null ? routeDescription : "";
            rm.heightmeters = 0;
            rm.setTags(new List<String>(tags));

            int routeID = RouteModel.insertRoute(rm);
            rm.routeID = routeID;

            return new EmptyResult();
        }

        public ActionResult SearchViaTags(String searchString)
        {
            // Parse the search string
            List<String> searchList = new List<String>(searchString.Split(' '));

            List<RouteModel> ResultRoutes = RouteModel.searchRoutesViaTags(new List<String>(searchList));
            return PartialView("Routes/_ListRoutes", ResultRoutes);
        }

        public ActionResult SearchViaPoint(String Latitude, String Longitude)
        {
            Latitude = Latitude.Replace(".", ",");
            Longitude = Longitude.Replace(".", ",");

            RoutePointModel rpm = new RoutePointModel(double.Parse(Latitude), double.Parse(Longitude));
            List<RouteModel> ResultRoutes = RouteModel.getNearestRoutes(rpm, 10);
            return PartialView("Routes/_ListRoutes", ResultRoutes);
        }

        //
        // GET: /Route/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            RouteModel.deleteRoute(id);
            return RedirectToAction("Index", "Home");
        }
    }
}
