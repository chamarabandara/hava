using HavaBusinessObjects.ControllerRepository;
using HavaBusinessObjects.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Web.Mvc;

namespace WebMVC.Controllers
{

    public class BookingController : Controller
    {
        private PartnerRepository _partnerRepository = new PartnerRepository();
        private BookingRepository _bookingRepository = new BookingRepository();

        [HttpGet]
        [AllowAnonymous]
        public JObject GetProducts(int partnerId, int locationId)
        {
            JObject returnObj = new JObject();
            try
            {
                var prodPrices = _partnerRepository.GetPartnerProducts(partnerId, locationId);
                if (prodPrices.Count() > 0)
                {
                   // returnObj.Add("data", JsonConvert.SerializeObject(prodPrices).Replace("\\", ""));

                    var returnData = JsonConvert.SerializeObject(prodPrices).Replace("\"", "");
                    returnObj.Add("data", returnData);
                    return returnObj;                    
                }
                else
                {
                    return returnObj;
                }
                
            }
            catch (Exception ex)
            {
                returnObj.Add("error", ex.Message.ToString());
                return returnObj;
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public JObject Insert(BookingViewModel vm)
        {
            JObject returnObj = new JObject();

            if (ModelState.IsValid)
            {
                var booking = _bookingRepository.Insert(vm);
                returnObj.Add("data", JsonConvert.SerializeObject(booking));
                return returnObj;

            }
            returnObj.Add("error", "Please provide mandatory fields");
            return returnObj;
        }

        [HttpGet]
        [AllowAnonymous]
        public JObject GetById(int id)
        {
            JObject returnObj = new JObject();
            try
            {
                var booking = _bookingRepository.GetById(id);

                var returnData = JsonConvert.SerializeObject(booking).Replace("\"", "");
                returnObj.Add("data", returnData);
                return returnObj;

            }
            catch (Exception ex)
            {
                returnObj.Add("error", "General Error");
                return returnObj;
            }
        }


        //public System.Web.Mvc
    }
}
