using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HavaBusinessObjects.ViewModels
{
    public class LocationDetailViewModel
    {
        public int Id { get; set; }
        public string name { get; set; }
        public int? PartnerId { get; set; }
        public bool? IsActive { get; set; }
        public string FromLocation { get; set; }
        public string ToLocation { get; set; }
        public bool IsAirPortTour { get; set; }
    }
}