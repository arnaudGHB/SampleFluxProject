using AutoMapper;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;

namespace CBS.TransactionManagement.API
{
    public class ManagementFeeParameterProfile : Profile
    {
        public ManagementFeeParameterProfile()
        {
            CreateMap<ManagementFeeParameter, ManagementFeeParameterDto>().ReverseMap();
            CreateMap<AddManagementFeeParameterCommand, ManagementFeeParameter>();
            CreateMap<UpdateManagementFeeParameterCommand, ManagementFeeParameter>();
        }
    }
}
