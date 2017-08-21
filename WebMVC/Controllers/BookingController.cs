using HavaBusinessObjects.ControllerRepository;
using HavaBusinessObjects.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
                    returnObj.Add("data", JsonConvert.SerializeObject(prodPrices));
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
        
        //[HttpPost]
        //[Route("", Name = "InserBooking")]
        //[AllowAnonymous]
        //public IHttpActionResult Insert(BookingViewModel vm)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var batteryQuality = _bookingRepository.Insert(vm);
        //        return CreatedAtRoute("GetBatteryQualityById", new { batteryQuality.Id }, batteryQuality);

        //    }
        //    return BadRequest(ModelState);
        //}

        //[HttpGet]
        //[Route("GetById", Name = "GetById")]
        //[AllowAnonymous]
        //public IHttpActionResult GetById(int id)
        //{
        //    try
        //    {
        //        return Ok(_bookingRepository.GetById(id));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest("General Error"); 
        //    }
        //}


        //public System.Web.Mvc
    }
}
