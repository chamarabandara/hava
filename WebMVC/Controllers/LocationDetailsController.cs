using HavaBusinessObjects.ControllerRepository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace WebMVC.Controllers
{
   
    public class LocationDetailsController : Controller
    {
        private LocationDetailsRepository _locationDetailsRepository = new LocationDetailsRepository();

        [HttpGet]        
        [AllowAnonymous]
        public JObject GetAllByPartnerId(int id)
        {
            JObject returnObj = new JObject();

            try
            {                
                var result = _locationDetailsRepository.GetAllByPartnerId(id);
                JArray albums = JArray.Parse(JsonConvert.SerializeObject(result)) as JArray;
                JObject jalbum = albums[0] as JObject;
                returnObj.Add("data", jalbum);
                return returnObj;

            }
            catch (Exception ex)
            {
                returnObj.Add("error", ex.Message.ToString());
                return returnObj;
            }
        }
    }
}
