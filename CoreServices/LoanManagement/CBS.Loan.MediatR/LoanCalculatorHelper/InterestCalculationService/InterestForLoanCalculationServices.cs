using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Entity.InterestCalculationP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Data.Enums;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.CustomerAccount.Command;
using CBS.NLoan.MediatR.LoanCalculatorHelper.InterestCalculationService;
using CBS.NLoan.MediatR.SMS.Command;
using CBS.NLoan.Repository.AlertProfileP;
using CBS.NLoan.Repository.InterestCalculationP;
using CBS.NLoan.Repository.LoanApplicationP;
using CBS.NLoan.Repository.LoanApplicationP.LoanAmortizationP;
using CBS.NLoan.Repository.LoanApplicationP.LoanP;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
public class InterestForLoanCalculationServices : IInterestForLoanCalculationServices
{
    private readonly ILoanRepository _loanRepository;
    private readonly IUnitOfWork<LoanContext> _uow;
    private readonly ILoanAmortizationRepository _loanAmortizationRepository;
    private readonly ILogger<InterestForLoanCalculationServices> _logger;
    private readonly IDailyInterestCalculationRepository _dailyInterestCalculationRepository;
    private readonly PathHelper _pathHelper;
    private readonly IMediator _mediator;

    private readonly ILoanApplicationRepository _loanApplicationRepository;
    private readonly IAlertProfileRepository _alertProfileRepository;
    public InterestForLoanCalculationServices(
        ILoanRepository loanRepository,
        IUnitOfWork<LoanContext> uow,
        ILoanAmortizationRepository loanAmortizationRepository,
        ILogger<InterestForLoanCalculationServices> logger,
        IMediator mediator,
        IDailyInterestCalculationRepository dailyInterestCalculationRepository,
        PathHelper pathHelper,
        ILoanApplicationRepository loanApplicationRepository,
        IAlertProfileRepository alertProfileRepository = null)
    {
        _loanRepository = loanRepository;
        _uow = uow;
        _loanAmortizationRepository = loanAmortizationRepository;
        _logger = logger;
        _mediator = mediator;
        _dailyInterestCalculationRepository = dailyInterestCalculationRepository;
        _pathHelper = pathHelper;
        _loanApplicationRepository = loanApplicationRepository;
        _alertProfileRepository = alertProfileRepository;
    }

