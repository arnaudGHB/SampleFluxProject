using AutoMapper;
using CBS.BankMGT.Data;
using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Data.Entity;
using CBS.BankMGT.MediatR.Commands;

namespace CBS.BankMGT.API
{
    public class ThirdPartyBrancheProfile : Profile
    {
        public ThirdPartyBrancheProfile()
        {
            CreateMap<CorrespondingBankBranchDto, ThirdPartyBranche>().ReverseMap();
            CreateMap< AddThirdPartyBrancheCommand, ThirdPartyBranche >();
        }
    }

    public class BankingZoneProfile : Profile
    {
        public BankingZoneProfile()
        {
            CreateMap<BankingZone, BankingZoneDto>().ReverseMap();
            CreateMap<AddBankingZoneCommand, BankingZone>();
        }
    }
 
    public class BankZoneBranchProfile : Profile
    {
        public BankZoneBranchProfile()
        {
            CreateMap<BankZoneBranch, BankZoneBranchDto>().ReverseMap();
            CreateMap<AddBankZoneBranchCommand, BankZoneBranchDto>();
        }
    }

}
