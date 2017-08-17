using HavaBusinessObjects;
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
            Utility utility = new Utility();
            //tspViewModel.createdBy = utility.GetUserId(User.Identity.Name).Value;
            tspViewModel.createdBy = 1;
            returnObj.Add("status" , tspRepository.SaveTSP(tspViewModel));
            return returnObj;
        }

        [HttpGet]
        public JObject GetList()
        {
            TSPRepository tspRepository = new TSPRepository();
            return tspRepository.GetTSP();
        }

        [HttpGet]
        public JObject GetTSPById(int id)
        {
            TSPRepository tspRepository = new TSPRepository();
            return tspRepository.GetTSPById(id);
        }

        [HttpPost]
        public JObject EditTSP(TSPDetailViewModel tspViewModel)
        {
            JObject returnObj = new JObject();
            TSPRepository tspRepository = new TSPRepository();
            Utility utility = new Utility();
            //tspViewModel.createdBy = utility.GetUserId(User.Identity.Name).Value;
            tspViewModel.createdBy = 1;
            returnObj.Add("status" , tspRepository.UpdateTSP(tspViewModel));
            return returnObj;
        }
    }
}