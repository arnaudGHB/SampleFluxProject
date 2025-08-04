using AutoMapper;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Commands.WithdrawalNotificationP;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.WithdrawalNotificationP;
using CBS.TransactionManagement.Data.Entity.WithdrawalNotificationP;
using CBS.TransactionManagement.Dto;

namespace CBS.TransactionManagement.API
{
    public class WithdrawalLimitsProfile : Profile
    {
        public WithdrawalLimitsProfile()
        {
            CreateMap<WithdrawalParameter, WithdrawalParameterDto>().ReverseMap();
            CreateMap<AddWithdrawalLimitsCommand, WithdrawalParameter>();
            CreateMap<UpdateWithdrawalLimitsCommand, WithdrawalParameter>();

            CreateMap<WithdrawalNotification, WithdrawalNotificationDto>().ReverseMap();
            CreateMap<AddWithdrawalNotificationRequestCommand, WithdrawalNotification>().ReverseMap();
            CreateMap<UpdateWithdrawalNotificationCommand, WithdrawalNotification>().ReverseMap();
            CreateMap<ValidationWithdrawalNotificationCommand, WithdrawalNotification>().ReverseMap();
            CreateMap<CashDeskWithdrawalNotificationCommand, WithdrawalNotification>().ReverseMap();

        }
    }
}
