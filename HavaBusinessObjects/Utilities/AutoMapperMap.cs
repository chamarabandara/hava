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
                .ForMember(dest => dest.PartnerProduct , opt => opt.MapFrom(src => src.PartnerProduct))
                .ReverseMap();
            //CreateMap<PartnerProductViewModel, PartnerProduct>()
            //    .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.productVM))
            //    .MaxDepth(2)
            //    .ReverseMap();
            CreateMap<ProductViewModel , Product>().ReverseMap();
            CreateMap<BookingTypeViewModel , BookingType>().ReverseMap();
            CreateMap<BookingPaymentViewModel , BookingPayment>()
                .ReverseMap();
            CreateMap<BookingProductsViewModel , BookingProduct>().ReverseMap();
            CreateMap<BookingOptionViewModel , BookingOption>().ReverseMap();
            CreateMap<BookingStatusViewModel , BookingStatu>().ReverseMap();
            CreateMap<Booking, BookingViewModel>()
                .MaxDepth(2);

            CreateMap<BookingViewModel, Booking>()
               .ForMember(dest => dest.BookingStatusId, opt => opt.MapFrom(src => src.BookingStatu.Id))
               .MaxDepth(2);

            CreateMap<ProductFeature , ProductFeaturesViewMOdel>().ReverseMap();

        }

    }
}