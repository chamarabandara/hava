using HavaBusinessObjects.ControllerRepository;
using HavaBusinessObjects.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Web.Mvc;
namespace HavaWeb.Controllers
{
    public class PartnerController : Controller
    {
        //
        // GET: /Partner/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Add()
        {
            return View();
        }

        public ActionResult PartnerView()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        public JObject GetList()
        {

            PartnerRepository partnerRepository = new PartnerRepository();

            return partnerRepository.GetPartner();
        }

        [HttpGet]
        public JObject GetProductList()
        {

            PartnerRepository partnerRepository = new PartnerRepository();

            return partnerRepository.GetProduct();
        }

        [HttpGet]
        public JObject GetSites()
        {

            SitesRepository siteRepository = new SitesRepository();

            return siteRepository.GetPartner();
        }

        #region add Product
        /// <summary>
        /// Adds the specified Product.
        /// Date		    Author/(Reviewer)		    Description
        /// -------------------------------------------------------	
        /// 11 Aug 2015     Chamara Bandara           Creation
        /// </summary>
        /// <param name="customerViewModel">The customer view model.</param>
        /// <returns></returns>
        [HttpPost]
        public JObject Post(PartnerViewModel partnerViewModel)
        {
            try
            {
                JObject obj = new JObject();
                PartnerRepository partnerRepository = new PartnerRepository();
                bool status = partnerRepository.SavePartner(partnerViewModel);
                obj.Add("status" , status);
                return obj;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}