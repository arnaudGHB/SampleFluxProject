using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Command
{
  
    public class AddAccountingEventCommand : IRequest<ServiceResponse<AccountingEventDto>>
    {
        public string EventCode { get; set; }
        public string ChartOfAccountId { get; set; }
        public string OperationEventAttributeId { get; set; }
        public string OperationAccountTypeId { get; set; }
        public string OperationType { get; set; } //Loan_Product,Teller,Saving_Product
    }
}
