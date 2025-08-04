using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Data.Entity;
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Commands
{
    /// <summary>
    /// Represents a command to update a ThirdPartyInstitution.
    /// </summary>
    public class UpdateThirdPartyInstitutionCommand : IRequest<ServiceResponse<CorrespondingBankDto>>
    {

        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string InstitionType { get; set; }
        public string HeadOffice { get; set; }
    }

}
