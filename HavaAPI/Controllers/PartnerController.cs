using HavaBusinessObjects.ControllerRepository;
using HavaBusinessObjects.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Web.Mvc;

namespace HavaBusinessObject.Controllers
{
    public class PartnerController : Controller
    {
        //
        // GET: /Partner/
        public ActionResult Index()
        {
            return View();
        }

        #region add customer
        /// <summary>
        /// Adds the specified Partner.
        /// Date		    Author/(Reviewer)		    Description
        /// -------------------------------------------------------	
        /// 15 June 2017     Chamara Bandara          Creation
        /// </summary>
        /// <param name="customerViewModel">Adds the specified Partner.</param>
        /// <returns></returns>
        public JObject Post(PartnerViewModel partnerViewModel)
        {
            try
            {
                PartnerRepository partnerRepository = new PartnerRepository();

                return new JObject();
                // return partnerRepository.AddPartners(partnerViewModel, User.Identity.GetUserName());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        //public JObject Get(int Id) {

        //    PartnerRepository partnerRepository = new PartnerRepository();

        //    return partnerRepository.GetPartner(Id);
        //}

        public JObject GetList()
        {

            PartnerRepository partnerRepository = new PartnerRepository();

            return partnerRepository.GetPartner();
        }
    }
}