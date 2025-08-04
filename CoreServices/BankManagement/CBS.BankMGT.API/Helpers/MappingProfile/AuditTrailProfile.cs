using AutoMapper;
using CBS.BankMGT.Data;
using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.MediatR.Commands;

namespace CBS.BankMGT.API
{
    public class AuditTrailProfile : Profile
    {
        public AuditTrailProfile()
        {
            CreateMap<AuditTrail, AuditTrailDto>().ReverseMap();
            CreateMap<AddAuditTrailCommand, AuditTrail>();
        }
    }
}
