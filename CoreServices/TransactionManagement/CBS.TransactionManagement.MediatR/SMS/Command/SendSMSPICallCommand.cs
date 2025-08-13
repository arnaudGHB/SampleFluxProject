using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Command
{
    /// <summary>
    /// Represents a query to retrieve a specific Transaction by its unique identifier.
    /// </summary>
    public class SendSMSPICallCommand : IRequest<ServiceResponse<SMSDto>>
    {
        public string senderService { get; set; }
        public string recipient { get; set; }
        public string messageBody { get; set; }
    }

   
}
