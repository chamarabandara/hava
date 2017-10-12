using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HavaBusinessObjects.ViewModels
{
    public class BookingViewModel
    {
        public int Id { get; set; }
        public BookingTypeViewModel BookingType { get; set; }
        public System.DateTime? PickupDate { get; set; }
        public System.TimeSpan? PickupTime { get; set; }
        public string PickupLocation { get; set; }
        public System.TimeSpan? ReturnTime { get; set; }
        public string ReturnPickupLocation { get; set; }
        public LocationDetailViewModel DropLocation { get; set; }
        public System.DateTime? ReturnDate { get; set; }
        public string RefNo { get; set; }
        public PartnerViewModel Partner { get; set; }
        public int? NumberOfDays { get; set; }
        public BookingStatusViewModel BookingStatu { get; set; }
        public int? TSPId { get; set; }
        public int? PaymentTypeId { get; set; }
        public int? PromotionId { get; set; }
        public bool? HasPromotions { get; set; }
        public bool UserConfirmed { get; set; }
        public int? UserId { get; set; }
        public bool IsReturn { get; set; }
        public bool IsAirportTransfer { get; set; }

        public List<BookingProductsViewModel> BookingProducts { get; set; }
        public List<BookingSubProductViewModel> BookingSubProducts { get; set; }
        public List<BookingPaymentViewModel> BookingPayments { get; set; }
        public List<BookingOptionViewModel> BookingOptions { get; set; }
        public List<BookingPassengerViewModel> BookingPassenger { get; set; }
    }

    public class BookingStatusViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }

    }

    public class BookingTypeViewModel
    {
        public int Id { get; set; }
        public string type { get; set; }
        public bool? IsActive { get; set; }
    }

    public class BookingPaymentViewModel
    {
        public int Id { get; set; }
        public BookingViewModel Booking { get; set; }
        public string CardHolderName { get; set; }
        public string ExpireDate { get; set; }
        public string CardNo { get; set; }
        public int CardType { get; set; }
        public string CVV { get; set; }
    }

    public class BookingProductsViewModel
    {
        public int Id { get; set; }
        public BookingViewModel Booking { get; set; }
        public int? ProductId { get; set; }
        public decimal? Price { get; set; }
        public decimal? PartnerPrice { get; set; }
        public decimal? MarketPrice { get; set; }
        public BookedProductViewModel Product { get; set; }
    }

    public class BookingSubProductViewModel
    {
        public int Id { get; set; }
        public BookingViewModel Booking { get; set; }
        public ProductViewModel Product { get; set; }
        public decimal? HavaPrice { get; set; }
        public decimal? MarketPrice { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
    }

    public class BookingOptionViewModel
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public string FlightNo { get; set; }
        public int? FlyerProgramId { get; set; }
        public string FlyerReffNo { get; set; }
        public string PickupSign { get; set; }
        public string PickupSignReffNo { get; set; }
        public string NoteToDriver { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string PickupAddress { get; set; }
        
    }

    public class BookingPassengerViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public CountryViewModel Country { get; set; }
    }

    public class CommonViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }


}