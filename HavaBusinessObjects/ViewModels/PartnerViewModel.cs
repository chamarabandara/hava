    using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HavaBusinessObjects.ViewModels
{
    public class PartnerViewModel
    {
        public string name { get; set; }
        public string code { get; set; }
        public string address { get; set; }
        public string email { get; set; }
        public string telephoneLand { get; set; }
        public string telephoneMobile { get; set; }
        public List<RepresentativeViewModel> representativeData { get; set; }
        public List<PartnerProductViewModel> productGridData { get; set; }

    }

    public class RepresentativeViewModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string teleNo { get; set; }
        public string mobileNo { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string status { get; set; }
    }

    public class PartnerProductViewModel
    {
        public string partnerPercentage { get; set; }
        public string partnerMarkup { get; set; }
        public bool isMarkup { get; set; }
        public string partnerSellingPrice { get; set; }
        public string marketPrice { get; set; }
        public string havaPrice { get; set; }
        public int productId { get; set; }
    }


}