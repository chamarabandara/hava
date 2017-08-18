using AutoMapper;
using AutoMapper.Configuration;
using HavaBusiness;
using HavaBusinessObjects.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HavaBusinessObjects.Utilities
{
    public class AutoMapperMap : Profile
    {
        public AutoMapperMap()
        {
            CreateMap<LocationDetailViewModel, LocationDetail>().ReverseMap();
            CreateMap<PartnerViewModel, Partner>().ReverseMap();
            CreateMap<PartnerProductRateViewModel, PartnerProductRate>().ReverseMap();
            CreateMap<PartnerProductViewModel, PartnerProduct>().ReverseMap();
            CreateMap<BookingTypeViewModel, BookingType>().ReverseMap();
            CreateMap<BookingPaymentViewModel, BookingPayment>().ReverseMap();
            CreateMap<BookingProductsViewModel, BookingProduct>().ReverseMap();
            CreateMap<BookingOptionViewModel, BookingOption>().ReverseMap();
        }
    
    }
}