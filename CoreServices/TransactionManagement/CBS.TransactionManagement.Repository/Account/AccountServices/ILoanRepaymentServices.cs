using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.LoanRepayment;
using CBS.TransactionManagement.Data.Dto.Transaction;
using CBS.TransactionManagement.Dto;
using MediatR;

namespace CBS.TransactionManagement.Repository.AccountServices
{
    public interface ILoanRepaymentServices
    {
        IMediator _mediator { get; set; }

        Task<TransactionDto> LoanDepositCash(BulkDeposit request, Teller teller, Account tellerAccount, Account loanTransitAccount, TellerLoanPaymentObject paymentObject);
    }
}