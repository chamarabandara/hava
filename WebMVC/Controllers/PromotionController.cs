using HavaBusinessObjects.ControllerRepository;
using HavaBusinessObjects.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebMVC.Controllers
{
    public class PromotionController : Controller
    {

        PromotionRepository _promotionRepository = new PromotionRepository();

        [HttpGet]
        public JObject GetPromotionById(int id)
        {
            var promotion = _promotionRepository.GetPromotionById(id);

            JObject promotionObj = new JObject();
            promotionObj.Add("id", promotion.Id);
            promotionObj.Add("name", promotion.Name);
            promotionObj.Add("code", promotion.Code);
            promotionObj.Add("partnerId", promotion.Partner.id);
            promotionObj.Add("partnerName", promotion.Partner.name);
            promotionObj.Add("discountTypeId", promotion.PromotionDiscount.PromotionDiscountType.Id);
            promotionObj.Add("discountTypeName", promotion.PromotionDiscount.PromotionDiscountType.Name);
            promotionObj.Add("discountAmount", promotion.PromotionDiscount.AmountOrPercentage);

            JObject returnObj = new JObject();

            returnObj.Add("data", promotionObj);
            return returnObj;
        }

        [HttpGet]
        public JObject GetPromotions()
        {
            var promotions = _promotionRepository.GetPromotions();

            JArray returnArr = new JArray();

            foreach (var promotion in promotions)
            {
                JObject promotionObj = new JObject();
                promotionObj.Add("id", promotion.Id);
                promotionObj.Add("name", promotion.Name);
                promotionObj.Add("code", promotion.Code);
                promotionObj.Add("partnerId", promotion.Partner.id);
                promotionObj.Add("partnerName", promotion.Partner.name);
                promotionObj.Add("discountTypeId", promotion.PromotionDiscount.PromotionDiscountType.Id);
                promotionObj.Add("discountTypeName", promotion.PromotionDiscount.PromotionDiscountType.Name);
                promotionObj.Add("discountAmount", promotion.PromotionDiscount.AmountOrPercentage);

                returnArr.Add(promotionObj);
            }
            
            JObject returnObj = new JObject();

            returnObj.Add("data", returnArr);
            return returnObj;
        }

        [HttpGet]
        public JObject GetDiscounts()
        {
            var discounts = _promotionRepository.GetAllDiscount();

            JArray returnArr = new JArray();

            foreach (var discount in discounts)
            {
                JObject promotionObj = new JObject();
                promotionObj.Add("id", discount.Id);
                promotionObj.Add("amountOrPercentage", discount.AmountOrPercentage);
                promotionObj.Add("discountTypeId", discount.PromotionDiscountType.Id);
                promotionObj.Add("discountTypeName", discount.PromotionDiscountType.Name);

                returnArr.Add(promotionObj);
            }

            JObject returnObj = new JObject();

            returnObj.Add("data", returnArr);
            return returnObj;
        }

        [HttpPost]
        [AllowAnonymous]
        public JObject InsertPromotion(PromotionViewModel promotionVM)
        {
            var promotion = _promotionRepository.InsertPromotion(promotionVM);

            JObject promotionObj = new JObject();
            promotionObj.Add("id", promotion.Id);
            promotionObj.Add("name", promotion.Name);
            promotionObj.Add("code", promotion.Code);
            promotionObj.Add("partnerId", promotion.Partner.id);
            promotionObj.Add("partnerName", promotion.Partner.name);
            promotionObj.Add("discountTypeId", promotion.PromotionDiscount.PromotionDiscountType.Id);
            promotionObj.Add("discountTypeName", promotion.PromotionDiscount.PromotionDiscountType.Name);
            promotionObj.Add("discountAmount", promotion.PromotionDiscount.AmountOrPercentage);

            JObject returnObj = new JObject();

            returnObj.Add("data", promotionObj);
            return returnObj;
        }
    }
}