using AutoMapper;
using HavaBusiness;
using HavaBusinessObjects.ViewModels;

namespace HavaBusinessObjects.Utilities
{
    public class AutoMapperMap : Profile
    {
        public AutoMapperMap()
        {
            CreateMap<LocationDetailViewModel , LocationDetail>().ReverseMap();
            CreateMap<PartnerViewModel , Partner>().ReverseMap();
            CreateMap<PartnerProductRateViewModel , PartnerProductRate>()
                .ForMember(dest => dest.Partner , opt => opt.MapFrom(src => src.Partner))
                //.ForMember(dest => dest.PartnerProduct , opt => opt.MapFrom(src => src.PartnerProduct))
                .ReverseMap();
            //CreateMap<PartnerProductViewModel, PartnerProduct>()
            //    .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.productVM))
            //    .MaxDepth(2)
            //    .ReverseMap();
            CreateMap<ProductViewModel , Product>();
            CreateMap<Product, ProductViewModel>()
                .ForMember(dest => dest.productFeatures, opt => opt.MapFrom(src => src.ProductFeatures));

            CreateMap<ProductFeature, ProductFeaturesViewMOdel>().ReverseMap();

            CreateMap<BookingTypeViewModel , BookingType>().ReverseMap();
            CreateMap<BookedProductViewModel, Product>().ReverseMap();
            CreateMap<BookingPayment, BookingPaymentViewModel>()
                 .ReverseMap();

            //CreateMap<BookingPaymentViewModel, BookingPayment>()
            //    .ForMember(dest => dest.CardType.Value, opt => opt.MapFrom(src => src.CardType.Id))
            //     .MaxDepth(2);

            CreateMap<CommonViewModel, Common>().ReverseMap();
            CreateMap<CountryViewModel, Country>().ReverseMap();

            CreateMap<BookingProductsViewModel , BookingProduct>().ReverseMap();
            CreateMap<BookingSubProductViewModel, BookingSubProduct>()
                .ReverseMap();

            CreateMap<BookingOption, BookingOptionViewModel>()
                .ReverseMap()
                        .MaxDepth(2);

            CreateMap<BookingPassengerViewModel, BookingPassenger>()
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country.Id))
                        .MaxDepth(2);

            CreateMap<BookingPassenger, BookingPassengerViewModel>()
                 .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
                        .MaxDepth(2);

            CreateMap<BookingStatusViewModel , BookingStatu>().ReverseMap();
            CreateMap<Booking, BookingViewModel>()
                .ForMember(dest => dest.DropLocation, opt => opt.MapFrom(src => src.LocationDetail))
               .ForMember(dest => dest.BookingSubProducts, opt => opt.MapFrom(src => src.BookingSubProducts))
                .MaxDepth(4);

            CreateMap<BookingViewModel, Booking>()
               .ForMember(dest => dest.BookingStatusId, opt => opt.MapFrom(src => src.BookingStatu.Id))
               .ForMember(dest => dest.DropLocation, opt => opt.MapFrom(src => src.DropLocation.Id))
               .MaxDepth(3);

            CreateMap<ProductFeature , ProductFeaturesViewMOdel>().ReverseMap();

            CreateMap<Promotion, PromotionViewModel>().MaxDepth(2).ReverseMap();

            CreateMap<PromotionDiscount, PromotionDiscountViewModel>().ReverseMap();
            CreateMap<PromotionDiscountType, PromotionDiscountTypeViewModel>().ReverseMap();

        }

    }
}