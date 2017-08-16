using HavaBusinessObjects.ControllerRepository;
using HavaBusinessObjects.ViewModels;
using Newtonsoft.Json.Linq;
using System.Web.Mvc;

namespace WebMVC.Controllers
{
    public class TSPController : Controller
    {
        //
        // GET: /TSP/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Add()
        {
            return View();
        }

        public ActionResult TSPView()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        [HttpGet]
        public JObject GetProducts()
        {

            TSPRepository tspRepository = new TSPRepository();
            return tspRepository.GetProducts();
        }

        [HttpPost]
        public JObject AddTSP(TSPDetailViewModel tspViewModel)
        {
            JObject returnObj = new JObject();
            TSPRepository tspRepository = new TSPRepository();
            tspViewModel.createdBy = 1;
            returnObj.Add("status", tspRepository.SaveTSP(tspViewModel));
            return returnObj;
        }
    }
}