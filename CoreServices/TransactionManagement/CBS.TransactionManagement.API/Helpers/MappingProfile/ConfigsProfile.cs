using AutoMapper;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.MemberAccountConfiguration;
using CBS.TransactionManagement.Data.Entity.MemberAccountConfiguration;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.MediatR.MemberAccountConfiguration.MemberAccountActivationPolici.Commands;
using CBS.TransactionManagement.MemberAccountConfiguration.Commands;

namespace CBS.TransactionManagement.API
{
    public class ConfigsProfile : Profile
    {
        public ConfigsProfile()
        {
            CreateMap<Config, ConfigDto>().ReverseMap();
            CreateMap<UpdateConfigCommand, Config>().ReverseMap();
            CreateMap<AddConfigCommand, Config>().ReverseMap();

            CreateMap<MemberRegistrationFeePolicy, MemberAccountActivationPolicyDto>().ReverseMap();
            CreateMap<AddMemberAccountActivationPolicyCommand, MemberRegistrationFeePolicy>();
            CreateMap<UpdateMemberAccountActivationPolicyCommand, MemberRegistrationFeePolicy>();
            CreateMap<MemberAccountActivation, MemberAccountActivationDto>().ReverseMap();
            CreateMap<AddMemberAccountActivationCommand, MemberAccountActivation>();
            CreateMap<UpdateMemberAccountActivationCommand, MemberAccountActivation>();
        }
    }
}
