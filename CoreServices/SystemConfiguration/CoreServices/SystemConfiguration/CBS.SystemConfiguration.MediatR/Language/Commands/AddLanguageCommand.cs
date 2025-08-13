using CBS.SystemConfiguration.Data.Dto.Languages;
using CBS.SystemConfiguration.Helper;
using MediatR;

namespace CBS.SystemConfiguration.MediatR.Language.Commands
{
    public class AddLanguageCommand : IRequest<ServiceResponse<LanguageDto>>
    {
        public string Code { get; set; }
        public string Label { get; set; }
    }
}
