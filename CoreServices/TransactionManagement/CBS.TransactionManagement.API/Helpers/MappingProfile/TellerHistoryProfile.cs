using AutoMapper;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.TellerP.Commands;

namespace CBS.TransactionManagement.API
{
    public class TellerHistoryProfile : Profile
    {
        public TellerHistoryProfile()
        {
            CreateMap<PrimaryTellerProvisioningHistory, PrimaryTellerProvisioningHistoryDto>().ReverseMap();
            CreateMap<SubTellerProvioningHistory, SubTellerProvioningHistoryDto>().ReverseMap();
            CreateMap<AddTellerCommand, PrimaryTellerProvisioningHistory>();
            CreateMap<UpdateTransferCommand, PrimaryTellerProvisioningHistory>();
        }
    }
}
