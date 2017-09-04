﻿using System.Collections.Generic;

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
        public List<PartnerProductViewModel> productGridData { get; set; }
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

    public class PartnerProductViewModel
    {
        public int id { get; set; }
        public int productId { get; set; }
        public bool isActive { get; set; }
        public List<PrtnerLocations> partnerLocations { get; set; }

    }

    public class PartnerSitesViewModel
    {
        public int id { get; set; }
        public int siteId { get; set; }
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