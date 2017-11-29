using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HavaBusinessObjects.ViewModels
{
    public class PartnerObjViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string address { get; set; }
        public string email { get; set; }
        public string telephoneLand { get; set; }
        public string telephoneMobile { get; set; }
        public List<RepViewModel> representativeGridData { get; set; }
        //public List<PartnerProductViewModel> mainProductDetails { get; set; }
        //public List<PartnerProductViewModel> subProductDetails { get; set; }
        //public List<LocationProducts> locationProducts { get; set; }
        //public List<PartnerSitesViewModel> siteGridData { get; set; }
        public int createdBy { get; set; }
    }

    public class RepViewModel
    {
        public int id { get; set; }
        public int repId { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string teleNo { get; set; }
        public string mobileNo { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string status { get; set; }
    }


}