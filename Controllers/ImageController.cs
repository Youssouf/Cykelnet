using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Web.Security;

namespace Cykelnet.Controllers
{
    public class ImageController : Controller
    {
        //
        // GET: /Image/
        
        [Authorize]
        public ActionResult ProfileAvatar(Guid? id)
        {
            
            if(!id.HasValue)
            {
                throw new HttpException(404, "No userid was provided!");
            }
            
            string filePath= Request.MapPath("~/Content/Images/Avatars/" + id.ToString() + ".png");
            
            if(!System.IO.File.Exists(filePath))
                return File(Request.MapPath("~/Content/Images/defaultavatar.png"),"image/png");
            else
                return File(filePath,"image/png");
        }
    }
}
