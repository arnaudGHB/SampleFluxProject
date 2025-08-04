using CBS.NLoan.Data.Dto.AlertProfileP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.AlertProfileMediaR.Commands
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class AddAlertProfileCommand : IRequest<ServiceResponse<AlertProfileDto>>
    {
        public string Name { get; set; }
        public string Msisdn { get; set; }
        public bool SendSMS { get; set; }
        public bool SendEmail { get; set; }
        public bool IsSupperAdmin { get; set; }
        public string Language { get; set; }
        public bool ActiveStatus { get; set; }
        public string ServiceId { get; set; }
        public string BankId { get; set; }
        public string BranchId { get; set; }
        public string OrganizationId { get; set; }
    }

}
