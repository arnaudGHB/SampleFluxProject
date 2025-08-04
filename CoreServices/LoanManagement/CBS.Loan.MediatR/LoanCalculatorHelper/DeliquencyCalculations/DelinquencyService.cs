using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Data.Enums;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.AuthHelper;
using CBS.NLoan.MediatR.LoanApplicationMediaR.Queries;
using CBS.NLoan.MediatR.LoanDeliquencyConfigurationMediaR.Queries;
using CBS.NLoan.Repository.LoanApplicationP.LoanP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using SixLabors.Fonts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CBS.NLoan.MediatR.LoanCalculatorHelper.DeliquencyCalculations
{


    /// <summary>
    /// Service to process loan delinquencies by calculating overdue days, delinquent amounts, and updating loan statuses.
    /// This service handles all loans that are currently unpaid and processes their delinquency status daily.
    /// </summary>
    public class DelinquencyService : IDelinquencyService
    {
        private readonly ILoanRepository _loanRepository;  // Repository to access loan data.
        private readonly IUnitOfWork<LoanContext> _uow;  // Unit of work to save database changes atomically.
        private readonly PathHelper _pathHelper;  // Helper for accessing system paths and configurations.
        private readonly IMediator _mediator;  // Mediator for sending commands/queries within the system.
        private readonly ILogger<DelinquencyService> _logger;  // Logger to log important events and errors.
        private readonly TokenStorageService _tokenStorageService;

        /// <summary>
        /// Initializes a new instance of the DelinquencyService.
        /// </summary>
        /// <param name="loanRepository">Repository for loan data access.</param>
        /// <param name="uow">Unit of work for committing transactions.</param>
        /// <param name="pathHelper">PathHelper for system configurations.</param>
        /// <param name="mediator">IMediator for handling internal commands/queries.</param>
        /// <param name="logger">Logger for logging events and errors.</param>
        public DelinquencyService(
            ILoanRepository loanRepository,
            IUnitOfWork<LoanContext> uow,
            PathHelper pathHelper,
            IMediator mediator,
            ILogger<DelinquencyService> logger,
            TokenStorageService tokenStorageService)
        {
            _loanRepository = loanRepository;
            _uow = uow;
            _pathHelper = pathHelper;
            _mediator = mediator;
            _logger = logger;
            _tokenStorageService=tokenStorageService;
        }

        /// <summary>
        /// Processes all unpaid loans to calculate and update their delinquency details.
        /// Only loans with outstanding balances are processed, and already processed loans for the current day are skipped.
        /// </summary>
        public async Task ProcessAllLoansAsync()
        {
            // Step 1: Authenticate the service to ensure valid access.
            bool isAuthenticated = await AuthenticationHelper.AuthenticateAsync(_pathHelper, _logger, _tokenStorageService);
            if (!isAuthenticated)
            {
                _logger.LogWarning("Authentication failed. Exiting the delinquency process.");
                return;
            }

          
            try
            {
                // Fetch all unpaid loans that are open, not deleted, and have an outstanding balance.
                var unpaidLoans = await _loanRepository
                    .FindBy(x => x.LoanStatus == LoanStatus.Open.ToString()  // Loan should be open.
                        && x.IsDeleted == false  // The loan should not be deleted.
                        && (x.LoanAmount - x.Paid) > 0)  // The loan should have an outstanding balance.
                    .AsNoTracking()  // Prevent entity tracking to improve read performance.
                    .ToListAsync();

                // Log the total number of loans to be processed.
                _logger.LogInformation("Delinquency calculation started at {StartTime} for {LoanCount} loans.", BaseUtilities.UtcNowToDoualaTime(), unpaidLoans.Count);

                int processedLoans = 0;

                // Filter out loans that have already been processed today.
                var loansToProcess = unpaidLoans
                    .Where(loan => !loan.LastDeliquecyProcessedDate.HasValue  // If no previous process date.
                        || loan.LastDeliquecyProcessedDate.Value.Date != BaseUtilities.UtcNowToDoualaTime().Date)  // Or if the last process date is not today.
                    .ToList();

                // Process each loan in the filtered list.
                foreach (var loan in loansToProcess)
                {
                    await ProcessLoanAsync(loan);  // Call the processing logic for the loan.
                    processedLoans++;  // Increment the processed loans counter.
                }

                // Log the completion of the delinquency process.
                string successMessage = $"Delinquency calculation completed successfully. Processed {processedLoans}/{unpaidLoans.Count} loans.";
                _logger.LogInformation(successMessage);

                // Audit the success.
                await BaseUtilities.LogAndAuditAsync(successMessage, null, HttpStatusCodeEnum.OK, LogAction.LoanDeliquencyProcessingSuccess, LogLevelInfo.Information, _tokenStorageService.GetFullName(), _tokenStorageService.GetToken());
            }
            catch (Exception ex)
            {
                // Log the error and perform an audit of the error.
                string errorMessage = $"Delinquency calculation failed: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.InternalServerError, LogAction.LoanDeliquencyProcessingError, LogLevelInfo.Error, _tokenStorageService.GetFullName(), _tokenStorageService.GetToken());
            }
        }

    

     

        /// <summary>
        /// Processes an individual loan to calculate its delinquency status, overdue days, delinquent amount, and interest.
        /// Updates the loan entity with the calculated values and saves changes to the database.
        /// </summary>
        /// <param name="loan">The loan to process.</param>
        public async Task ProcessLoanAsync(Loan loan)
        {
            var today = BaseUtilities.UtcNowToDoualaTime();  // Get today's date in the Douala timezone.

            // Determine the start date for processing, either the last processed date or the next installment date.
            var startDate = loan.LastDeliquecyProcessedDate ?? loan.LastRefundDate;
            if (startDate >= today)
            {
                _logger.LogInformation("Loan {LoanId} already up-to-date. Skipping.", loan.Id);  // Log and skip if already processed.
                return;
            }

            try
            {


                LoanApplicationDto loanApplication =await GetLoanApplication(loan.LoanApplicationId);

                loan.LoanDuration=loanApplication==null? loan.LoanDuration : loanApplication.LoanDuration;
                // Calculate the daily amount by dividing the loan balance by the loan duration in days.
                decimal dailyAmount = BaseUtilities.RoundUpValue(loan.Balance / (loan.LoanDate.AddMonths(loan.LoanDuration) - loan.LoanDate).Days);

                // Calculate the daily interest rate based on the annual rate and assume 30 days in a month.
                decimal dailyInterestRate = (loan.InterestRate / 100) / 30;

                // Determine whether the loan is delinquent based on the last refund date.
                bool isDelinquent = loan.LastRefundDate == DateTime.MinValue
                    ? (BaseUtilities.UtcNowToDoualaTime() - loan.LoanDate.AddMonths(1)).Days > 0  // If no refunds, check from LoanDate.
                    : (BaseUtilities.UtcNowToDoualaTime() - loan.LastRefundDate.AddMonths(1)).Days > 0;  // Otherwise, check from the last refund.

                if (isDelinquent)
                {
                    // Calculate delinquent days based on the appropriate start date.
                    int delinquentDays = loan.LastRefundDate == DateTime.MinValue
                        ? (BaseUtilities.UtcNowToDoualaTime() - loan.LoanDate.AddMonths(1)).Days
                        : (BaseUtilities.UtcNowToDoualaTime() - loan.LastRefundDate.AddMonths(1)).Days;

                    // Calculate the delinquent amount and interest.
                    loan.DeliquentDays = delinquentDays;
                    loan.DeliquentAmount = BaseUtilities.RoundUpValue(dailyAmount * delinquentDays);
                    loan.DeliquentInterest = BaseUtilities.RoundUpValue(loan.Balance * dailyInterestRate * delinquentDays);
                    loan.IsDeliquentLoan = true;  // Mark the loan as delinquent.
                    loan.DeliquentStatus = LoanDeliquentStatus.Delinquent.ToString();
                    loan.AdvancedPaymentAmount = 0;  // Reset advanced payment details.
                    loan.AdvancedPaymentDays = 0;

                    // Log the delinquency details.
                    _logger.LogInformation("Loan {LoanId} marked as delinquent. Days: {Days}, Amount: {Amount}, Interest: {Interest}",
                        loan.Id, delinquentDays, loan.DeliquentAmount, loan.DeliquentInterest);
                }
                else
                {
                    // Reset delinquency details if the loan is current.
                    loan.DeliquentDays = 0;
                    loan.DeliquentAmount = 0;
                    loan.IsDeliquentLoan = false;
                    loan.DeliquentInterest = 0;
                    loan.DeliquentStatus = LoanDeliquentStatus.Current.ToString();  // Set status as current.

                    // Log that the loan is not delinquent.
                    _logger.LogInformation("Loan {LoanId} is current. No delinquency.", loan.Id);
                }

                // Retrieve the delinquency configuration based on the number of delinquent days.
                var delinquencyRange = await GetDelinquencyRange(loan.DeliquentDays);
                loan.LoanDeliquencyConfigurationId = delinquencyRange?.Id ?? "";

                // Update the last processed date to today.
                loan.LastDeliquecyProcessedDate = today;

                // Save changes to the database.
                _loanRepository.Update(loan);
                await _uow.SaveAsync();
            }
            catch (Exception ex)
            {
                // Log and audit any errors during loan processing.
                _logger.LogError(ex, "Error processing loan {LoanId}. Error: {Message}", loan.Id, ex.Message);
                await BaseUtilities.LogAndAuditAsync($"Error processing loan {loan.Id}: {ex.Message}", loan, HttpStatusCodeEnum.InternalServerError, LogAction.LoanDeliquencyProcessingError, LogLevelInfo.Error, _tokenStorageService.GetFullName(), _tokenStorageService.GetToken());
            }
        }

        /// <summary>
        /// Retrieve loan application  based on the number of Loan Application Id.
        /// </summary>
        /// <param name="LoanApplicationId">loan application Id.</param>
        private async Task<LoanApplicationDto> GetLoanApplication(string LoanApplicationId)
        {
            GetLoanApplicationQuery getLoanApplicationQuery = new()
            {
                Id = LoanApplicationId,
            };// Create query request.
            var response = await _mediator.Send(getLoanApplicationQuery);// Send the request using mediator.
            return response.Success && response.Data != null ? response.Data : null;  // Return Loan Application if found.
        }

        /// <summary>
        /// Retrieves the delinquency configuration based on the number of delinquent days.
        /// The configuration specifies thresholds and penalties for delinquent loans.
        /// </summary>
        /// <param name="delinquencyDays">The number of days the loan has been delinquent.</param>
        private async Task<LoanDeliquencyConfigurationDto> GetDelinquencyRange(int delinquencyDays)
        {
            var request = new GetLoanDeliquencyConfigurationWithDaysCountQuery { days = delinquencyDays };  // Create query request.
            var response = await _mediator.Send(request);  // Send the request using mediator.
            return response.Success && response.Data != null ? response.Data : null;  // Return configuration if found.
        }
    }



    //public class DelinquencyService : IDelinquencyService
    //{
    //    private readonly ILoanRepository _loanRepository;
    //    private readonly IUnitOfWork<LoanContext> _uow;
    //    private readonly PathHelper _pathHelper;
    //    private readonly IMediator _mediator;
    //    private readonly ILogger<DelinquencyService> _logger;

    //    public DelinquencyService(
    //        ILoanRepository loanRepository,
    //        IUnitOfWork<LoanContext> uow,
    //        PathHelper pathHelper,
    //        IMediator mediator,
    //        ILogger<DelinquencyService> logger)
    //    {
    //        _loanRepository = loanRepository;
    //        _uow = uow;
    //        _pathHelper = pathHelper;
    //        _mediator = mediator;
    //        _logger = logger;
    //    }


    //    /// <summary>
    //    /// Processes a loan to calculate delinquency status, overdue days, delinquent amount, and delinquent interest 
    //    /// for each day between the last processed date and today. Updates the loan's status accordingly.
    //    /// </summary>
    //    /// <param name="loan">The loan to process.</param>
    //    /// <returns>A Task representing the asynchronous operation.</returns>
    //    public async Task ProcessLoanAsync(Loan loan)
    //    {
    //        var today = BaseUtilities.UtcNowToDoualaTime();

    //        // Determine the start date for processing, either the last processed date or the next installment date
    //        var startDate = loan.LastDeliquecyProcessedDate ?? loan.NextInstallmentDate;
    //        if (startDate >= today)
    //        {
    //            // If already processed up to today, skip further processing
    //            _logger.LogInformation("Loan {LoanId} already up-to-date. Skipping.", loan.Id);
    //            return;
    //        }

    //        try
    //        {

    //            // Calculate and accumulate delinquent amount (daily principal portion)
    //            if (loan.LoanDuration !=0)
    //            {
    //                // Calculate the daily amount by dividing the loan balance by the total number of days in the loan period (from LoanDate to LoanDate + LoanDuration)
    //                decimal dailyAmount = BaseUtilities.RoundUpValue(loan.Balance / (loan.LoanDate.AddMonths(loan.LoanDuration) - loan.LoanDate).Days);

    //                // Calculate the daily interest rate based on the annual interest rate (assuming 30-day months for simplicity)
    //                var dailyInterestRate = (loan.InterestRate / 100) / 30; // Assumes 30-day month

    //                // Check if the loan is delinquent. If LastRefundDate is DateTime.MinValue, calculate delinquent days from LoanDate; otherwise, from LastRefundDate
    //                bool isDiffDays = loan.LastRefundDate == DateTime.MinValue ? (DateTime.Now - loan.LoanDate.AddMonths(1)).Days > 0 : (DateTime.Now - loan.LastRefundDate.AddMonths(1)).Days > 0;

    //                // If the loan is delinquent, perform the calculations for delinquent days, amount, and interest
    //                if (isDiffDays)
    //                {
    //                    // If LastRefundDate is not set (it's DateTime.MinValue), calculate delinquent amounts from LoanDate
    //                    if (loan.LastRefundDate == DateTime.MinValue)
    //                    {
    //                        loan.DeliquentDays = (DateTime.Now - loan.LoanDate.AddMonths(1)).Days; // Set the number of delinquent days
    //                        loan.DeliquentAmount = BaseUtilities.RoundUpValue(dailyAmount * (DateTime.Now - loan.LoanDate).Days); // Accumulate delinquent amount
    //                        loan.DeliquentInterest = BaseUtilities.RoundUpValue(loan.Balance * dailyInterestRate * (DateTime.Now - loan.LoanDate).Days); // Accumulate delinquent interest
    //                    }
    //                    else
    //                    {
    //                        // If LastRefundDate is set, calculate delinquent amounts from LastRefundDate
    //                        loan.DeliquentDays = (DateTime.Now - loan.LastRefundDate.AddMonths(1)).Days; // Set the number of delinquent days
    //                        loan.DeliquentAmount = BaseUtilities.RoundUpValue(dailyAmount * (DateTime.Now - loan.LastRefundDate).Days); // Accumulate delinquent amount
    //                        loan.DeliquentInterest = BaseUtilities.RoundUpValue(loan.Balance * dailyInterestRate * (DateTime.Now - loan.LastRefundDate).Days); // Accumulate delinquent interest
    //                    }

    //                    // Mark the loan as delinquent and reset any advanced payment details
    //                    loan.IsDeliquentLoan = true;
    //                    loan.AdvancedPaymentAmount = 0;
    //                    loan.AdvancedPaymentDays = 0;

    //                    // Set the delinquent status of the loan
    //                    loan.DeliquentStatus = LoanDeliquentStatus.Delinquent.ToString();
    //                }
    //                else
    //                {
    //                    // If the loan is not delinquent (current), reset all delinquent-related fields
    //                    loan.DeliquentDays = 0; // No delinquent days
    //                    loan.DeliquentAmount = 0; // No delinquent amount
    //                    loan.IsDeliquentLoan = false; // The loan is not delinquent
    //                    loan.DeliquentInterest = 0; // No delinquent interest

    //                    // Set the loan status as current
    //                    loan.DeliquentStatus = LoanDeliquentStatus.Current.ToString();

    //                    // Log that the loan is current and not delinquent
    //                    _logger.LogInformation("Loan {LoanId} is current. No delinquency.", loan.Id);
    //                }


    //                _logger.LogInformation("Getting the delinquency range by loanId .", loan.Id);
    //                var deliquencyRange = await getDeliquencyRange(loan.DeliquentDays);
    //                loan.LoanDeliquencyConfigurationId = deliquencyRange != null ? deliquencyRange.Id : "";

    //                // Log the overall delinquency results for the loan
    //                _logger.LogInformation(
    //                    "Processed loan {LoanId}: Total Days Overdue: {TotalDaysOverdue}, Total Delinquent Amount: {TotalDelinquentAmount}, Total Delinquent Interest: {TotalDelinquentInterest}.",
    //                    loan.Id, loan.DeliquentDays, loan.DeliquentAmount, loan.DeliquentInterest);

    //                // Calculate any advanced payment made by the borrower
    //                loan.AdvancedPaymentAmount = Math.Max(0, loan.Paid - loan.Principal);

    //                // Update the last processed date to today
    //                loan.LastDeliquecyProcessedDate = today;

    //                // Save the updated loan details to the repository
    //                _loanRepository.Update(loan);
    //                await _uow.SaveAsync();
    //            }


    //           /* }
    //            else
    //            {
    //                _logger.LogInformation($"delinquency already calculated for loan of Id : {loan.Id}");

    //            }*/

    //        }
    //        catch (Exception ex)
    //        {
    //            // Log any errors that occur during loan processing
    //            _logger.LogError(ex, "Error processing loan {LoanId}. Error: {Message}", loan.Id, ex.Message);
    //        }
    //    }

    //    private async Task< LoanDeliquencyConfigurationDto> getDeliquencyRange(int delinquencyDays)
    //    {
    //        GetLoanDeliquencyConfigurationWithDaysCountQuery request = new()
    //        {
    //            days = delinquencyDays
    //        };

    //        var response =await _mediator.Send(request);
    //        if(response.Success && response.Data != null)
    //        {
    //            return response.Data;
    //        }
    //        return null;
    //    }

    //    private decimal calculatePenaltyAmount(int diffDays)
    //    {
    //        return 0;
    //    }


    //    /// <summary>
    //    /// Processes all unpaid loans to calculate and update their delinquency details.
    //    /// </summary>
    //    public async Task ProcessAllLoansAsync()
    //    {
    //        try
    //        {
    //            // Fetch all unpaid loans that are not deleted
    //            var unpaidLoans = await _loanRepository
    //                .FindBy(x => x.LoanStatus == LoanStatus.Open.ToString()
    //                    && x.IsDeleted == false
    //                    && (x.LoanAmount - x.Paid) > 0)
    //                .AsNoTracking()
    //                .ToListAsync();

    //            _logger.LogInformation("Delinquency calculation started at {StartTime} for {LoanCount} loans.", DateTime.Now, unpaidLoans.Count);

    //            int processedLoans = 0;

    //            var unpaidLoansNotOfToday = unpaidLoans.Where(loan => !loan.LastDeliquecyProcessedDate.HasValue ||(loan.LastDeliquecyProcessedDate.HasValue &&  (loan.LastDeliquecyProcessedDate.Value.Date - DateTime.Now.Date).Days != 0)).ToList();

    //            foreach (var loan in unpaidLoansNotOfToday)
    //            {

    //                // Process only if it has not been processed today
    //                /* if (loan.LastDeliquecyProcessedDate != DateTime.Today)
    //                 {*/
    //                await ProcessLoanAsync(loan);
    //                processedLoans++;
    //                //  }
    //            }

    //            _logger.LogInformation("Delinquency calculation completed successfully. Processed {ProcessedLoans}/{TotalLoans} loans.",
    //                processedLoans, unpaidLoans.Count);
    //        }
    //        catch (Exception ex)
    //        {
    //            var errorMessage = $"Delinquency calculation failed: {ex.Message}";
    //            _logger.LogError(ex, errorMessage);
    //        }
    //    }
    //}

}
