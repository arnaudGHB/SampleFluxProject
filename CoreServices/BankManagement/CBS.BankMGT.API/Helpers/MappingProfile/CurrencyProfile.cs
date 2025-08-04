using AutoMapper;
using CBS.BankMGT.Data;
using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.MediatR.Commands;

namespace CBS.BankMGT.API
{
    public class CurrencyProfile: Profile
    {
        public CurrencyProfile()
        {
            CreateMap<Currency, CurrencyDto>().ReverseMap();
            CreateMap<AddCurrencyCommand, Currency>();
            CreateMap<UpdateCurrencyCommand, Currency>();
        }
    }
}
