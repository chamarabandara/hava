using System.Collections.Generic;

namespace HavaBusinessObjects.ViewModels
{
    public class TSPDetailViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string email { get; set; }
        public string telephoneLand { get; set; }
        public string telephoneMobile { get; set; }
        public bool isActive { get; set; }
        public int createdBy { get; set; }
        public List<VehiclesViewModel> vehicles { get; set; }
        public List<ProductsViewModel> products { get; set; }

    }

    public class VehiclesViewModel
    {
        public int id { get; set; }
        public string vehicleNo { get; set; }
        public string regNo { get; set; }
        public string driverName { get; set; }
        public string driverIDDLNo { get; set; }
        public int maxPassengers { get; set; }
        public int maxLuggages { get; set; }
        public int productId { get; set; }
        public bool isActive { get; set; }
        public int rowId { get; set; }
    }

    public class ProductsViewModel
    {
        public int id { get; set; }
        public int productId { get; set; }
        public decimal productPrice { get; set; }
        public bool isActive { get; set; }
        public int rowId { get; set; }
    }
}