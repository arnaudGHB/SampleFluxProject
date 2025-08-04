using AutoMapper;
using CBS.BankMGT.Data;
using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Data.Entity;
using CBS.BankMGT.MediatR.Commands;

namespace CBS.BankMGT.API
{
    public class ThirdPartyInstitutionProfile : Profile
    {
        public ThirdPartyInstitutionProfile()
        {
            //CreateMap<CorrespondingBank, CorrespondingBankDto>().ReverseMap();
            //CreateMap<AddThirdPartyInstitutionCommand, CorrespondingBankDto>();
            //CreateMap<AddThirdPartyInstitutionCommand, CorrespondingBank>().ReverseMap();
        }
    }
}
