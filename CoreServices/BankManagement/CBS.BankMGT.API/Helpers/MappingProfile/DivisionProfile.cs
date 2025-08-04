using AutoMapper;
using CBS.BankMGT.Data;
using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.MediatR.Commands;

namespace CBS.BankMGT.API
{
    public class DivisionProfile: Profile
    {
        public DivisionProfile()
        {
            CreateMap<Division, DivisionDto>().ReverseMap();
            CreateMap<AddDivisionCommand, Division>();
            CreateMap<UpdateDivisionCommand, Division>();
        }
    }
}
