using AutoMapper;
using CBS.BankMGT.Data;
using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.MediatR.Commands;

namespace CBS.BankMGT.API
{
    public class FundingLineProfile: Profile
    {
        public FundingLineProfile()
        {
            CreateMap<FundingLine, FundingLineDto>().ReverseMap();
            //CreateMap<AddFundingLineCommand, FundingLine>();
            //CreateMap<UpdateFundingLineCommand, FundingLine>();
        }
    }
}
