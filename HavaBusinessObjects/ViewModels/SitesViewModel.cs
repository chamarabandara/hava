using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HavaBusinessObjects.ViewModels
{
    public class SitesViewModel
    {
        public string description { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public Nullable<bool> isMainProduct { get; set; }
        public ProductImageViewModel productLogoImage { get; set; }

    }
    public class SitesImageViewModel
    {
        public int id { get; set; }
        public int productId { get; set; }
        public string name { get; set; }
        public string documentPath { get; set; }
        public string size { get; set; }
    }


}