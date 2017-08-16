using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HavaWeb.Controllers
{
    public class WidgetsController : Controller
    {
        //
        // GET: /Widgets/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult navbar()
        {
            return View();
        }
        public ActionResult statics()
        {
            return View();
        }
        public ActionResult tiles()
        {
            return View();
        }
        public ActionResult navLoogedUser()
        {
            return View("nav-looged-user");
        }
	}
}