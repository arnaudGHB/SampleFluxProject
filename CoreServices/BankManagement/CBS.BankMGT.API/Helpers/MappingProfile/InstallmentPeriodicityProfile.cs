using AutoMapper;
using CBS.BankMGT.Data;
using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.MediatR.Commands;

namespace CBS.BankMGT.API
{
    public class InstallmentPeriodicityProfile: Profile
    {
        public InstallmentPeriodicityProfile()
        {
            CreateMap<InstallmentPeriodicity, InstallmentPeriodicityDto>().ReverseMap();
            //CreateMap<AddInstallmentPeriodicityCommand, InstallmentPeriodicity>();
            //CreateMap<UpdateInstallmentPeriodicityCommand, InstallmentPeriodicity>();
        }
    }
}
