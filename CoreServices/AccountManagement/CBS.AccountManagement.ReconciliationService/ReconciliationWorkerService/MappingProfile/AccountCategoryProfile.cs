using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Entity;
using CBS.AccountManagement.MediatR.Commands;

namespace CBS.AccountManagement.API
{
    public class AccountCategoryProfile : Profile
    {
        public AccountCategoryProfile()
        {
            CreateMap<AccountCategory, AccountCartegoryDto>().ReverseMap();
            CreateMap<AddAccountCategoryCommand, AccountCategory>();
            CreateMap<UpdateAccountCartegoryCommand, AccountCategory>();
        }
    }

    public class DepositNotificationProfile : Profile
    {
        public DepositNotificationProfile()
        {
            CreateMap<AddDepositNotificationCommand, DepositNotificationDto>().ReverseMap();
            CreateMap<DepositNotification, DepositNotificationDto>().ReverseMap();
            CreateMap<UpdateDepositNotificationCommand, DepositNotification>();
        }
    }

    public class ReportProfile :Profile
    {
        public ReportProfile()
        {
            CreateMap<AddReportCommand, ReportDownload>().ReverseMap();
            CreateMap<ReportDownload, ReportDto>().ReverseMap();
            
        }
    }
}