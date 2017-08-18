using HavaBusinessObjects.ControllerRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebMVC.Controllers
{
    [RoutePrefix("api/locationDetails")]
    public class LocationDetailsController : ApiController
    {
        private LocationDetailsRepository _locationDetailsRepository = new LocationDetailsRepository();

        [HttpGet]
        [Route("GetAllByPartnerId/{id}", Name = "GetAllByPartnerId")]
        [AllowAnonymous]
        public IHttpActionResult GetAllByPartnerId(int id)
        {
            try
            {
                return Ok(_locationDetailsRepository.GetAllByPartnerId(id));
            }
            catch (Exception ex)
            {
                return BadRequest("General Error");
            }
        }
    }
}
