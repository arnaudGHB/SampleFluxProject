using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Data.Enums;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.Commands;
using CBS.NLoan.MediatR.LoanApplicationMediaR.Commands;
using CBS.NLoan.MediatR.LoanProductMediaR.Commands;
using System.Text;

namespace CBS.NLoan.MediatR
{
    public class LoanValidation
    {
        public static string AddLoanValidation(AddLoanApplicationCommand applicationCommand, LoanProduct loanProduct)
        {
            // Initialize the StringBuilder to store validation messages.
            StringBuilder res = new StringBuilder();

            // Check if the loan application type is "Reschedule".
            if (applicationCommand.LoanApplicationType == LoanApplicationTypes.Reschedule.ToString())
            {
                // Validate Loan Duration
                if (applicationCommand.LoanDuration < loanProduct.LoanTerm.MinInMonth)
                {
                    res.Append($"The loan duration must be at least {loanProduct.LoanTerm.MinInMonth} {loanProduct.LoanDurationPeriod}. Please adjust the duration accordingly. ");
                }
                if (applicationCommand.LoanDuration > loanProduct.LoanTerm.MaxInMonth)
                {
                    res.Append($"The loan duration cannot exceed {loanProduct.LoanTerm.MaxInMonth} {loanProduct.LoanDurationPeriod}. Please revise the duration to meet the requirements. ");
                }

                // Validate Interest Rate
                if (applicationCommand.InterestRate < loanProduct.MinimumInterestRate)
                {
                    res.Append($"The interest rate must be at least {loanProduct.MinimumInterestRate}%. Please provide an appropriate interest rate. ");
                }
                if (applicationCommand.InterestRate > loanProduct.MaximumInterestRate)
                {
                    res.Append($"The interest rate cannot exceed {loanProduct.MaximumInterestRate}%. Kindly revise the interest rate to comply with the specified limits. ");
                }

                // Validate Share Account Coverage
                if (applicationCommand.ShareAccountCoverageAmount < loanProduct.MinimumShareAccountBalanceForTheRequestAmount)
                {
                    res.Append($"The share account balance must be at least {BaseUtilities.FormatCurrency(loanProduct.MinimumShareAccountBalanceForTheRequestAmount)}. Ensure the share account meets this requirement. ");
                }

                // Validate Saving Account Coverage
                if (applicationCommand.SavingAccountCoverageRate < loanProduct.MinimumSavingAccountBalanceRateForTheRequestAmount)
                {
                    res.Append($"The saving account balance rate must be at least {loanProduct.MinimumSavingAccountBalanceRateForTheRequestAmount}%. Please adjust the saving account balance accordingly. ");
                }

                // Validate required Saving Account
                if (loanProduct.IsRequiredSavingAccount && applicationCommand.SavingAccountCoverageRate == 0)
                {
                    res.Append("A saving account is required for this loan application but has not been provided. Please include the necessary saving account details. ");
                }

                // Validate required Share Account
                if (loanProduct.IsRequiredShareAccount && applicationCommand.ShareAccountCoverageAmount == 0)
                {
                    res.Append("A share account is required for this loan application but has not been provided. Ensure the share account details are included. ");
                }

                // Validate required Salary Account
                if (loanProduct.IsRequiredSalaryccount && applicationCommand.SalaryAccountCoverageAmount == 0)
                {
                    res.Append("A salary account is required for this loan application but has not been provided. Please provide the necessary salary account details. ");
                }

                // Validate required Down Payment
                if (applicationCommand.RequiredDownPaymentCoverageRate && applicationCommand.DownPaymentCoverageAmountProvided == 0)
                {
                    res.Append("A down payment is required for this loan application but has not been provided. Ensure the down payment amount is specified. ");
                }

                // Return the validation result as a string.
                return res.ToString();
            }
            // Check if the loan application type is "Restructure".
            else if (applicationCommand.LoanApplicationType == LoanApplicationTypes.Restructure.ToString())
            {
                // Validate Loan Duration
                if (applicationCommand.LoanDuration < loanProduct.LoanTerm.MinInMonth)
                {
                    res.Append($"The loan duration must be at least {loanProduct.LoanTerm.MinInMonth} {loanProduct.LoanDurationPeriod}. Please adjust the duration to meet this requirement. ");
                }
                if (applicationCommand.LoanDuration > loanProduct.LoanTerm.MaxInMonth)
                {
                    res.Append($"The loan duration cannot exceed {loanProduct.LoanTerm.MaxInMonth} {loanProduct.LoanDurationPeriod}. Kindly revise the duration accordingly. ");
                }

                // Validate Interest Rate
                if (applicationCommand.InterestRate < loanProduct.MinimumInterestRate)
                {
                    res.Append($"The interest rate must be at least {loanProduct.MinimumInterestRate}%. Please adjust the rate to comply with this minimum. ");
                }
                if (applicationCommand.InterestRate > loanProduct.MaximumInterestRate)
                {
                    res.Append($"The interest rate cannot exceed {loanProduct.MaximumInterestRate}%. Kindly provide an interest rate within the specified range. ");
                }

                // Validate Share Account Coverage
                if (applicationCommand.ShareAccountCoverageAmount < loanProduct.MinimumShareAccountBalanceForTheRequestAmount)
                {
                    res.Append($"The share account balance must be at least {BaseUtilities.FormatCurrency(loanProduct.MinimumShareAccountBalanceForTheRequestAmount)}. Ensure that the share account meets this criterion. ");
                }

                // Validate Saving Account Coverage
                if (applicationCommand.SavingAccountCoverageRate < loanProduct.MinimumSavingAccountBalanceRateForTheRequestAmount)
                {
                    res.Append($"The saving account balance rate must be at least {loanProduct.MinimumSavingAccountBalanceRateForTheRequestAmount}%. Please adjust the saving account balance rate accordingly. ");
                }

                // Validate required Saving Account
                if (loanProduct.IsRequiredSavingAccount && applicationCommand.SavingAccountCoverageRate == 0)
                {
                    res.Append("A saving account is required for this loan application but has not been provided. Please include the saving account details. ");
                }

                // Validate required Share Account
                if (loanProduct.IsRequiredShareAccount && applicationCommand.ShareAccountCoverageAmount == 0)
                {
                    res.Append("A share account is required for this loan application but has not been provided. Ensure the share account details are specified. ");
                }

                // Validate required Salary Account
                if (loanProduct.IsRequiredSalaryccount && applicationCommand.SalaryAccountCoverageAmount == 0)
                {
                    res.Append("A salary account is required for this loan application but has not been provided. Please provide the salary account information. ");
                }

                // Validate required Down Payment
                if (applicationCommand.RequiredDownPaymentCoverageRate && applicationCommand.DownPaymentCoverageAmountProvided == 0)
                {
                    res.Append("A down payment is required for this loan application but has not been provided. Ensure the down payment details are included. ");
                }

                // Return the validation result as a string.
                return res.ToString();
            }
            // Handle standard loan applications
            else
            {
                // Validate Loan Amount
                if (applicationCommand.Amount <= 0)
                {
                    res.Append("The loan amount must be greater than 0. Please provide a valid loan amount. ");
                }
                if (applicationCommand.Amount < loanProduct.LoanMinimumAmount)
                {
                    res.Append($"The loan amount must be at least {BaseUtilities.FormatCurrency(loanProduct.LoanMinimumAmount)}. Adjust the requested amount accordingly. ");
                }
                if (applicationCommand.Amount > loanProduct.LoanMaximumAmount)
                {
                    res.Append($"The loan amount must not exceed {BaseUtilities.FormatCurrency(loanProduct.LoanMaximumAmount)}. Please revise the requested amount to meet this limit. ");
                }

                // Validate Loan Duration
                if (applicationCommand.LoanDuration < loanProduct.LoanTerm.MinInMonth)
                {
                    res.Append($"The loan duration must be at least {loanProduct.LoanTerm.MinInMonth} {loanProduct.LoanDurationPeriod}. Adjust the duration to comply with this requirement. ");
                }
                if (applicationCommand.LoanDuration > loanProduct.LoanTerm.MaxInMonth)
                {
                    res.Append($"The loan duration cannot exceed {loanProduct.LoanTerm.MaxInMonth} {loanProduct.LoanDurationPeriod}. Please revise the loan duration accordingly. ");
                }

                // Validate Interest Rate
                if (applicationCommand.InterestRate < loanProduct.MinimumInterestRate)
                {
                    res.Append($"The interest rate must be at least {loanProduct.MinimumInterestRate}%. Ensure the interest rate meets the minimum requirement. ");
                }
                if (applicationCommand.InterestRate > loanProduct.MaximumInterestRate)
                {
                    res.Append($"The interest rate must not exceed {loanProduct.MaximumInterestRate}%. Adjust the interest rate within the permissible range. ");
                }

                // Validate Share Account Coverage
                if (applicationCommand.ShareAccountCoverageAmount < loanProduct.MinimumShareAccountBalanceForTheRequestAmount)
                {
                    res.Append($"The share account balance must be at least {BaseUtilities.FormatCurrency(loanProduct.MinimumShareAccountBalanceForTheRequestAmount)}. Ensure the share account balance meets this threshold. ");
                }

                // Validate Saving Account Coverage
                if (applicationCommand.SavingAccountCoverageRate < loanProduct.MinimumSavingAccountBalanceRateForTheRequestAmount)
                {
                    res.Append($"The saving account balance rate must be at least {loanProduct.MinimumSavingAccountBalanceRateForTheRequestAmount}%. Adjust the saving account balance rate accordingly. ");
                }

                // Validate required Saving Account
                if (loanProduct.IsRequiredSavingAccount && applicationCommand.SavingAccountCoverageRate == 0)
                {
                    res.Append("A saving account is required for this loan application but has not been provided. Please include the saving account details. ");
                }

                // Validate required Share Account
                if (loanProduct.IsRequiredShareAccount && applicationCommand.ShareAccountCoverageAmount == 0)
                {
                    res.Append("A share account is required for this loan application but has not been provided. Ensure the share account details are specified. ");
                }

                // Validate required Salary Account
                if (loanProduct.IsRequiredSalaryccount && applicationCommand.SalaryAccountCoverageAmount == 0)
                {
                    res.Append("A salary account is required for this loan application but has not been provided. Please provide the salary account information. ");
                }

                if (loanProduct.MinimumDownPaymentPercentage > 0 && applicationCommand.DownPaymentCoverageAmountProvided == 0)
                {
                    var downPaymentValue = (loanProduct.MinimumDownPaymentPercentage / 100) * applicationCommand.Amount;

                    res.Append($"A down payment is required for this loan application. ");
                    res.Append($"The required down payment percentage is {loanProduct.MinimumDownPaymentPercentage}% ");
                    res.Append($"(equivalent to {BaseUtilities.FormatCurrency(downPaymentValue)} for the requested loan amount of {BaseUtilities.FormatCurrency(applicationCommand.Amount)}), ");
                    res.Append($"but no down payment amount has been provided. ");
                    res.Append($"Please ensure the down payment coverage amount of {downPaymentValue:C} is included.");
                }


                // Return the validation result as a string.
                return res.ToString();
            }
        }
        public static string UpdateloanapplicationValidation(UpdateLoanApplicationCommand applicationCommand, LoanProduct loanProduct, LoanApplication loanApplication)
        {
            // Initialize the StringBuilder to store validation messages.
            StringBuilder res = new StringBuilder();

            // Check if the loan application type is "Reschedule".
            if (loanApplication.LoanApplicationType == LoanApplicationTypes.Reschedule.ToString())
            {
                // Validate Loan Duration
                if (applicationCommand.LoanDuration < loanProduct.LoanTerm.MinInMonth)
                {
                    res.Append($"The loan duration must be at least {loanProduct.LoanTerm.MinInMonth} {loanProduct.LoanDurationPeriod}. Please adjust the duration accordingly. ");
                }
                if (applicationCommand.LoanDuration > loanProduct.LoanTerm.MaxInMonth)
                {
                    res.Append($"The loan duration cannot exceed {loanProduct.LoanTerm.MaxInMonth} {loanProduct.LoanDurationPeriod}. Please revise the duration to meet the requirements. ");
                }

                // Validate Interest Rate
                if (applicationCommand.InterestRate < loanProduct.MinimumInterestRate)
                {
                    res.Append($"The interest rate must be at least {loanProduct.MinimumInterestRate}%. Please provide an appropriate interest rate. ");
                }
                if (applicationCommand.InterestRate > loanProduct.MaximumInterestRate)
                {
                    res.Append($"The interest rate cannot exceed {loanProduct.MaximumInterestRate}%. Kindly revise the interest rate to comply with the specified limits. ");
                }

                // Validate Share Account Coverage
                if (applicationCommand.ShareAccountCoverageAmount < loanProduct.MinimumShareAccountBalanceForTheRequestAmount)
                {
                    res.Append($"The share account balance must be at least {BaseUtilities.FormatCurrency(loanProduct.MinimumShareAccountBalanceForTheRequestAmount)}. Ensure the share account meets this requirement. ");
                }

                // Validate Saving Account Coverage
                if (applicationCommand.SavingAccountCoverageRate < loanProduct.MinimumSavingAccountBalanceRateForTheRequestAmount)
                {
                    res.Append($"The saving account balance rate must be at least {loanProduct.MinimumSavingAccountBalanceRateForTheRequestAmount}%. Please adjust the saving account balance accordingly. ");
                }

                // Validate required Saving Account
                if (loanProduct.IsRequiredSavingAccount && applicationCommand.SavingAccountCoverageRate == 0)
                {
                    res.Append("A saving account is required for this loan application but has not been provided. Please include the necessary saving account details. ");
                }

                // Validate required Share Account
                if (loanProduct.IsRequiredShareAccount && applicationCommand.ShareAccountCoverageAmount == 0)
                {
                    res.Append("A share account is required for this loan application but has not been provided. Ensure the share account details are included. ");
                }

               

             

                // Return the validation result as a string.
                return res.ToString();
            }
            // Check if the loan application type is "Restructure".
            else if (loanApplication.LoanApplicationType == LoanApplicationTypes.Restructure.ToString())
            {
                // Validate Loan Duration
                if (applicationCommand.LoanDuration < loanProduct.LoanTerm.MinInMonth)
                {
                    res.Append($"The loan duration must be at least {loanProduct.LoanTerm.MinInMonth} {loanProduct.LoanDurationPeriod}. Please adjust the duration to meet this requirement. ");
                }
                if (applicationCommand.LoanDuration > loanProduct.LoanTerm.MaxInMonth)
                {
                    res.Append($"The loan duration cannot exceed {loanProduct.LoanTerm.MaxInMonth} {loanProduct.LoanDurationPeriod}. Kindly revise the duration accordingly. ");
                }

                // Validate Interest Rate
                if (applicationCommand.InterestRate < loanProduct.MinimumInterestRate)
                {
                    res.Append($"The interest rate must be at least {loanProduct.MinimumInterestRate}%. Please adjust the rate to comply with this minimum. ");
                }
                if (applicationCommand.InterestRate > loanProduct.MaximumInterestRate)
                {
                    res.Append($"The interest rate cannot exceed {loanProduct.MaximumInterestRate}%. Kindly provide an interest rate within the specified range. ");
                }

                // Validate Share Account Coverage
                if (applicationCommand.ShareAccountCoverageAmount < loanProduct.MinimumShareAccountBalanceForTheRequestAmount)
                {
                    res.Append($"The share account balance must be at least {BaseUtilities.FormatCurrency(loanProduct.MinimumShareAccountBalanceForTheRequestAmount)}. Ensure that the share account meets this criterion. ");
                }

                // Validate Saving Account Coverage
                if (applicationCommand.SavingAccountCoverageRate < loanProduct.MinimumSavingAccountBalanceRateForTheRequestAmount)
                {
                    res.Append($"The saving account balance rate must be at least {loanProduct.MinimumSavingAccountBalanceRateForTheRequestAmount}%. Please adjust the saving account balance rate accordingly. ");
                }

                // Validate required Saving Account
                if (loanProduct.IsRequiredSavingAccount && applicationCommand.SavingAccountCoverageRate == 0)
                {
                    res.Append("A saving account is required for this loan application but has not been provided. Please include the saving account details. ");
                }

                // Validate required Share Account
                if (loanProduct.IsRequiredShareAccount && applicationCommand.ShareAccountCoverageAmount == 0)
                {
                    res.Append("A share account is required for this loan application but has not been provided. Ensure the share account details are specified. ");
                }

               

                // Validate required Down Payment
                if (applicationCommand.RequiredDownPaymentCoverageRate && loanApplication.DownPaymentCoverageAmountProvided == 0)
                {
                    res.Append("A down payment is required for this loan application but has not been provided. Ensure the down payment details are included. ");
                }

                // Return the validation result as a string.
                return res.ToString();
            }
            // Handle standard loan applications
            else
            {
                // Validate Loan Amount
                if (applicationCommand.Amount <= 0)
                {
                    res.Append("The loan amount must be greater than 0. Please provide a valid loan amount. ");
                }
                if (applicationCommand.Amount < loanProduct.LoanMinimumAmount)
                {
                    res.Append($"The loan amount must be at least {BaseUtilities.FormatCurrency(loanProduct.LoanMinimumAmount)}. Adjust the requested amount accordingly. ");
                }
                if (applicationCommand.Amount > loanProduct.LoanMaximumAmount)
                {
                    res.Append($"The loan amount must not exceed {BaseUtilities.FormatCurrency(loanProduct.LoanMaximumAmount)}. Please revise the requested amount to meet this limit. ");
                }

                // Validate Loan Duration
                if (applicationCommand.LoanDuration < loanProduct.LoanTerm.MinInMonth)
                {
                    res.Append($"The loan duration must be at least {loanProduct.LoanTerm.MinInMonth} {loanProduct.LoanDurationPeriod}. Adjust the duration to comply with this requirement. ");
                }
                if (applicationCommand.LoanDuration > loanProduct.LoanTerm.MaxInMonth)
                {
                    res.Append($"The loan duration cannot exceed {loanProduct.LoanTerm.MaxInMonth} {loanProduct.LoanDurationPeriod}. Please revise the loan duration accordingly. ");
                }

                // Validate Interest Rate
                if (applicationCommand.InterestRate < loanProduct.MinimumInterestRate)
                {
                    res.Append($"The interest rate must be at least {loanProduct.MinimumInterestRate}%. Ensure the interest rate meets the minimum requirement. ");
                }
                if (applicationCommand.InterestRate > loanProduct.MaximumInterestRate)
                {
                    res.Append($"The interest rate must not exceed {loanProduct.MaximumInterestRate}%. Adjust the interest rate within the permissible range. ");
                }

                // Validate Share Account Coverage
                if (applicationCommand.ShareAccountCoverageAmount < loanProduct.MinimumShareAccountBalanceForTheRequestAmount)
                {
                    res.Append($"The share account balance must be at least {BaseUtilities.FormatCurrency(loanProduct.MinimumShareAccountBalanceForTheRequestAmount)}. Ensure the share account balance meets this threshold. ");
                }

                // Validate Saving Account Coverage
                if (applicationCommand.SavingAccountCoverageRate < loanProduct.MinimumSavingAccountBalanceRateForTheRequestAmount)
                {
                    res.Append($"The saving account balance rate must be at least {loanProduct.MinimumSavingAccountBalanceRateForTheRequestAmount}%. Adjust the saving account balance rate accordingly. ");
                }

                // Validate required Saving Account
                if (loanProduct.IsRequiredSavingAccount && applicationCommand.SavingAccountCoverageRate == 0)
                {
                    res.Append("A saving account is required for this loan application but has not been provided. Please include the saving account details. ");
                }

                // Validate required Share Account
                if (loanProduct.IsRequiredShareAccount && applicationCommand.ShareAccountCoverageAmount == 0)
                {
                    res.Append("A share account is required for this loan application but has not been provided. Ensure the share account details are specified. ");
                }

               

                if (loanProduct.MinimumDownPaymentPercentage > 0 && loanApplication.DownPaymentCoverageAmountProvided == 0)
                {
                    var downPaymentValue = (loanProduct.MinimumDownPaymentPercentage / 100) * applicationCommand.Amount;

                    res.Append($"A down payment is required for this loan application. ");
                    res.Append($"The required down payment percentage is {loanProduct.MinimumDownPaymentPercentage}% ");
                    res.Append($"(equivalent to {BaseUtilities.FormatCurrency(downPaymentValue)} for the requested loan amount of {BaseUtilities.FormatCurrency(applicationCommand.Amount)}), ");
                    res.Append($"but no down payment amount has been provided. ");
                    res.Append($"Please ensure the down payment coverage amount of {downPaymentValue:C} is included.");
                }


                // Return the validation result as a string.
                return res.ToString();
            }
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
        public static Loan NewLoanObjectMappring(LoanApplication application, List<LoanAmortization> loanInstallments, string loanId, string branchCode, string customerName, LoanProduct loanProduct)
        {
            var Penalty = loanInstallments.FirstOrDefault().Penalty;
            decimal Fee = application.LoanApplicationFees.Sum(x => x.FeeAmount);
            decimal FeePaid = application.LoanApplicationFees.Where(x => x.IsPaid).Sum(x => x.AmountPaid);
            decimal VAT = 0;
            decimal interest = loanInstallments.Sum(x => x.Interest);
            var Balance = loanInstallments.Sum(x => x.Principal);

            var nextInstallment = loanInstallments.FirstOrDefault(x => x.Sno == 2);
            var nextInstallmentDate = nextInstallment?.NextPaymentDate ?? loanInstallments.FirstOrDefault(x => x.Sno == 1).NextPaymentDate;
            var loan = new Loan
            {
                LoanApplicationId = application.Id,
                Id = loanId,
                BankId = application.BankId,
                BranchId = application.BranchId,
                CustomerId = application.CustomerId,
                DisbursementDate = DateTime.MinValue,
                DueAmount = /*application.IsInterestPaidUpFront ? (application.InterestAmountUpfront + application.Amount) :*/ application.Amount,
                LoanAmount = application.Amount,
                LastEventData = DateTime.MinValue,
                InterestForcasted = /*application.IsInterestPaidUpFront ? application.InterestAmountUpfront : */loanInstallments.Sum(x => x.Interest),
                InterestRate = application.InterestRate,
                Principal = application.Amount,
                Tax = 0,
                TaxPaid = 0,
                InterestMustBePaidUpFront=false,
                InterestAmountUpfront=0/*application.InterestAmountUpfront*/,
                LoanJourneyStatus = LoanStatus.NormalLoans.ToString(),
                LoanCategory = application.LoanCategory,
                LoanTarget = application.LoanTarget,
                AccrualInterestPaid = 0,
                AccrualInterest = application.IsInterestPaidUpFront ? application.InterestAmountUpfront : 0,
                TotalPrincipalPaid = 0,
                Fee = Fee - FeePaid,
                Penalty = 0,
                PenaltyPaid = 0,
                VatRate = application.VatRate,
                Paid = 0,
                LastPayment = 0,
                FeePaid = FeePaid,
                LoanType = application.LoanType,
                LoanDuration = application.LoanDuration,
                BranchCode = branchCode,
                CustomerName = customerName,
                DisbursmentStatus = DisbursmentStatus.Pending.ToString(),
                Balance = application.Amount,
                IsCurrentLoan = true,
                IsDeliquentLoan = false,
                LoanStructuringStatus = application.LoanApplicationType,
                IsWriteOffLoan = false,
                LastInterestCalculatedDate = BaseUtilities.UtcToDoualaTime(DateTime.Now),
                LastRefundDate = DateTime.MinValue,
                LoanStatus = DisbursmentStatus.Pending.ToString(),
                FirstInstallmentDate = DateTime.Now.AddMonths(application.GracePeriodBeforeFirstPayment),
                NextInstallmentDate = nextInstallmentDate,
                MaturityDate = DateTime.Now.AddMonths(application.LoanDuration),
                OrganizationId = application.OrganizationId,
                LoanDate = BaseUtilities.UtcToDoualaTime(DateTime.Now),
                IsLoanDisbursted = false,
                LoanManager = application.LoanManager,
                NewLoanId = string.Empty,
                LastCalculatedInterest = 0,
                LoanId = loanId,
                AdvancedPaymentAmount=0,
                AdvancedPaymentDays=0,
                DateInterestWastStoped=DateTime.MinValue,
                DeliquentAmount=0,
                DeliquentDays=0,
                DeliquentInterest=0,
                LoanStructuringDate=DateTime.MinValue,
                DeliquentStatus=LoanDeliquentStatus.Current.ToString(),
                LastDeliquecyProcessedDate=DateTime.MinValue,
                OldBalance=0,
                OldCapital=0,
                OldDueAmount=0,
                OldInterest=0,
                OldPenalty=0,
                OldVAT=0,
                StopInterestCalculation=false,
                StoppedBy="N/A",
                IsUpload = false,
                AccountNumber = loanProduct.AccountNumber,
            };

            return loan;
        }

        public static Loan NewLoanObjectMappring(LoanApplication application, List<LoanAmortization> loanInstallments, Loan Oldloan)
        {
            var Penalty = loanInstallments.FirstOrDefault().Penalty;
            decimal Fee = application.LoanApplicationFees.Sum(x => x.FeeAmount);
            decimal FeePaid = application.LoanApplicationFees.Where(x => x.IsPaid).Sum(x => x.AmountPaid);
            decimal VAT = 0;
            decimal interest = loanInstallments.Sum(x => x.Interest);
            var Balance = loanInstallments.Sum(x => x.Principal);

            var nextInstallment = loanInstallments.FirstOrDefault(x => x.Sno == 2);
            var nextInstallmentDate = nextInstallment?.NextPaymentDate ?? loanInstallments.FirstOrDefault(x => x.Sno == 1).NextPaymentDate;
            if (application.LoanApplicationType==LoanApplicationTypes.Reschedule.ToString())
            {
                Oldloan.LoanDuration = application.LoanDuration;
                Oldloan.LoanStatus = LoanStatus.Rescheduled.ToString();
                Oldloan.LoanStructuringStatus = LoanApplicationTypes.Reschedule.ToString();
                Oldloan.MaturityDate = DateTime.Now.AddMonths(application.LoanDuration);
                Oldloan.NextInstallmentDate = nextInstallmentDate;
                Oldloan.StopInterestCalculation = application.StopInterestCalculation;
                Oldloan.StoppedBy = application.StopInterestCalculation ? application.LoanManager : "N/A";
                Oldloan.DateInterestWastStoped = application.StopInterestCalculation ? BaseUtilities.UtcNowToDoualaTime() : DateTime.MinValue;

            }
            else
            {

                Oldloan.LoanAmount = application.Amount;
                Oldloan.InterestRate = application.InterestRate;
                Oldloan.AccrualInterest = 0;
                Oldloan.DeliquentInterest = 0;
                Oldloan.Balance = application.Amount;
                Oldloan.Tax = 0;
                Oldloan.LoanDuration = application.LoanDuration;
                Oldloan.DueAmount = application.Amount;
                Oldloan.LoanStatus = LoanStatus.Restructured.ToString();
                Oldloan.LoanStructuringStatus = LoanStatus.Restructured.ToString();
                Oldloan.LoanDuration = application.LoanDuration;
                Oldloan.MaturityDate = DateTime.Now.AddMonths(application.LoanDuration);
                Oldloan.NextInstallmentDate = nextInstallmentDate;
                Oldloan.StopInterestCalculation = application.StopInterestCalculation;
                Oldloan.StoppedBy = application.StopInterestCalculation ? application.LoanManager : "N/A";
                Oldloan.DateInterestWastStoped = application.StopInterestCalculation ? BaseUtilities.UtcNowToDoualaTime() : DateTime.MinValue;

            }

            return Oldloan;
        }

        public static SimulateLoanInstallementQuery SimulateLoanInstallementRequest(LoanApplication application, decimal tax, decimal fee, string loanID)
        {
            var loan = new SimulateLoanInstallementQuery
            {
                BankId = application.BankId,
                BranchId = application.BranchId,
                Tax = tax,
                Fee = fee,
                VatRate = application.VatRate,
                Amount = application.Amount,
                InterestCalculationPeriod = application.LoanProduct?.LoanInterestPeriod ?? InterestPeriod.Daily.ToString(),
                InterestRate = application.InterestRate,
                RepaymentStartDate = application.FirstInstallmentDate,
                IsSimulation = false,
                LoanDuration = application.LoanDuration,
                LoanDurationType = application.LoanProduct?.LoanInterestPeriod ?? LoanDurationPeriod.Months.ToString(),
                LoanId = loanID,
                AmortizationType = "",
                RepaymentCycle = application.RepaymentCircle
            };
            return loan;
        }
        public static LoanParameters AddLoanAmortizationCommandRequest(LoanApplication application, decimal tax, decimal fee, string loanID)
        {
            var loan = new LoanParameters
            {
                BankId = application.BankId,
                BranchId = application.BranchId,
                Tax = tax,
                Fee = fee,
                VatRate = application.VatRate,
                Amount = application.Amount,
                InterestCalculationPeriod = application.LoanProduct?.LoanInterestPeriod ?? InterestPeriod.Daily.ToString(),
                InterestRate = application.InterestRate,
                RepaymentStartDate = DateTime.Now.AddMonths(application.GracePeriodBeforeFirstPayment),
                IsSimulation = false,
                LoanDuration = application.LoanDuration,
                LoanDurationType = application.LoanProduct?.LoanDurationPeriod ?? LoanDurationPeriod.Months.ToString(),
                LoanId = loanID,
                AmortizationType = application.AmortizationType,
                RepaymentCycle = application.LoanProduct.LoanProductRepaymentCycles
                    .Where(x => x.Id == application.RepaymentCircle)
                    ?.FirstOrDefault()?.RepaymentCycle ?? "Monthly",
                LoanApplicationId = application.Id,
                NumberOfInstallments = 0,
                Penalty = 0
            };
            return loan;
        }

    }
}
