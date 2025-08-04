using AutoMapper;
using CBS.BankMGT.Data;
using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.MediatR.Commands;

namespace CBS.BankMGT.API
{
    public class BranchProfile: Profile
    {
        public BranchProfile()
        {
            CreateMap<Branch, BranchDto>().ReverseMap();
            CreateMap<AddBranchCommand, Branch>();
            CreateMap<UpdateBranchCommand, Branch>();
        }
    }
}
