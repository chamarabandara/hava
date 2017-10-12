using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HavaBusinessObjects.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string description { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public bool? isMainProduct { get; set; }
        public int? MaxPassengers { get; set; }
        public int? MaxLuggage { get; set; }
        public string ProductImagePath { get; set; }
        public ProductFeaturesViewMOdel productFeatures { get; set; }
        public ProductImageViewModel productLogoImage { get; set; }

    }


    public class BookedProductViewModel
    {
        public string description { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public bool? isMainProduct { get; set; }
        public int? MaxPassengers { get; set; }
        public int? MaxLuggage { get; set; }
        public string ProductImagePath { get; set; }
    }

        public class ProductImageViewModel
    {
        public int id { get; set; }
        public int productId { get; set; }
        public string name { get; set; }
        public string documentPath { get; set; }
        public string size { get; set; }
    }

    public class ProductFeaturesViewMOdel
    {
        public int id { get; set; }
        public int productId { get; set; }
        public string description { get; set; }
    }
}