using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;
namespace CBS.TransactionManagement.MediatR.TellerP.Commands
{
    public class EndOfDayAccountantCommand : IRequest<ServiceResponse<PrimaryTellerProvisioningHistoryDto>>
    {
        public string primaryTellerProvioningHistoryID { get; set; }
        public string Comment { get; set; }
        public decimal AmountRecieved { get; set; }
        public string EODClosedStatus { get; set; }
        public string AccountantConfirmationStatus { get; set; }
    }
}
