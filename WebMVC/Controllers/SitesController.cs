using HavaBusinessObjects.ControllerRepository;
using HavaBusinessObjects.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebMVC.Controllers
{
    public class SitesController : Controller
    {
        //
        // GET: /Sites/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Add()
        {
            return View();
        }

        public ActionResult SitesView()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        #region add Sites
        /// <summary>
        /// Adds the specified Sites.
        /// Date		    Author/(Reviewer)		    Description
        /// -------------------------------------------------------	
        /// 11 Aug 2015     Chamara Bandara           Creation
        /// </summary>
        /// <param name="customerViewModel">The sites view model.</param>
        /// <returns></returns>
        public JObject Post(SitesViewModel sitesViewModel)
        {
            try
            {
                JObject obj = new JObject();
                SitesRepository sitesRepository = new SitesRepository();
                bool status = sitesRepository.SaveSites(sitesViewModel);
                obj.Add("status", status);
                return obj;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        public JObject GetList()
        {

            SitesRepository sitesRepository = new SitesRepository();

            return sitesRepository.GetPartner();
        }

    }
}