    private async Task CalculateDailyInterestAsync(Loan loan, string token)
    {
        _logger.LogInformation("Starting daily interest calculation for loan ID: {LoanId}", loan.Id);
        decimal previousBalance = loan.Balance;
        try
        {
            // 1. Ensure the loan duration is set
            if (loan.LoanDuration == 0)
            {
                var loanApplication = await _loanApplicationRepository.FindAsync(loan.LoanApplicationId);
                loan.LoanDuration = loanApplication.LoanDuration;
            }

            // 2. Calculate the daily interest rate
            decimal dailyInterestRate = loan.InterestRate / 100;
            _logger.LogInformation("Daily interest rate calculated: {DailyInterestRate}", dailyInterestRate);

            // 3. Determine the last interest calculation date
            DateTime lastInterestCalculationDate = loan.LastInterestCalculatedDate.Date == DateTime.MinValue.Date ? loan.DisbursementDate.Date : loan.LastInterestCalculatedDate.Date;

            // 4. Get the current date in UTC
            DateTime today = BaseUtilities.UtcNowToDoualaTime();
            int daysToCalculate = (today - lastInterestCalculationDate).Days;

            _logger.LogInformation("Today’s Date: {TodayDate}", BaseUtilities.UtcNowToDoualaTime());
            _logger.LogInformation("Last Interest Calculation Date: {LastInterestCalculationDate}, LoanId: {LoanId}", lastInterestCalculationDate, loan.Id);
            _logger.LogInformation("Days to Calculate: {DaysToCalculate}", daysToCalculate);

            // 5. Skip calculation if already done for today
            if (daysToCalculate == 0)
            {
                _logger.LogInformation("Interest already calculated for today for loan ID: {LoanId}", loan.Id);
                return;
            }

            // 6. Initialize variables for interest and VAT calculations
            decimal totalCalculatedInterest = 0;
            decimal totalCalculatedVat = 0;
            decimal loanPrincipal = loan.LoanAmount - loan.Paid;

            // 7. If there's a principal balance, calculate the interest
            if (loanPrincipal > 0)
            {
                for (int i = 0; i < daysToCalculate; i++)
                {
                    // 8. Calculate daily interest
                    decimal dailyInterest = (loanPrincipal * dailyInterestRate) / 30;
                    _logger.LogInformation("Daily interest calculated: {DailyInterest}", BaseUtilities.FormatCurrency(dailyInterest));
                    dailyInterest = BaseUtilities.RoundUpValue(dailyInterest, 0);

                    // 9. Handle cases where interest is less than or equal to zero
                    if (dailyInterest <= 0)
                    {
                        HandleZeroOrNegativeInterest(loan, dailyInterest);
                        return;
                    }
                    // 9. Handle cases where interest is less than or equal to zero
                    if (loan.VatRate < 0)
                    {
                        loan.VatRate = 0;
                        loan.LastCalculatedInterest = 0;
                        HandleZeroOrNegativeInterest(loan, dailyInterest);
                        return;
                    }

                    // 10. Calculate VAT on the interest
                    decimal vatOnInterest = dailyInterest * (loan.VatRate / 100);
                    vatOnInterest = BaseUtilities.RoundUpValue(vatOnInterest, 0);
                    _logger.LogInformation("Daily VAT calculated: {VatOnInterest}", BaseUtilities.FormatCurrency(vatOnInterest));

                    // 11. Accumulate interest and VAT
                    loan.AccrualInterest += dailyInterest;
                    loan.Tax += vatOnInterest;
                    loan.DueAmount += dailyInterest + vatOnInterest;
                    totalCalculatedInterest += dailyInterest;
                    totalCalculatedVat += vatOnInterest;
                    loan.LastCalculatedInterest = BaseUtilities.RoundUpValue(dailyInterest);
                }
           
                // 12. Update loan details if interest was calculated
                if (totalCalculatedInterest > 0)
                {
                    await UpdateLoanDetailsAsync(loan, previousBalance, totalCalculatedInterest, totalCalculatedVat, token);
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError($"Error occurred while calculating daily interest for loan ID: {loan.Id}: {e.Message}");
        }
    }

    private void HandleZeroOrNegativeInterest(Loan loan, decimal dailyInterest)
    {
        if (loan.InterestRate == 0)
        {
            loan.AccrualInterest = 0;
            loan.Balance = loan.LoanAmount - loan.Paid;
            _loanRepository.Update(loan);
            _uow.SaveAsync().Wait();
        }

        _logger.LogInformation("Interest is less than 0 OR negative. loan ID: {loan.Id}, Interest Rate: {dailyInterest}, Interest calculated: {dailyInterest}", loan.Id, loan.InterestRate, dailyInterest);
    }

    private async Task UpdateLoanDetailsAsync(Loan loan, decimal previousBalance, decimal totalCalculatedInterest, decimal totalCalculatedVat, string token)
    {
        loan.AccrualInterest = BaseUtilities.RoundUpValue(loan.AccrualInterest);
        loan.Tax = BaseUtilities.RoundUpValue(loan.Tax);
        loan.Balance = BaseUtilities.RoundUpValue(loan.Balance);
        loan.DueAmount = BaseUtilities.RoundUpValue(loan.DueAmount);
        loan.LastInterestCalculatedDate = BaseUtilities.UtcNowToDoualaTime();
        _loanRepository.Update(loan);

        var dailyInterestCalculation = new DailyInterestCalculation
        {
            BranchId = loan.BranchId,
            CustomerId = loan.CustomerId,
            CustomerName = loan.CustomerName,
            Date = BaseUtilities.UtcNowToDoualaTime(),
            Id = BaseUtilities.GenerateUniqueNumber(),
            InterestCalculated = BaseUtilities.RoundUpValue(totalCalculatedInterest),
            InterestRate = loan.InterestRate,
            LoanAmount = loan.LoanAmount,
            LoanId = loan.Id,
            Fines = 0,  
            DueAmount = loan.DueAmount,
            NewBalance = loan.Balance,
            CalculatedVat = BaseUtilities.RoundUpValue(totalCalculatedVat),
            VatRate = loan.VatRate,
            PreviousBalance = previousBalance,
        };
        _dailyInterestCalculationRepository.Add(dailyInterestCalculation);

        //if (loan.Balance < 0)
        //{
        //    loan.Balance = (loan.Paid - loan.LoanAmount) + loan.AccrualInterest;
        //}

        await _uow.SaveAsync();

        _logger.LogInformation($"Daily interest calculation completed for Loan Service: loan ID: {loan.Id}, New balance: {loan.Balance}");

        //decimal totalInterest = dailyInterestCalculation.InterestCalculated + dailyInterestCalculation.CalculatedVat + dailyInterestCalculation.Fines;

        //var updateLoanAccountBalanceCommand = new UpdateLoanAccountBalanceCommand
        //{
        //    CustomerId = loan.CustomerId,
        //    Balance = -(loan.DueAmount),
        //    Token = token,
        //    Interest = -(totalInterest),
        //    LoanId = loan.Id,
        //    ExternalReference = dailyInterestCalculation.Id
        //};
        //var updateLoanAccountBalanceCommandResult = await _mediator.Send(updateLoanAccountBalanceCommand);

        //if (updateLoanAccountBalanceCommandResult.StatusCode != 200)
        //{
        //    _logger.LogInformation($"Loan Account Update Failed. API response message: {updateLoanAccountBalanceCommandResult.Message}");
        //}
        //else
        //{
        //    _logger.LogInformation($"Loan Account Updated Successfully. API response message: {updateLoanAccountBalanceCommandResult.Message}");
        //}

    }




    private async Task SendSMSInit(int Members, bool IsInit, string token)
    {
        var profiles = await _alertProfileRepository.All.AsNoTracking().ToListAsync();

        string msg = $"Interest calculation started at {DateTime.Now}. Accounts to be treated: {Members}.\nEnvironment: {_pathHelper.SMSEnvironment}";
        if (!IsInit)
        {
            msg = $"Interest calculation completed at {DateTime.Now}. Accounts treated: {Members}. Next Calculation time is at {DateTime.Now.AddDays(1)}.\nEnvironment: {_pathHelper.SMSEnvironment}";
        }

        foreach (var profile in profiles)
        {
            if (profile.ActiveStatus)
            {
                if (profile.SendSMS)
                {
                    var sMSPICallCommand2 = new SendSMSPICallCommand
                    {
                        messageBody = msg,
                        Token = token,
                        recipient = BaseUtilities.Add237Prefix(profile.Msisdn),
                    };
                    await _mediator.Send(sMSPICallCommand2);
                }

            }

        }


    }


    public void CalculateAndUpdateInterest(string loanId)
    {
        // Retrieve the loan from the database
        var loan = GetLoanById(loanId);

        if (loan == null)
        {
            throw new Exception("Loan not found.");
        }

        // Retrieve the current installment (amortization record)
        var amortization = GetCurrentLoanAmortization(loanId);

        if (amortization == null)
        {
            throw new Exception("Amortization record not found.");
        }

        DateTime currentDate = DateTime.Now;
        DateTime installmentDueDate = amortization.DateOfPayment.AddDays(30);

        // Check if the installment is due
        if (installmentDueDate <= currentDate)
        {
            // Calculate daily interest rate
            decimal dailyInterestRate = loan.InterestRate / 100 / 30;
            int daysElapsed = (currentDate - loan.LastInterestCalculatedDate).Days;

            // Calculate the interest for the elapsed days
            decimal dailyInterest = amortization.PrincipalBalance * dailyInterestRate;
            decimal totalInterest = dailyInterest * daysElapsed;

            // Calculate VAT on interest
            decimal vat = (totalInterest * 19.25m) / 100;

            // Update the amortization record
            amortization.Interest += totalInterest;
            amortization.Tax += vat;
            amortization.TotalDue = amortization.Principal + amortization.Interest + amortization.Tax;
            amortization.InterestBalance += totalInterest;
            amortization.Balance = amortization.PrincipalBalance + amortization.InterestBalance;
            amortization.InterestDue = totalInterest;

            // Update loan fields
            loan.AccrualInterest += totalInterest;
            loan.AccrualInterestPaid += vat;
            loan.LastCalculatedInterest = totalInterest;
            loan.LastInterestCalculatedDate = currentDate;

            // Move to next installment if the current one is not fully paid
            if (amortization.Paid < amortization.TotalDue)
            {
                amortization.PreviousInstallmentDue = true;
                loan.DeliquentInterest += amortization.Interest;
                loan.DueAmount += amortization.TotalDue - amortization.Paid;
                amortization.InterestDue = amortization.Interest;
            }
            else
            {
                amortization.PreviousInstallmentDue = false;
            }

            // Check for fines after 60 days of overdue
            if (installmentDueDate.AddDays(30) <= currentDate && amortization.Paid < amortization.TotalDue)
            {
                decimal fine = (amortization.InterestDue * 0.05m); // Assuming 5% fine
                amortization.Penalty = fine;
                amortization.PenaltyPaid += fine;
                loan.Penalty += fine;
                loan.PenaltyPaid += fine;
            }

            // Save the changes back to the database
            UpdateLoan(loan);
            UpdateLoanAmortization(amortization);
        }
        else
        {
            // If the installment is not due yet, calculate normal daily interest
            decimal dailyInterestRate = loan.InterestRate / 100 / 30;
            int daysElapsed = (currentDate - loan.LastInterestCalculatedDate).Days;

            // Calculate the interest for the elapsed days
            decimal dailyInterest = amortization.PrincipalBalance * dailyInterestRate;
            decimal totalInterest = dailyInterest * daysElapsed;

            // Calculate VAT on interest
            decimal vat = (totalInterest * 19.25m) / 100;

            // Update the amortization record
            amortization.Interest += totalInterest;
            amortization.Tax += vat;
            amortization.TotalDue = amortization.Principal + amortization.Interest + amortization.Tax;
            amortization.InterestBalance += totalInterest;
            amortization.Balance = amortization.PrincipalBalance + amortization.InterestBalance;
            amortization.InterestDue = totalInterest;

            // Update loan fields
            loan.AccrualInterest += totalInterest;
            loan.AccrualInterestPaid += vat;
            loan.LastCalculatedInterest = totalInterest;
            loan.LastInterestCalculatedDate = currentDate;

            // Save the changes back to the database
            UpdateLoan(loan);
            UpdateLoanAmortization(amortization);
        }
    }

    // Placeholder methods to retrieve and update loan and amortization records
    private Loan GetLoanById(string loanId)
    {
        // Implement database retrieval logic here
        return new Loan(); // Placeholder
    }

    private LoanAmortization GetCurrentLoanAmortization(string loanId)
    {
        // Implement database retrieval logic here
        return new LoanAmortization(); // Placeholder
    }

    private void UpdateLoan(Loan loan)
    {
        // Implement database update logic here
    }

    private void UpdateLoanAmortization(LoanAmortization amortization)
    {
        // Implement database update logic here
    }

   
    public async Task CalculateDailyInterestAndUpdateAsync(CancellationToken stoppingToken)
    {
        AuthRequest auth = new AuthRequest(_pathHelper.UserName, _pathHelper.Password);
        string stringifyData = JsonConvert.SerializeObject(auth);
        _logger.LogInformation("LMS, Authentication started at {StartTime} for loans microservice.", DateTime.Now);

        var serviceResponse = await APICallHelper.AuthenthicationAuto<ServiceResponse<LoginDto>>(_pathHelper, stringifyData);
        if (serviceResponse.StatusCode != 200)
        {
            _logger.LogInformation($"Authentication failed. at {DateTime.Now}");
            return;
        }

        _logger.LogInformation($"LMS, Authentication was successful. at {DateTime.Now}");
        string token = serviceResponse.Data.bearerToken;

        try
        {
            var unpaidLoans = await _loanRepository.FindBy(x => x.LoanStatus == LoanStatus.Open.ToString() && x.InterestRate>0 && x.IsDeleted == false && (x.LoanAmount - x.Paid) > 0).AsNoTracking().ToListAsync();
            _logger.LogInformation("Interest calculation started at {StartTime} for {LoanCount} loans microservice.", DateTime.Now, unpaidLoans.Count);
            await SendSMSInit(unpaidLoans.Count(), true, token);
            int i = 0;
            foreach (var loan in unpaidLoans)
            {
                await CalculateDailyInterestAsync(loan, token);
                i++;
            }
            _logger.LogInformation("Interest calculation completed successfully.");
            await SendSMSInit(i, false, token);
        }
        catch (Exception ex)
        {
            var errorMessage = $"Interest calculation failed with error: {ex.Message}";
            _logger.LogError(errorMessage);
        }
    }


}


