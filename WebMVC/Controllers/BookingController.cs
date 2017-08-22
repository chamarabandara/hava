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
                    //JArray albums = JArray.Parse(JsonConvert.SerializeObject(prodPrices).Replace("\\", "")) as JArray;
                    ////JObject jalbum = albums[0] as JObject;
                    //returnObj.Add("data", albums);

                    JArray returnArr = new JArray();

                    foreach (var item in prodPrices)
                    {
                        JObject itemObj = new JObject();
                        itemObj.Add("Id", item.Id);
                        itemObj.Add("Rate", item.Rate);
                        itemObj.Add("HavaPrice", item.HavaPrice);
                        itemObj.Add("MarketPrice", item.MarketPrice);
                        itemObj.Add("PartnerSellingPrice", item.PartnerSellingPrice);
                        itemObj.Add("IsMarkUp", item.IsMarkUp);
                        itemObj.Add("Markup", item.Markup);
                        itemObj.Add("Percentage", item.Percentage);
                        itemObj.Add("AirportRate", item.AirportRate);
                        itemObj.Add("HavaPriceReturn", item.HavaPriceReturn);
                        itemObj.Add("MarketPriceReturn", item.MarketPriceReturn);
                        itemObj.Add("PartnerSellingPriceReturn", item.PartnerSellingPriceReturn);
                        itemObj.Add("AdditionaDayRate", item.AdditionaDayRate);
                        itemObj.Add("AdditionaHourRate", item.AdditionaHourRate);
                        itemObj.Add("ChufferDailyRate", item.ChufferDailyRate);
                        itemObj.Add("ChufferKMRate", item.ChufferKMRate);
                        itemObj.Add("ChildSeatRate", item.ChildSeatRate);

                        JObject partnerObj = new JObject();
                        if (item.Partner != null)
                        {
                            partnerObj.Add("name", item.Partner.name);
                            partnerObj.Add("code", item.Partner.code);
                            partnerObj.Add("address", item.Partner.address);
                            partnerObj.Add("email", item.Partner.email);
                            partnerObj.Add("telephoneLand", item.Partner.telephoneLand);
                            partnerObj.Add("telephoneMobile", item.Partner.telephoneMobile);
                        }

                        itemObj.Add("Partner", partnerObj);

                        JObject locationObj = new JObject();
                        if (item.LocationDetail != null)
                        {
                            locationObj.Add("Id", item.LocationDetail.Id);
                            locationObj.Add("name", item.LocationDetail.name);
                            locationObj.Add("PartnerId", item.LocationDetail.PartnerId);
                            locationObj.Add("IsActive", item.LocationDetail.IsActive);
                            locationObj.Add("FromLocation", item.LocationDetail.FromLocation);
                            locationObj.Add("ToLocation", item.LocationDetail.ToLocation);
                        }

                        itemObj.Add("LocationDetail", locationObj);

                        JObject partnerProdcutObj = new JObject();
                        if (item.PartnerProduct != null)
                        {
                            partnerProdcutObj.Add("partnerPercentage", item.PartnerProduct.partnerPercentage);
                            partnerProdcutObj.Add("partnerMarkup", item.PartnerProduct.partnerMarkup);
                            partnerProdcutObj.Add("isMarkup", item.PartnerProduct.isMarkup);
                            partnerProdcutObj.Add("partnerSellingPrice", item.PartnerProduct.partnerSellingPrice);
                            partnerProdcutObj.Add("marketPrice", item.PartnerProduct.marketPrice);
                            partnerProdcutObj.Add("havaPrice", item.PartnerProduct.havaPrice);
                            partnerProdcutObj.Add("productId", item.PartnerProduct.productId);

                            JObject productObj = new JObject();
                            if (item.PartnerProduct.productVM != null)
                            {
                                productObj.Add("description", item.PartnerProduct.productVM.description);
                                productObj.Add("code", item.PartnerProduct.productVM.code);
                                productObj.Add("name", item.PartnerProduct.productVM.name);
                                productObj.Add("isMainProduct", item.PartnerProduct.productVM.isMainProduct);

                                JObject productFeaturesObj = new JObject();
                                if (item.PartnerProduct.productVM.productFeatures != null)
                                {
                                    productFeaturesObj.Add("id", item.PartnerProduct.productVM.productFeatures.id);
                                    productFeaturesObj.Add("description", item.PartnerProduct.productVM.productFeatures.description);
                                }

                                productObj.Add("productFeatures", productFeaturesObj);

                                JObject productImageObj = new JObject();
                                if (item.PartnerProduct.productVM.productLogoImage != null)
                                {
                                    productImageObj.Add("name", item.PartnerProduct.productVM.productLogoImage.name);
                                    productImageObj.Add("documentPath", item.PartnerProduct.productVM.productLogoImage.documentPath);
                                }

                                productObj.Add("productLogoImage", productImageObj);
                            }
                        }

                        itemObj.Add("PartnerProduct", partnerProdcutObj);

                        returnArr.Add(itemObj);
                    }

                    returnObj.Add("data", returnArr);
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

                returnObj.Add("data", this.ReturnBookingJson(booking));
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
                returnObj.Add("data", this.ReturnBookingJson(booking));
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

        private JObject ReturnBookingJson(BookingViewModel vm)
        {
            JObject returnObj = new JObject();

            returnObj.Add("Id", vm.Id);
            returnObj.Add("PickupDate", vm.PickupDate);
            returnObj.Add("PickupTime", vm.PickupTime);
            returnObj.Add("PickupLocation", vm.PickupLocation);
            returnObj.Add("ReturnTime", vm.ReturnTime);
            returnObj.Add("ReturnPickupLocation", vm.ReturnPickupLocation);
            returnObj.Add("DropLocation", vm.DropLocation);
            returnObj.Add("ReturnDate", vm.ReturnDate);
            returnObj.Add("RefNo", vm.RefNo);
            returnObj.Add("NumberOfDays", vm.NumberOfDays);
            returnObj.Add("TSPId", vm.TSPId);
            returnObj.Add("PaymentTypeId", vm.PaymentTypeId);
            returnObj.Add("PromotionId", vm.PromotionId);
            returnObj.Add("HasPromotions", vm.HasPromotions);
            returnObj.Add("UserConfirmed", vm.UserConfirmed);
            returnObj.Add("UserId", vm.UserId);
            returnObj.Add("IsReturn", vm.IsReturn);
            returnObj.Add("IsAirportTransfer", vm.IsAirportTransfer);

            JObject bookingTypeObj = new JObject();
            bookingTypeObj.Add("Id", vm.BookingType.Id);
            bookingTypeObj.Add("Type", vm.BookingType.type);
            bookingTypeObj.Add("IsActive", vm.BookingType.IsActive);

            returnObj.Add("BookingType ", bookingTypeObj);

            JObject partnerObj = new JObject();
            if (vm.Partner != null)
            {
                partnerObj.Add("name", vm.Partner.name);
                partnerObj.Add("code", vm.Partner.code);
                partnerObj.Add("address", vm.Partner.address);
                partnerObj.Add("email", vm.Partner.email);
                partnerObj.Add("telephoneLand", vm.Partner.telephoneLand);
                partnerObj.Add("telephoneMobile", vm.Partner.telephoneMobile);
            }
            returnObj.Add("Partner ", partnerObj);

            JObject bookingStatusObj = new JObject();
            bookingStatusObj.Add("Id", vm.BookingStatu.Id);
            bookingStatusObj.Add("Name", vm.BookingStatu.Name);
            bookingStatusObj.Add("IsActive", vm.BookingStatu.IsActive);

            returnObj.Add("BookingStatu", bookingStatusObj);

            JArray productArr = new JArray();
            foreach (var product in vm.BookingProducts)
            {
                JObject bookingProductObj = new JObject();
                bookingProductObj.Add("Id", product.Id);
                bookingProductObj.Add("ProductId", product.ProductId);
                bookingProductObj.Add("Price", product.Price);
                bookingProductObj.Add("IsAirPortTour", product.IsAirPortTour);
                bookingProductObj.Add("AdditionalDays", product.AdditionalDays);
                bookingProductObj.Add("AdditionalHours", product.AdditionalHours);
                bookingProductObj.Add("AdditionalChufferDate", product.AdditionalChufferDate);
                bookingProductObj.Add("AdditionalChufferHours", product.AdditionalChufferHours);
                bookingProductObj.Add("NoOfChildSeats", product.NoOfChildSeats);
                bookingProductObj.Add("ChildSeatDays", product.ChildSeatDays);

                productArr.Add(bookingProductObj);
            }
            returnObj.Add("BookingProducts", productArr);

            JArray paymentsArr = new JArray();
            foreach (var payment in vm.BookingPayments)
            {
                JObject paymentsObj = new JObject();
                paymentsObj.Add("Id", payment.Id);
                paymentsObj.Add("CardHolderName", payment.CardHolderName);
                paymentsObj.Add("ExpireDate", payment.ExpireDate);
                paymentsObj.Add("CardNo", payment.CardNo);

                paymentsArr.Add(paymentsObj);
            }

            returnObj.Add("BookingPayments", paymentsArr);

            JArray optionArr = new JArray();
            foreach (var option in vm.BookingOptions)
            {
                JObject optionObj = new JObject();

                optionObj.Add("Id", option.Id);
                optionObj.Add("BookingId", option.BookingId);
                optionObj.Add("FlightNo", option.FlightNo);
                optionObj.Add("FlyerProgramId", option.FlyerProgramId);
                optionObj.Add("FlyerReffNo", option.FlyerReffNo);
                optionObj.Add("PickupSign", option.PickupSign);
                optionObj.Add("PickupSignReffNo", option.PickupSignReffNo);
                optionObj.Add("NoteToDriver", option.PickupSignReffNo);
                optionObj.Add("CreatedDate", option.CreatedDate);

                optionArr.Add(optionObj);
            }

            returnObj.Add("BookingOptions", optionArr);

            return returnObj;
        }


        //public System.Web.Mvc
    }
}
