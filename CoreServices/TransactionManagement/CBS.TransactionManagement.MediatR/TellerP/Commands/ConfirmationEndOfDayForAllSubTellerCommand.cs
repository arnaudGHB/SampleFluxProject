using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.TellerP.Commands
{
    public class ConfirmationEndOfDayForAllSubTellerCommand : IRequest<ServiceResponse<SubTellerProvioningHistoryDto>>
    {
        public string Comment { get; set; }
        public string PrimaryTellerConfirmationStatus { get; set; }
        public string SubTellerProvioningHistoryID { get; set; }
    }
}
