using HavaBusinessObjects;
using HavaBusinessObjects.ControllerRepository;
using HavaBusinessObjects.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
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
        private UserRepository _userRepository = new UserRepository();
        private HavaBusinessObjects.Utility _utility = new HavaBusinessObjects.Utility();


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
                    JArray albums = JArray.Parse(JsonConvert.SerializeObject(prodPrices).Replace("\\", "")) as JArray;
                    //JObject jalbum = albums[0] as JObject;
                    returnObj.Add("data", albums);

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
                var user = _utility.GetUserById(vm.UserId.Value);

                string messageBody = string.Empty;
                string[] tomail = new string[1];
                tomail[0] = user.Email;

                string[] ccmail = new string[1];
                ccmail[0] = user.Email;

               bool isSend = _utility.SendMailToRecepients(tomail, ccmail, messageBody, "Confirm booking");

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
                JObject albums = JObject.Parse(JsonConvert.SerializeObject(booking)) as JObject;
                //JObject jalbum = albums[0] as JObject;
                returnObj.Add("data", albums);
                return returnObj;

            }
            catch (Exception ex)
            {
                returnObj.Add("error", "General Error");
                return returnObj;
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        public JObject GetBookingList()
        {
            JObject returnObj = new JObject();
            returnObj.Add("data", _bookingRepository.GetBookingList());
            return returnObj;
        }


        //public System.Web.Mvc
    }
}
