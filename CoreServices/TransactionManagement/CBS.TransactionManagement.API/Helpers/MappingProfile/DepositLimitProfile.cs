using AutoMapper;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;

namespace CBS.TransactionManagement.API
{
    public class DepositLimitProfile : Profile
    {
        public DepositLimitProfile()
        {
            CreateMap<CashDepositParameter, CashDepositParameterDto>().ReverseMap();
            CreateMap<AddDepositLimitCommand, CashDepositParameter>();
            CreateMap<UpdateDepositLimitCommand, CashDepositParameter>();
        }
    }
}
