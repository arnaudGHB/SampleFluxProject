using AutoMapper;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;

namespace CBS.TransactionManagement.API
{
    public class AccountingEventProfile : Profile
    {
        public AccountingEventProfile()
        {
            CreateMap<AccountingEvent, AccountingEventDto>().ReverseMap();
            CreateMap<AddAccountingEventCommand, AccountingEvent>();
            //CreateMap<UpdateAccountingEventCommand, AccountingEvent>();
        }
    }
}
