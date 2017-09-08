using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HavaBusinessObjects.ViewModels
{
    public class PromotionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool? IsAvtive { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public PromotionDiscountViewModel PromotionDiscount { get; set; }
        public PartnerViewModel Partner { get; set; }
    }

    public class PromotionDiscountViewModel
    {
        public int Id { get; set; }
        public PromotionDiscountTypeViewModel PromotionDiscountType { get; set; }
        public decimal? AmountOrPercentage { get; set; }
    }


    public class PromotionDiscountTypeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }
    }
}