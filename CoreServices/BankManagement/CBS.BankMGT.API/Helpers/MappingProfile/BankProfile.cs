using AutoMapper;
using CBS.BankMGT.Data;
using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.MediatR.Commands;

namespace CBS.BankMGT.API
{
    public class BankProfile: Profile
    {
        public BankProfile()
        {
            CreateMap<Bank, BankDto>().ReverseMap();
            //CreateMap<Organization, OrganizationDto>().ReverseMap();
            //CreateMap<Branch, BranchDto>().ReverseMap();
            //CreateMap<Town, TownDto>().ReverseMap();
            //CreateMap<Subdivision, SubdivisionDto>().ReverseMap();
            CreateMap<AddBankCommand, Bank>();
            CreateMap<UpdateBankCommand, Bank>();
        }
    }
}
