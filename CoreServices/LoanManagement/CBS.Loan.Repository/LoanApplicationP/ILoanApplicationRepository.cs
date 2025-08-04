using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Data;
using CBS.NLoan.Data.Dto.CustomerAccountDto;
using CBS.NLoan.Data.Dto.CustomerP;
using CBS.NLoan.Data.Entity.CollateraP;
using CBS.NLoan.Data.Entity.DocumentP;
using CBS.NLoan.Data.Entity.LoanApplicationP;

namespace CBS.NLoan.Repository.LoanApplicationP
{
    public interface ILoanApplicationRepository : IGenericRepository<LoanApplication>
    {
       bool LoanApplicationValidator(LoanApplication loanApplication, LoanProduct loanProduct, List<LoanGuarantor> loanGuarantors, List<LoanApplicationCollateral> productCollaterals, List<DocumentAttachedToLoan> documentAttachedToLoans);
        Task<bool> ValidateMembersAccounts(LoanApplication loanApplication, CustomerDto customer, List<AccountDto> accounts, LoanProduct loanProduct = null);
        decimal DownPaymentProvision(LoanApplication loanApplication, decimal savingBalance);
    }
}
