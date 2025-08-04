using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.MediatR.LoanApplicationMediaR.Commands;
using CBS.NLoan.MediatR.LoanProductMediaR.Commands;

namespace CBS.NLoan.MediatR.LoanApplicationMediaR.Validations
{
    public class LoanValidation
    {
        public static string AddLoanValidation(AddLoanApplicationCommand loan, LoanProduct loanProduct)
        {
            var res = "";

            //if (loan.Amount <= 0)
            //{
            //    res = res + "Amount must greater than 0; ";
            //}
            //if (loanProduct.LoanMaximumAmount < loan.Amount || loan.Amount < loanProduct.LoanMinimumAmount)
            //{
            //    res = res + " Loan amount must between " + loanProduct.LoanMinimumAmount + " and " + loanProduct.LoanMaximumAmount + " ; ";
            //}
            ////if (loanProduct.MaximumNumberOfRepayment < loan.NumberOfRepayment || loan.NumberOfRepayment < loanProduct.MinimumNumberOfRepayment)
            ////{
            ////    res = res + " Number Installment must between " + loanProduct.MinimumNumberOfRepayment + " "+ loanProduct.LoanDurationPeriod+ " and " + loanProduct.MaximumNumberOfRepayment +" "+ loanProduct.LoanDurationPeriod ;
            ////}
            //if (loanProduct.MaximumInterestRate < loan.InterestRate || loan.InterestRate < loanProduct.MinimumInterestRate)
            //{
            //    res = res + " InterestRate must between " + loanProduct.MinimumInterestRate + "% and " + loanProduct.MaximumInterestRate + "%; ";
            //}
            
            return res;
        }
        public static string UpdateLoanApplicationValidation(UpdateLoanApplicationCommand loan, LoanProduct loanProduct)
        {
            var res = "";

            //if (loan.Amount <= 0)
            //{
            //    res = res + "Amount must greater than 0; ";
            //}
            //if (loanProduct.LoanAmountMax < loan.Amount | loan.Amount < loanProduct.LoanAmountMin)
            //{
            //    res = res + " Amount must between " + loanProduct.LoanAmountMin + " and " + loanProduct.LoanAmountMax + " ; ";
            //}
            //if (loanProduct.NumberOfInstallmentMax < loan.NumberOfInstallment | loan.NumberOfInstallment < loanProduct.NumberOfInstallmentMin)
            //{
            //    res = res + " Number Installment must between " + loanProduct.NumberOfInstallmentMin + " and " + loanProduct.NumberOfInstallmentMax + " ; ";
            //}
            //if (loan.Salary <= 0)
            //{
            //    res = res + " Salary must greater than 0; ";
            //}

            return res;
        }

        public static string AddLoanProductValidation(AddLoanProductCommand loanProduct)
        {
            var res = "";
            //if (loanProduct.MinimumNumberOfRepayment <= 0)
            //{
            //    res ="MinimumNumberOfRepayment must greater than 0";
            //}
            //if (loanProduct.MaximumNumberOfRepayment <= 0)
            //{
            //    res = res + "MaximumNumberOfRepayment must greater than 0; ";
            //}
            //if (loanProduct.MaximumNumberOfRepayment < loanProduct.MinimumNumberOfRepayment)
            //{
            //    res = res + "MaximumNumberOfRepayment must greater or eqauls to MinimumNumberOfRepayment; ";
            //}
            //if (loanProduct.LoanMinimumAmount <= 0)
            //{
            //    res = res + "LoanAmountMin must greater than 0; ";
            //}
            //if (loanProduct.LoanMaximumAmount <= 0)
            //{
            //    res = res + "LoanAmountMax must greater than 0; ";
            //}
            //if (loanProduct.LoanMaximumAmount < loanProduct.LoanMinimumAmount)
            //{
            //    res = res + "LoanAmountMax must greater or eqauls to LoanAmountMin; ";
            //}

            return res;
        }

        public static string UpdateLoanProductValidation(UpdateLoanProductCommand loanProduct)
        {
            var res = "";

            //if (loanProduct.NumberOfInstallmentMin <= 0)
            //{
            //    res = res + "NumberInstallmentMin must greater than 0; ";
            //}
            //if (loanProduct.NumberOfInstallmentMax <= 0)
            //{
            //    res = res + "NumberInstallmentMax must greater than 0; ";
            //}
            //if (loanProduct.NumberOfInstallmentMax < loanProduct.NumberOfInstallmentMin)
            //{
            //    res = res + "NumberInstallmentMax must greater or eqauls to NumberInstallmentMin; ";
            //}
            //if (loanProduct.LoanAmountMin <= 0)
            //{
            //    res = res + "LoanAmountMin must greater than 0; ";
            //}
            //if (loanProduct.LoanAmountMax <= 0)
            //{
            //    res = res + "LoanAmountMax must greater than 0; ";
            //}
            //if (loanProduct.LoanAmountMax < loanProduct.LoanAmountMin)
            //{
            //    res = res + "LoanAmountMax must greater or eqauls to LoanAmountMin; ";
            //}

            return res;
        }
    }
}
