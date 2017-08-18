using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HavaBusinessObjects.ViewModels
{
    public class PartnerProductRateViewModel
    {
        public int Id { get; set; }
        public PartnerViewModel Partner { get; set; }
        public LocationDetailViewModel LocationDetail { get; set; }
        public PartnerProductViewModel PartnerProduct { get; set; }
        public decimal? Rate { get; set; }
        public decimal? HavaPrice { get; set; }
        public decimal? MarketPrice { get; set; }
        public decimal? PartnerSellingPrice { get; set; }
        public bool? IsMarkUp { get; set; }
        public decimal? Markup { get; set; }
        public decimal? Percentage { get; set; }
        public decimal? AirportRate { get; set; }
        public decimal? HavaPriceReturn { get; set; }
        public decimal? MarketPriceReturn { get; set; }
        public decimal? PartnerSellingPriceReturn { get; set; }
        public decimal? AdditionaDayRate { get; set; }
        public decimal? AdditionaHourRate { get; set; }
        public decimal? ChufferDailyRate { get; set; }
        public decimal? ChufferKMRate { get; set; }
        public decimal? ChildSeatRate { get; set; }
    }
}