using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR.Commands;

namespace CBS.AccountManagement.API.Helpers.MappingProfile
{
    public class ProductAccountingBookProfile : Profile
    {
        public ProductAccountingBookProfile()
        {
            CreateMap<ProductAccountingBook, ProductAccountingBookDto>().ReverseMap();
            CreateMap<AddProductAccountingBookCommand, ProductAccountingBookDto>();
            CreateMap<UpdateProductAccountingBookCommand, ProductAccountingBookDto>();
        }
    }
}