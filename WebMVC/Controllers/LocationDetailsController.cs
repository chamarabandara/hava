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
                var tt = User.Identity.Name;
                //JArray albums = JArray.Parse(JsonConvert.SerializeObject(result).Replace("\r\n", string.Empty)) as JArray;
                //returnObj.Add("data", albums);
                //return returnObj;

                JArray returnArr = new JArray();

                foreach (var item in result)
                {
                    JObject itemObj = new JObject();
                    itemObj.Add("Id", item.Id);
                    itemObj.Add("name", item.name);
                    itemObj.Add("PartnerId", item.PartnerId);
                    itemObj.Add("IsActive", item.IsActive);
                    itemObj.Add("FromLocation", item.FromLocation);
                    itemObj.Add("ToLocation", item.ToLocation);

                    returnArr.Add(itemObj);
                }
                returnObj.Add("data", returnArr);
                return returnObj;

            }
            catch (Exception ex)
            {
                returnObj.Add("error", ex.Message.ToString());
                return returnObj;
            }
        }


        [HttpGet]
        [AllowAnonymous]
        public JObject GetAll()
        {
            JObject returnObj = new JObject();

            try
            {
                var result = _locationDetailsRepository.GetAll();
                JArray returnArr = new JArray();

                foreach (var item in result)
                {
                    JObject itemObj = new JObject();
                    itemObj.Add("Id", item.Id);
                    itemObj.Add("name", item.name);
                    itemObj.Add("PartnerId", item.PartnerId);
                    itemObj.Add("IsActive", item.IsActive);
                    itemObj.Add("FromLocation", item.FromLocation);
                    itemObj.Add("ToLocation", item.ToLocation);

                    returnArr.Add(itemObj);
                }
                returnObj.Add("data", returnArr);
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
