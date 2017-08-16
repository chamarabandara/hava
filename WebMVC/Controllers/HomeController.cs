using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HavaBusinessObjects.ControllerRepository;
using Newtonsoft.Json.Linq;

namespace WebMVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult Site()
        {
            // ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult Login()
        {
            // ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult Sites()
        {
            // ViewBag.Title = "Home Page";

            return View();
        }

        #region Get Nav Bar Menues

        public JObject Menues()
        {
            UserRepository userRepository = new UserRepository();
            return userRepository.GetMenues();
        }
        #endregion
    }
}