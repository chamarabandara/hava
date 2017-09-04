using HavaBusinessObjects.ControllerRepository;
using Newtonsoft.Json.Linq;
using System.Web.Mvc;

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

        [Authorize]
        public JObject Menues()
        {
            UserRepository userRepository = new UserRepository();
            return userRepository.GetMenues();
        }
        #endregion
    }
}