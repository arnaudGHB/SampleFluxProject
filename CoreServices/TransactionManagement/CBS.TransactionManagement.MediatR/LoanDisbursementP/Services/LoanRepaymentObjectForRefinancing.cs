using CBS.TransactionManagement.Data.Dto.Transaction;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.MediatR.LoanRepayment.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBS.TransactionManagement.MediatR.Operations.Cash.CashIn.Commands;
using CBS.TransactionManagement.MediatR.LoanDisbursementP.Commands;
using CBS.TransactionManagement.Dto;

namespace CBS.TransactionManagement.MediatR.LoanDisbursementP.Services
{
    public static class LoanRepaymentObjectForRefinancing
    {
        public static AddBulkOperationDepositCommand MockRepaymentObject(LoanDisbursmentCommand repayment)
        {
            return new AddBulkOperationDepositCommand
            {
                BulkOperations = new List<BulkOperation>
            {
                new BulkOperation
                {
                    AccountNumber = repayment.OldLoanPayment.LoanId,
                    Fee = 0,
                    CustomerId = repayment.CustomerId,
                    Amount = repayment.OldLoanPayment.Capital,
                    Balance = 0, VAT=repayment.OldLoanPayment.VAT,
                    Penalty = repayment.OldLoanPayment.Penalty,
                    Interest = repayment.OldLoanPayment.Interest,
                    Total = repayment.OldLoanPayment.Amount,
                    MembershipActivationAmount = 0,
                    AccountType = repayment.LoanApplicationType,
                    LoanId = repayment.OldLoanPayment.LoanId,
                    LoanApplicationId = null,
                    Note = "Loan Repayment For Refinancing.",
                    OperationType = "LoanRepayment",
                    IsSWS = repayment.IsNormal,
                    SourceType = null,
                    CheckNumber = null,
                    CheckName = null,
                    isDepositDoneByAccountOwner = true,
                    IsChargesInclussive = repayment.IsChargeInclussive,
                    currencyNotes = CurrencyNotesMapper.CalculateCurrencyNotes(repayment.Amount),
                    Depositer = new Depositer
                    {
                        DepositorName = null,
                        DepositerNote = null,
                        DepositerTelephone = null,
                        DepositorIDNumber = null,
                        DepositorIDIssueDate = null,
                        DepositorIDExpiryDate = null,
                        DepositorIDNumberPlaceOfIssue = null
                    }
                }
            },
                DepositType = "LoanRepayment",
                Period = null,
                OperationType = "Deposit",
                IsCashOperation = true,
                Id = null,
                IsPartialRequest=true,
            };
        }


    }
}
