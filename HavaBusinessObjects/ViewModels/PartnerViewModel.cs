using System;
using System.Collections.Generic;

namespace HavaBusinessObjects.ViewModels
{
    public class PartnerViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string address { get; set; }
        public string email { get; set; }
        public string telephoneLand { get; set; }
        public string telephoneMobile { get; set; }
        public List<RepresentativeViewModel> representativeData { get; set; }
        public List<PartnerProductViewModel> mainProductDetails { get; set; }
        public List<PartnerProductViewModel> subProductDetails { get; set; }
        public List<LocationProducts> locationProducts { get; set; }
        public List<PartnerSitesViewModel> siteGridData { get; set; }
        public int createdBy { get; set; }

    }

    public class RepresentativeViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string teleNo { get; set; }
        public string mobileNo { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string status { get; set; }
    }

    public class PartnerChauffeurProductViewModel
    {
        public int id { get; set; }
        public string productName { get; set; }
        public int productId { get; set; }
        public ProductViewModel Product { get; set; }
        public bool isActive { get; set; }
        public decimal? HavaPrice { get; set; }
        public decimal? MarketPrice { get; set; }
        public decimal? PartnerSellingPrice { get; set; }
        public bool? IsMarkUp { get; set; }
        public decimal? Markup { get; set; }
        public decimal? Percentage { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

    }

    public class PartnerProductViewModel
    {
        public int Id { get; set; }
        public string productName { get; set; }
        public int ProductId { get; set; }
        public ProductViewModel Product { get; set; }
        public bool? IsInclude { get; set; }
        public LocationDetailViewModel LocationDetail { get; set; }
        public decimal? HavaPrice { get; set; }
        public decimal? MarketPrice { get; set; }
        public decimal? PartnerSellingPrice { get; set; }
        public bool? IsMarkUp { get; set; }
        public decimal? Markup { get; set; }
        public decimal? Percentage { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

    }

    public class PartnerSitesViewModel
    {
        public int id { get; set; }
        public int siteId { get; set; }
    }

    public class LocationProducts
    {
        public int id { get; set; }
        public LocationDetailViewModel location { get; set; }
        public List<PartnerProductViewModel> products { get; set; }
    }

    public class PrtnerLocations
    {
        public int locationId { get; set; }
        public string locationName { get; set; }
        public string fromLocation { get; set; }
        public string toLocation { get; set; }
        public bool isAirPortTour { get; set; }
        public bool isActive { get; set; }

        public int productRateId { get; set; }
        public string partnerPercentage { get; set; }
        public string partnerMarkup { get; set; }
        public bool isMarkup { get; set; }
        public string partnerSellingPrice { get; set; }
        public string marketPrice { get; set; }
        public string havaPrice { get; set; }
        public decimal airPortRate { get; set; }
        public decimal havaPriceReturn { get; set; }
        public decimal marketPriceReturn { get; set; }
        public decimal partnerSellPriceReturn { get; set; }
        public decimal additionalDayRate { get; set; }
        public decimal additionalHourRate { get; set; }
        public decimal chufferDailyRate { get; set; }
        public decimal chufferKMRate { get; set; }
        public decimal childSeatRate { get; set; }
        public decimal rate { get; set; }
    }
}