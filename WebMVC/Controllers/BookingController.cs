using HavaBusiness;
using HavaBusinessObjects;
using HavaBusinessObjects.ControllerRepository;
using HavaBusinessObjects.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Mvc;
using WebMVC.ModelViews;

namespace WebMVC.Controllers
{

    public class BookingController : Controller
    {
        private CommonRepository _commonRepository = new CommonRepository();
        private PartnerRepository _partnerRepository = new PartnerRepository();
        private BookingRepository _bookingRepository = new BookingRepository();
        private ProductRepository _productRepository = new ProductRepository();
        private Models.UserRepository _userRepository = new Models.UserRepository();
        private HavaBusinessObjects.Utility _utility = new HavaBusinessObjects.Utility();


        [HttpGet]
        [AllowAnonymous]
        public JObject AllStatus()
        {
            JObject returnObj = new JObject();
            try
            {
                returnObj.Add("data" , _bookingRepository.BookingStatus());
                return returnObj;

            }
            catch (Exception ex)
            {
                returnObj.Add("error" , "General Error");
                return returnObj;
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public JObject CardTypes()
        {
            JObject returnObj = new JObject();
            try
            {
                returnObj.Add("data" , _commonRepository.GetAllCardTypes());
                return returnObj;

            }
            catch (Exception ex)
            {
                returnObj.Add("error" , "General Error");
                return returnObj;
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public JObject GetProductsChauffer(int partnerId, string pickupDate, string returnDate, string PromotionCode)
        {
            JObject returnObj = new JObject();
            try
            {
                var prodPrices = _partnerRepository.GetPartnerProductsChuffer(partnerId);
                var promotion = _bookingRepository.GetPromotionCode(PromotionCode, partnerId);

                DateTime pickupDateChauffer = Convert.ToDateTime(pickupDate);
                DateTime returnDateChauffer = Convert.ToDateTime(returnDate);

                int dateDiff = ((int)(returnDateChauffer - pickupDateChauffer).TotalDays) + 1;

                if (prodPrices.Count() > 0)
                {
                    JArray returnArr = new JArray();

                    foreach (var item in prodPrices)
                    {
                        JObject itemObj = new JObject();
                        itemObj.Add("Id", item.Id);

                        itemObj.Add("partnerPercentage", item.Percentage);
                        itemObj.Add("productId", item.ProductId);

                        itemObj.Add("description", item.Product.Description);
                        itemObj.Add("code", item.Product.Code);
                        itemObj.Add("name", item.Product.Name);
                        itemObj.Add("isMainProduct", item.Product.IsMainProduct);
                        itemObj.Add("maxPassengers", item.Product.MaxPassengers);
                        itemObj.Add("maxLuggage", item.Product.MaxLuggage);
                        itemObj.Add("imageName", item.Product.ProductImageName);
                        itemObj.Add("imagePath", item.Product.ProductImagePath);

                        JArray featureArr = new JArray();
                        if (item.Product.ProductFeatures.Count() > 0)
                        {

                            foreach (var feature in item.Product.ProductFeatures)
                            {
                                JObject productFeaturesObj = new JObject();
                                productFeaturesObj.Add("id", feature.Id);
                                productFeaturesObj.Add("description", feature.Description);

                                featureArr.Add(productFeaturesObj);
                            }

                        }

                        itemObj.Add("productFeatures", featureArr);

                        //itemObj.Add("Rate", item.Rate);
                        itemObj.Add("HavaPrice", (item.HavaPrice * dateDiff));
                        itemObj.Add("PartnerSellingPrice", (item.PartnerSellingPrice * dateDiff));

                        if (promotion != null)
                        {
                            itemObj.Add("MarketPrice", ((item.MarketPrice * ((100 - promotion.PromotionDiscount.AmountOrPercentage) / 100)) * dateDiff));
                        }
                        else
                        {
                            itemObj.Add("MarketPrice", (item.MarketPrice * dateDiff));
                        }

                        itemObj.Add("IsMarkUp", item.IsMarkUp);
                        itemObj.Add("Markup", item.Markup);
                        itemObj.Add("Percentage", item.Percentage);

                        itemObj.Add("promotionCode", promotion == null ? string.Empty : promotion.Code);
                        itemObj.Add("promotionAmount", promotion == null ? 0 : promotion.PromotionDiscount.AmountOrPercentage);

                        JObject partnerObj = new JObject();
                        if (item.Partner != null)
                        {
                            partnerObj.Add("name", item.Partner.Name);
                            partnerObj.Add("code", item.Partner.Code);
                            partnerObj.Add("address", item.Partner.FullAddress);
                            partnerObj.Add("email", item.Partner.Email);
                            partnerObj.Add("telephoneLand", item.Partner.TelLandLine);
                            partnerObj.Add("telephoneMobile", item.Partner.TelMobile);
                        }

                        itemObj.Add("Partner", partnerObj);
                        
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

        [HttpGet]
        [AllowAnonymous]
        public JObject GetProducts(int partnerId , int locationId , string PromotionCode)
        {
            JObject returnObj = new JObject();
            try
            {
                var prodPrices = _partnerRepository.GetPartnerProducts(partnerId , locationId);
                var promotion = _bookingRepository.GetPromotionCode(PromotionCode , partnerId);

                if (prodPrices.Count() > 0)
                {
                    //JArray albums = JArray.Parse(JsonConvert.SerializeObject(prodPrices).Replace("\\", "")) as JArray;
                    ////JObject jalbum = albums[0] as JObject;
                    //returnObj.Add("data", albums);

                    JArray returnArr = new JArray();

                    foreach (var item in prodPrices)
                    {
                        JObject itemObj = new JObject();
                        itemObj.Add("Id" , item.Id);

                        itemObj.Add("partnerPercentage" , item.Percentage);
                        itemObj.Add("productId" , item.ProductId);

                        itemObj.Add("description" , item.Product.Description);
                        itemObj.Add("code" , item.Product.Code);
                        itemObj.Add("name" , item.Product.Name);
                        itemObj.Add("isMainProduct" , item.Product.IsMainProduct);
                        itemObj.Add("maxPassengers" , item.Product.MaxPassengers);
                        itemObj.Add("maxLuggage" , item.Product.MaxLuggage);
                        itemObj.Add("imageName" , item.Product.ProductImageName);
                        itemObj.Add("imagePath" , item.Product.ProductImagePath);

                        JArray featureArr = new JArray();
                        if (item.Product.ProductFeatures.Count() > 0)
                        {

                            foreach (var feature in item.Product.ProductFeatures)
                            {
                                JObject productFeaturesObj = new JObject();
                                productFeaturesObj.Add("id" , feature.Id);
                                productFeaturesObj.Add("description" , feature.Description);

                                featureArr.Add(productFeaturesObj);
                            }

                        }

                        itemObj.Add("productFeatures" , featureArr);
                        
                        itemObj.Add("HavaPrice" , item.HavaPrice);
                        itemObj.Add("PartnerSellingPrice", item.PartnerSellingPrice);

                        if (promotion != null)
                        {
                            itemObj.Add("MarketPrice", (item.MarketPrice * ((100 - promotion.PromotionDiscount.AmountOrPercentage) / 100)));
                        }
                        else
                        {
                            itemObj.Add("MarketPrice", item.MarketPrice);
                        }

                        itemObj.Add("IsMarkUp" , item.IsMarkUp);
                        itemObj.Add("Markup" , item.Markup);
                        itemObj.Add("Percentage" , item.Percentage);

                        itemObj.Add("promotionCode" , promotion == null ? string.Empty : promotion.Code);
                        itemObj.Add("promotionAmount" , promotion == null ? 0 : promotion.PromotionDiscount.AmountOrPercentage);

                        JObject partnerObj = new JObject();
                        if (item.Partner != null)
                        {
                            partnerObj.Add("name" , item.Partner.Name);
                            partnerObj.Add("code" , item.Partner.Code);
                            partnerObj.Add("address" , item.Partner.FullAddress);
                            partnerObj.Add("email" , item.Partner.Email);
                            partnerObj.Add("telephoneLand" , item.Partner.TelLandLine);
                            partnerObj.Add("telephoneMobile" , item.Partner.TelMobile);
                        }

                        itemObj.Add("Partner" , partnerObj);

                        JObject locationObj = new JObject();
                        if (item.LocationDetail != null)
                        {
                            locationObj.Add("Id" , item.LocationDetail.Id);
                            locationObj.Add("name" , item.LocationDetail.name);
                            locationObj.Add("PartnerId" , item.LocationDetail.PartnerId);
                            locationObj.Add("IsActive" , item.LocationDetail.IsActive);
                            locationObj.Add("FromLocation" , item.LocationDetail.FromLocation);
                            locationObj.Add("ToLocation" , item.LocationDetail.ToLocation);
                            locationObj.Add("IsAirPortTour" , item.LocationDetail.IsAirPortTour);
                        }

                        itemObj.Add("LocationDetail" , locationObj);

                        returnArr.Add(itemObj);
                    }

                    returnObj.Add("data" , returnArr);
                    return returnObj;
                }
                else
                {
                    return returnObj;
                }

            }
            catch (Exception ex)
            {
                returnObj.Add("error" , ex.Message.ToString());
                return returnObj;
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public JObject GetPartnerSubProducts(int partnerId)
        {
            JObject returnObj = new JObject();
            try
            {
                var prodPrices = _partnerRepository.GetPartnerSubProducts(partnerId);

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

                        itemObj.Add("partnerPercentage", item.Percentage);
                        itemObj.Add("productId", item.ProductId);

                        itemObj.Add("description", item.Product.Description);
                        itemObj.Add("code", item.Product.Code);
                        itemObj.Add("name", item.Product.Name);
                        itemObj.Add("isMainProduct", item.Product.IsMainProduct);
                        itemObj.Add("maxPassengers", item.Product.MaxPassengers);
                        itemObj.Add("maxLuggage", item.Product.MaxLuggage);
                        itemObj.Add("imageName", item.Product.ProductImageName);
                        itemObj.Add("imagePath", item.Product.ProductImagePath);

                        JArray featureArr = new JArray();
                        if (item.Product.ProductFeatures.Count() > 0)
                        {

                            foreach (var feature in item.Product.ProductFeatures)
                            {
                                JObject productFeaturesObj = new JObject();
                                productFeaturesObj.Add("id", feature.Id);
                                productFeaturesObj.Add("description", feature.Description);

                                featureArr.Add(productFeaturesObj);
                            }

                        }

                        itemObj.Add("productFeatures", featureArr);

                        itemObj.Add("HavaPrice", item.HavaPrice);
                        itemObj.Add("PartnerSellingPrice", item.PartnerSellingPrice);
                        itemObj.Add("MarketPrice", item.MarketPrice);
                        itemObj.Add("IsMarkUp", item.IsMarkUp);
                        itemObj.Add("Markup", item.Markup);
                        itemObj.Add("Percentage", item.Percentage);

                        itemObj.Add("promotionCode", string.Empty);
                        itemObj.Add("promotionAmount", 0);
                        itemObj.Add("Quantity" , 0);

                        JObject partnerObj = new JObject();
                        if (item.Partner != null)
                        {
                            partnerObj.Add("name", item.Partner.Name);
                            partnerObj.Add("code", item.Partner.Code);
                            partnerObj.Add("address", item.Partner.FullAddress);
                            partnerObj.Add("email", item.Partner.Email);
                            partnerObj.Add("telephoneLand", item.Partner.TelLandLine);
                            partnerObj.Add("telephoneMobile", item.Partner.TelMobile);
                        }

                        itemObj.Add("Partner", partnerObj);

                        JObject locationObj = new JObject();
                        itemObj.Add("LocationDetail", locationObj);

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

             ModelState.Remove("DropLocation");
            

            foreach (ModelState modelState in ViewData.ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                   // DoSomethingWith(error);
                }
            }

            if (ModelState.IsValid)
            {
                //vm.UserId = 3;
                if (vm.UserId == 0)
                {
                    //vm.UserId = GetAppUser();
                    vm.UserId = _userRepository.GetUserIdByUserName(vm.UserName);
                }

                var booking = _bookingRepository.Insert(vm);
               
                returnObj.Add("data" , this.ReturnBookingJson(booking));
                returnObj.Add("success" , true);
                return returnObj;

            }
            returnObj.Add("error" , "Please provide mandatory fields");
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
                returnObj.Add("data" , this.ReturnBookingJson(booking));
                return returnObj;

            }
            catch (Exception ex)
            {
                returnObj.Add("error" , "General Error");
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
        public ActionResult Edit()
        {
            return View();
        }

        public JObject GetBookingList()
        {
            JObject returnObj = new JObject();
            returnObj.Add("data" , _bookingRepository.GetBookingList());
            return returnObj;
        }

        public JObject GetPromotion(string promotionCode , int partnerId)
        {
            var promotion = _bookingRepository.GetPromotionCode(promotionCode , partnerId);

            JObject promotionObj = new JObject();

            if (promotion != null)
            {
                promotionObj.Add("promotionId" , promotion.Id);
                promotionObj.Add("promotionCode" , promotion.Code);
                promotionObj.Add("discount" , promotion.PromotionDiscount.AmountOrPercentage);

            }
            return promotionObj;
        }

        [HttpPut]
        [AllowAnonymous]
        public JObject Update(BookingViewModel vm)
        {
            JObject returnObj = new JObject();

            if (ModelState.IsValid)
            {
                var booking = _bookingRepository.Update(vm);

                returnObj.Add("data" , this.ReturnBookingJson(booking));
                return returnObj;

            }
            returnObj.Add("error" , "Please provide mandatory fields");
            return returnObj;
        }

        private JObject ReturnBookingJson(BookingViewModel vm)
        {
            JObject returnObj = new JObject();

            returnObj.Add("Id" , vm.Id);
            returnObj.Add("PickupDate" , vm.PickupDate == null ? "" : vm.PickupDate.Value.ToString("dd/MM/yyyy"));
            returnObj.Add("PickupTime" , vm.PickupTime);
            returnObj.Add("PickupLocation" , vm.PickupLocation);
            returnObj.Add("ReturnTime" , vm.ReturnTime);
            returnObj.Add("ReturnPickupLocation" , vm.ReturnPickupLocation);
            returnObj.Add("DropLocation" , vm.DropLocation == null ? "" : vm.DropLocation.name);
            returnObj.Add("ReturnDate" , vm.ReturnDate == null ? "" : vm.ReturnDate.Value.ToString("dd/MM/yyyy"));
            returnObj.Add("RefNo" , vm.RefNo);
            returnObj.Add("NumberOfDays" , vm.NumberOfDays);
            returnObj.Add("TSPId" , vm.TSPId);
            returnObj.Add("PaymentTypeId" , vm.PaymentTypeId);
            returnObj.Add("PromotionId" , vm.PromotionId);
            returnObj.Add("HasPromotions" , vm.HasPromotions);
            returnObj.Add("UserConfirmed" , vm.UserConfirmed);
            returnObj.Add("UserId" , vm.UserId);
            returnObj.Add("IsReturn" , vm.IsReturn);
            returnObj.Add("IsAirportTransfer" , vm.IsAirportTransfer);
            returnObj.Add("IsChauffer", vm.IsAirportTransfer == true ? false : true);
            returnObj.Add("CreatedDate", vm.CreatedDate.ToString("dd/MM/yyyy"));
            returnObj.Add("TotalCost", vm.TotalCost);

            JObject bookingTypeObj = new JObject();
            bookingTypeObj.Add("Id" , vm.BookingType.Id);
            bookingTypeObj.Add("Type" , vm.BookingType.type);
            bookingTypeObj.Add("IsActive" , vm.BookingType.IsActive);

            returnObj.Add("BookingType " , bookingTypeObj);

            JObject partnerObj = new JObject();
            if (vm.Partner != null)
            {
                partnerObj.Add("name" , vm.Partner.name);
                partnerObj.Add("code" , vm.Partner.code);
                partnerObj.Add("address" , vm.Partner.address);
                partnerObj.Add("email" , vm.Partner.email);
                partnerObj.Add("telephoneLand" , vm.Partner.telephoneLand);
                partnerObj.Add("telephoneMobile" , vm.Partner.telephoneMobile);
            }
            returnObj.Add("Partner " , partnerObj);

            JObject bookingStatusObj = new JObject();
            bookingStatusObj.Add("Id" , vm.BookingStatu.Id);
            bookingStatusObj.Add("Name" , vm.BookingStatu.Name);
            bookingStatusObj.Add("IsActive" , vm.BookingStatu.IsActive);

            returnObj.Add("BookingStatu" , bookingStatusObj);

            JArray productArr = new JArray();
            foreach (var product in vm.BookingProducts)
            {
                JObject bookingProductObj = new JObject();
                bookingProductObj.Add("Id" , product.Id);
                bookingProductObj.Add("ProductId" , product.ProductId);
                bookingProductObj.Add("Price" , product.PartnerSellingPrice);
                bookingProductObj.Add("Description", product.Product.description);

                var path = ConfigurationManager.AppSettings["SiteUrl"].ToString() + product.Product.ProductImagePath;
                bookingProductObj.Add("ProductImagePath" , path);

                productArr.Add(bookingProductObj);
            }
            returnObj.Add("BookingProducts" , productArr);

            JArray subProductArr = new JArray();
            foreach (var product in vm.BookingSubProducts)
            {
                JObject bookingProductObj = new JObject();
                bookingProductObj.Add("Id", product.Id);
                bookingProductObj.Add("ProductId", product.ProductId);
                bookingProductObj.Add("MarketPrice", product.MarketPrice);
                bookingProductObj.Add("Quantity", product.Quantity);
                bookingProductObj.Add("Price", product.Price);
                bookingProductObj.Add("Description", product.Product.name);

                subProductArr.Add(bookingProductObj);
            }
            returnObj.Add("BookingSubProducts", subProductArr);

            JArray paymentsArr = new JArray();
            foreach (var payment in vm.BookingPayments)
            {
                JObject paymentsObj = new JObject();
                paymentsObj.Add("Id" , payment.Id);
                paymentsObj.Add("CardHolderName" , payment.CardHolderName);
                paymentsObj.Add("ExpireDate" , payment.ExpireDate);
                paymentsObj.Add("CardNo" , payment.CardNo);
                paymentsObj.Add("CardType" , payment.CardTypeVM.Name);
                paymentsObj.Add("CVV" , payment.CVV);

                paymentsArr.Add(paymentsObj);
            }

            returnObj.Add("BookingPayments" , paymentsArr);

            JArray optionArr = new JArray();
            foreach (var option in vm.BookingOptions)
            {
                JObject optionObj = new JObject();

                optionObj.Add("Id" , option.Id);
                optionObj.Add("BookingId" , option.BookingId);
                optionObj.Add("FlightNo" , option.FlightNo);
                optionObj.Add("FlyerProgramId" , option.FlyerProgramId);
                optionObj.Add("FlyerReffNo" , option.FlyerReffNo);
                optionObj.Add("PickupSign" , option.PickupSign);
                optionObj.Add("PickupSignReffNo" , option.PickupSignReffNo);
                optionObj.Add("NoteToDriver" , option.NoteToDriver);
                optionObj.Add("CreatedDate" , option.CreatedDate);
                optionObj.Add("PickupAddress" , option.PickupAddress);

                optionArr.Add(optionObj);
            }

            returnObj.Add("BookingOptions" , optionArr);

            JArray passengersArr = new JArray();
            foreach (var option in vm.BookingPassenger)
            {
                JObject optionObj = new JObject();

                optionObj.Add("FirstName", option.FirstName);
                optionObj.Add("LastName", option.LastName);
                optionObj.Add("Mobile", option.Mobile);
                optionObj.Add("Email", option.Email);
                //optionObj.Add("Country", option.Country.Name);

                passengersArr.Add(optionObj);
            }

            returnObj.Add("BookingPassenger", passengersArr);



            return returnObj;
        }

        [HttpGet]
        [AllowAnonymous]
        public JObject GetAllCountry()
        {
            JObject returnObj = new JObject();
            try
            {
                returnObj.Add("data" , _commonRepository.GetAllCountry());
                return returnObj;

            }
            catch (Exception ex)
            {
                returnObj.Add("error" , "General Error");
                return returnObj;
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public JObject GetAllBookingStatus()
        {
            JObject returnObj = new JObject();
            try
            {
                returnObj.Add("data", _commonRepository.GetAllBookingStatus());
                return returnObj;

            }
            catch (Exception ex)
            {
                returnObj.Add("error", "General Error");
                return returnObj;
            }
        }

        private int GetAppUser()
        {
            return _userRepository.GetUserIdByUserName(User.Identity.Name);
        }
        public ActionResult BookingHistory()
        {
            return View();
        }
        [HttpGet]
        public string GetAppUserName()
        {
            return _userRepository.GetNameByUserName(User.Identity.Name);
        }

        [HttpGet]
        public JObject GetMainProducts()
        {
            var products = _productRepository.GetMainProducts();
            JObject returnObj = new JObject();
            returnObj.Add("data", this.ProductsJsonObj(products));
            return returnObj;
        }

        [HttpGet]
        public JObject GetSubProducts()
        {
            var products = _productRepository.GetSubProducts();
            JObject returnObj = new JObject();
            returnObj.Add("data", this.ProductsJsonObj(products));
            return returnObj;
        }

        [HttpGet]
        public FileResult ExportBooking(int? partnerId, int? bookingStatus)
        {
            var booking = _bookingRepository.ExportBooking(partnerId, bookingStatus);
            var additionals = _productRepository.GetSubProducts();

            StringBuilder str = new StringBuilder();

            str.Append("<table border=`" + "1px" + "`b>");

            str.Append("<tr>");

            str.Append("<td><b><font face=Arial Narrow size=3>Reff No:</font></b></td>");
            str.Append("<td><b><font face=Arial Narrow size=3>Booking Type</font></b></td>");
            str.Append("<td><b><font face=Arial Narrow size=3>Current Status</font></b></td>");
            str.Append("<td><b><font face=Arial Narrow size=3>Pickup Date</font></b></td>");
            str.Append("<td><b><font face=Arial Narrow size=3>Pickup Time</font></b></td>");
            str.Append("<td><b><font face=Arial Narrow size=3>Pickup Location</font></b></td>");
            str.Append("<td><b><font face=Arial Narrow size=3>Return Date</font></b></td>");
            str.Append("<td><b><font face=Arial Narrow size=3>Return Time</font></b></td>");
            str.Append("<td><b><font face=Arial Narrow size=3>Drop Location</font></b></td>");
            str.Append("<td><b><font face=Arial Narrow size=3>Vehicle</font></b></td>");
            str.Append("<td><b><font face=Arial Narrow size=3>Product Price</font></b></td>");
            str.Append("<td><b><font face=Arial Narrow size=3>Total Amount</font></b></td>");
            str.Append("<td><b><font face=Arial Narrow size=3>Flight No</font></b></td>");
            str.Append("<td><b><font face=Arial Narrow size=3>Note</font></b></td>");
            str.Append("<td><b><font face=Arial Narrow size=3>Card Holder</font></b></td>");
            str.Append("<td><b><font face=Arial Narrow size=3>Card No</font></b></td>");
            str.Append("<td><b><font face=Arial Narrow size=3>Exp. Date</font></b></td>");
            str.Append("<td><b><font face=Arial Narrow size=3>CVV</font></b></td>");


            str.Append("<td><b><font face=Arial Narrow size=3>Passenger Name</font></b></td>");
            str.Append("<td><b><font face=Arial Narrow size=3>Mobile</font></b></td>");
            str.Append("<td><b><font face=Arial Narrow size=3>Email</font></b></td>");
            str.Append("<td><b><font face=Arial Narrow size=3>Country</font></b></td>");

            //foreach (var item in additionals)
            //{
            //    str.Append("<td><b><font face=Arial Narrow size=3>" + item.Name + "</font></b></td>");
            //    str.Append("<td><b><font face=Arial Narrow size=3>Qty.</font></b></td>");
            //    str.Append("<td><b><font face=Arial Narrow size=3>Price</font></b></td>");
            //}

            str.Append("</tr>");

            foreach (var book in booking)
            {

                str.Append("<tr>");

                string pickupDate = book.PickupDate == null ? string.Empty : book.PickupDate.Value.ToString("dd/MM/yyyy");
                string pickupTime = book.PickupTime == null ? string.Empty : book.PickupTime.Value.ToString();
                string pickupLocation = book.PickupLocation == null ? string.Empty : book.PickupLocation;
                string returnDate = !book.ReturnDate.HasValue ? string.Empty : book.ReturnDate.Value.ToString("dd/MM/yyyy");
                string returnTime = !book.ReturnTime.HasValue ? string.Empty : book.ReturnTime.Value.ToString();
                string dropLocation = book.DropLocation == null ? string.Empty : book.DropLocation.ToLocation;
                string bookingProduct = book.BookingProducts.Count() > 0 ? book.BookingProducts[0].Product.name.ToString() : string.Empty;
                string productPrice = book.BookingProducts.Count() > 0 ? book.BookingProducts[0].MarketPrice.Value.ToString("0.00") : string.Empty;
                string totaCost = book.TotalCost.ToString("0.00");


                str.Append("<td><b><font face=Arial Narrow size=3>" + book.BookingNo.ToString() + "</font></b></td>");
                str.Append("<td><b><font face=Arial Narrow size=3>" + book.BookingType.type.ToString() + "</font></b></td>");
                str.Append("<td><b><font face=Arial Narrow size=3>" + book.BookingStatu.Name.ToString() + "</font></b></td>");
                str.Append("<td><b><font face=Arial Narrow size=3>" + pickupDate + "</font></b></td>");
                str.Append("<td><b><font face=Arial Narrow size=3>" + pickupTime + "</font></b></td>");
                str.Append("<td><b><font face=Arial Narrow size=3>" + pickupLocation + "</font></b></td>");
                str.Append("<td><b><font face=Arial Narrow size=3>" + returnDate + "</font></b></td>");
                str.Append("<td><b><font face=Arial Narrow size=3>" + returnTime + "</font></b></td>");
                str.Append("<td><b><font face=Arial Narrow size=3>" + dropLocation + "</font></b></td>");
                str.Append("<td><b><font face=Arial Narrow size=3>" + bookingProduct + "</font></b></td>");
                str.Append("<td><b><font face=Arial Narrow size=3>" + productPrice + "</font></b></td>");
                str.Append("<td><b><font face=Arial Narrow size=3>" + totaCost + "</font></b></td>");

                if (book.BookingOptions.Count() > 0)
                {
                    str.Append("<td><b><font face=Arial Narrow size=3>" + (book.BookingOptions[0].FlightNo == null ? string.Empty : book.BookingOptions[0].FlightNo) + "</font></b></td>");
                    str.Append("<td><b><font face=Arial Narrow size=3>" + (book.BookingOptions[0].NoteToDriver == null ? string.Empty : book.BookingOptions[0].NoteToDriver) + "</font></b></td>");
                }
                else
                {
                    str.Append("<td><b><font face=Arial Narrow size=3>" + string.Empty + "</font></b></td>");
                    str.Append("<td><b><font face=Arial Narrow size=3>" + string.Empty + "</font></b></td>");
                }

                if (book.BookingPayments.Count() > 0)
                {
                    str.Append("<td><b><font face=Arial Narrow size=3>" + book.BookingPayments[0].CardHolderName.ToString() + "</font></b></td>");
                    str.Append("<td><b><font face=Arial Narrow size=3>" + book.BookingPayments[0].CardNo.ToString() + "</font></b></td>");
                    str.Append("<td><b><font face=Arial Narrow size=3>" + book.BookingPayments[0].ExpireDate.ToString() + "</font></b></td>");
                    str.Append("<td><b><font face=Arial Narrow size=3>" + book.BookingPayments[0].CVV.ToString() + "</font></b></td>");
                }
                else
                {
                    str.Append("<td><b><font face=Arial Narrow size=3>" + string.Empty + "</font></b></td>");
                    str.Append("<td><b><font face=Arial Narrow size=3>" + string.Empty + "</font></b></td>");
                    str.Append("<td><b><font face=Arial Narrow size=3>" + string.Empty + "</font></b></td>");
                    str.Append("<td><b><font face=Arial Narrow size=3>" + string.Empty + "</font></b></td>");
                }


                if (book.BookingPassenger.Count() > 0)
                {
                    str.Append("<td><b><font face=Arial Narrow size=3>" + (book.BookingPassenger[0].FirstName + " " + book.BookingPassenger[0].LastName) + "</font></b></td>");
                    str.Append("<td><b><font face=Arial Narrow size=3>" + book.BookingPassenger[0].Mobile + "</font></b></td>");
                    str.Append("<td><b><font face=Arial Narrow size=3>" + book.BookingPassenger[0].Email + "</font></b></td>");
                    str.Append("<td><b><font face=Arial Narrow size=3>" + book.BookingPassenger[0].Country.Name + "</font></b></td>");
                }
                else
                {
                    str.Append("<td><b><font face=Arial Narrow size=3>" + string.Empty + "</font></b></td>");
                    str.Append("<td><b><font face=Arial Narrow size=3>" + string.Empty + "</font></b></td>");
                    str.Append("<td><b><font face=Arial Narrow size=3>" + string.Empty + "</font></b></td>");
                    str.Append("<td><b><font face=Arial Narrow size=3>" + string.Empty + "</font></b></td>");
                }




                //str.Append("<td><b><font face=Arial Narrow size=3>" + string.Empty + "</font></b></td>");
                //str.Append("<td><b><font face=Arial Narrow size=3>" + string.Empty + "</font></b></td>");
                //str.Append("<td><b><font face=Arial Narrow size=3>" + string.Empty + "</font></b></td>");
                //str.Append("<td><b><font face=Arial Narrow size=3>" + string.Empty + "</font></b></td>");
                //str.Append("<td><b><font face=Arial Narrow size=3>" + string.Empty + "</font></b></td>");
                //str.Append("<td><b><font face=Arial Narrow size=3>" + string.Empty + "</font></b></td>");
                //str.Append("<td><b><font face=Arial Narrow size=3>" + string.Empty + "</font></b></td>");
                //str.Append("<td><b><font face=Arial Narrow size=3>" + string.Empty + "</font></b></td>");
                //str.Append("<td><b><font face=Arial Narrow size=3>" + string.Empty + "</font></b></td>");
                //str.Append("<td><b><font face=Arial Narrow size=3>" + string.Empty + "</font></b></td>");
                //str.Append("<td><b><font face=Arial Narrow size=3>" + string.Empty + "</font></b></td>");
                //str.Append("<td><b><font face=Arial Narrow size=3>" + string.Empty + "</font></b></td>");
                //str.Append("<td><b><font face=Arial Narrow size=3>" + string.Empty + "</font></b></td>");
                //str.Append("<td><b><font face=Arial Narrow size=3>" + string.Empty + "</font></b></td>");
                //str.Append("<td><b><font face=Arial Narrow size=3>" + string.Empty + "</font></b></td>");
                //str.Append("<td><b><font face=Arial Narrow size=3>" + string.Empty + "</font></b></td>");
                //str.Append("<td><b><font face=Arial Narrow size=3>" + string.Empty + "</font></b></td>");
                //str.Append("<td><b><font face=Arial Narrow size=3>" + string.Empty + "</font></b></td>");
                //str.Append("<td><b><font face=Arial Narrow size=3>" + string.Empty + "</font></b></td>");
                //str.Append("<td><b><font face=Arial Narrow size=3>" + string.Empty + "</font></b></td>");
                //str.Append("<td><b><font face=Arial Narrow size=3>" + string.Empty + "</font></b></td>");
                //str.Append("<td><b><font face=Arial Narrow size=3>" + string.Empty + "</font></b></td>");

                //foreach (var item in additionals)
                //{
                //    var service = book.BookingSubProducts.Where(a => a.ProductId == item.Id).FirstOrDefault();

                //    str.Append("<td><b><font face=Arial Narrow size=3>" + item.Name + "</font></b></td>");
                //    if (service == null)
                //    {
                //        str.Append("<td><b><font face=Arial Narrow size=3>" + "" + "</font></b></td>");
                //        str.Append("<td><b><font face=Arial Narrow size=3>" + "" + "</font></b></td>");
                //    }
                //    else
                //    {
                //        str.Append("<td><b><font face=Arial Narrow size=3>" + service == null ? "" : service.Quantity + "</font></b></td>");
                //        str.Append("<td><b><font face=Arial Narrow size=3>" + service == null ? "" : (service.Price == null ? string.Empty : service.Price.Value.ToString("0.00")) + "</font></b></td>");
                //    }
                //}

                str.Append("</tr>");

            }



            str.Append("</table>");


            HttpContext.Response.AddHeader("content-disposition", "attachment; filename=Booking_Information" + DateTime.Now.Year.ToString() + ".xls");
            this.Response.ContentType = "application/vnd.ms-excel";
            byte[] temp = System.Text.Encoding.UTF8.GetBytes(str.ToString());

            return File(temp, "application/vnd.ms-excel");
        }

        private JArray ProductsJsonObj(List<Product> productsList)
        {
            JArray products = new JArray();

            foreach (var item in productsList)
            {
                JObject product = new JObject();
                product.Add("Id", item.Id);
                product.Add("ProductId" , item.Id);
                product.Add("Name", item.Name);
                product.Add("Description", item.Description);
                product.Add("IsMainProduct", item.IsMainProduct);
                product.Add("ProductImagePath", item.ProductImagePath);

                products.Add(product);

            }

            return products;
        }


    }
}
