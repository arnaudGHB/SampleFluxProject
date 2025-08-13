using AutoMapper;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.MediatR.ChangeManagement.Command;

namespace CBS.TransactionManagement.API.Helpers.MappingProfile
{
    public class CurrencyNotesProfile : Profile
    {
        public CurrencyNotesProfile()
        {
            CreateMap<CurrencyNotes, CurrencyNotesDto>().ReverseMap();
            CreateMap<CurrencyNotesRequest, CurrencyNotes>();
            CreateMap<AddCurrencyNotesCommand, CurrencyNotes>();
        }
    }
}
