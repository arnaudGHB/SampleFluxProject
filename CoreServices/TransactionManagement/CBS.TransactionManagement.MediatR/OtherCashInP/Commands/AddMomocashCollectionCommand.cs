using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.OtherCashInP;
using CBS.TransactionManagement.Data.Dto.Transaction;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CBS.TransactionManagement.MediatR.OtherCashInP.Commands
{
  
    public class AddMomocashCollectionCommand : IRequest<ServiceResponse<OtherTransactionDto>>
    {
        public List<BulkOperation> BulkOperations { get; set; }
        public string DepositType { get; set; }//Normal, LoanFeePayment, Disbursment, LoanRepayment
        public string OperationType { get; set; }
        public bool IsCashOperation { get; set; }
       
    }

}
