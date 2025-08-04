using AutoMapper;
using CBS.BankMGT.Data;
using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.MediatR.Commands;

namespace CBS.BankMGT.API
{
    public class SubdivisionProfile: Profile
    {
        public SubdivisionProfile()
        {
            CreateMap<Subdivision, SubdivisionDto>().ReverseMap();
            CreateMap<AddSubdivisionCommand, Subdivision>();
            CreateMap<UpdateSubdivisionCommand, Subdivision>();
        }
    }
}
