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
                var returnData = JsonConvert.SerializeObject(result).Replace("\"", "");
                returnObj.Add("data", returnData);
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
