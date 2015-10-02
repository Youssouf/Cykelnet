using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Configuration;

namespace Cykelnet.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.Message = "Welcome to ASP.NET MVC, " + User.Identity.Name + "!";
                ViewBag.Username = User.Identity.Name;
            }
            else
            {
                ViewBag.Message = "Welcome to ASP.NET MVC!";
            }
            return View();
        }

        //public ActionResult Test()
        //{
        //    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CykelnetConnectionString"].ConnectionString);

        //    byte[] password = hashPassword("kodeord");

        //    string query = "UPDATE LastChangeUsers SET UserPassword = @password WHERE UserName = @UserName";

        //    var command = new SqlCommand(query, con);
        //    command.Parameters.AddWithValue("@password", password);
        //    command.Parameters.AddWithValue("@UserName", "Dimsedane");

        //    con.Open();
        //    var result = command.ExecuteNonQuery();
        //    con.Close();

        //    return new EmptyResult();
        //}
        
        public ActionResult About()
        {
            return View();
        }

        private byte[] hashPassword(string password)
        {
            var crypt = SHA512.Create();

            return crypt.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }
}
