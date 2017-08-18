using HavaBusinessObjects.ControllerRepository;
using HavaBusinessObjects.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebMVC.Controllers
{
    [RoutePrefix("api/booking")]
    public class BookingController : ApiController
    {
        private PartnerRepository _partnerRepository = new PartnerRepository();
        private BookingRepository _bookingRepository = new BookingRepository();

        [HttpGet]
        [Route("GetProducts/{partnerId}/{locationId}", Name = "GetPartnersProductsForLocations")]
        [AllowAnonymous]
        public IHttpActionResult GetPartnersProductsForLocations(int partnerId, int locationId)
        {
            try
            {
                var prodPrices = _partnerRepository.GetPartnerProducts(partnerId, locationId);
                if (prodPrices.Count() > 0)
                {
                    return Ok(prodPrices);
                }
                else
                {
                    return NotFound();
                }
                return Content(HttpStatusCode.OK, prodPrices);
            }
            catch (Exception ex)
            {
                return BadRequest("General Error");
            }
        }

        [HttpPost]
        [Route("", Name = "InserBooking")]
        [AllowAnonymous]
        public IHttpActionResult Insert(BookingViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var batteryQuality = _bookingRepository.Insert(vm);
                return CreatedAtRoute("GetBatteryQualityById", new { batteryQuality.Id }, batteryQuality);

            }
            return BadRequest(ModelState);
        }

        [HttpGet]
        [Route("GetById/{id}", Name = "GetById")]
        [AllowAnonymous]
        public IHttpActionResult GetById(int id)
        {
            try
            {
                return Ok(_bookingRepository.GetById(id));
            }
            catch (Exception ex)
            {
                return BadRequest("General Error"); 
            }
        }
    }
}
