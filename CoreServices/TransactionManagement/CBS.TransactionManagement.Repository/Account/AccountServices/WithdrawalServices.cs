using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.ChargesWaivedP;
using CBS.TransactionManagement.Data.Dto.FeeP;
using CBS.TransactionManagement.Data.Dto.Transaction;
using CBS.TransactionManagement.Data.Entity.AccountingDayOpening;
using CBS.TransactionManagement.Data.Entity.RemittanceP;
using CBS.TransactionManagement.Data.Entity.SavingProductFeeP;
using CBS.TransactionManagement.Data.Entity.WithdrawalNotificationP;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Helper.Helper;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Repository.ChargesWaivedP;
using CBS.TransactionManagement.Repository.HolyDayP;
using CBS.TransactionManagement.Repository.WithdrawalNotificationP;
using DocumentFormat.OpenXml.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CBS.TransactionManagement.Repository.AccountServices
{
    public class WithdrawalServices : IWithdrawalServices
    {
        private readonly ITellerOperationRepository _tellerOperationRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IChargesWaivedRepository _chargesWaivedRepository;
        private readonly ISavingProductFeeRepository _savingProductFeeRepository;
        private readonly IWithdrawalNotificationRepository _withdrawalNotificationRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly ISubTellerProvisioningHistoryRepository _subTellerProvioningHistoryRepository; // Repository for accessing teller data.
        private readonly IHolyDayRepository _holyDayRepository;

        public IMediator _mediator { get; set; }
        private readonly ITransactionRepository _TransactionRepository; // Repository for accessing Transaction data.
        private readonly ILogger<AccountRepository> _logger; // Logger for logging handler actions and errors.

        public WithdrawalServices(ITellerOperationRepository tellerOperationRepository = null, IMediator mediator = null, ITransactionRepository transactionRepository = null, ILogger<AccountRepository> logger = null, IAccountRepository accountRepository = null, IUnitOfWork<TransactionContext> uow = null, UserInfoToken userInfoToken = null, IChargesWaivedRepository chargesWaivedRepository = null, IWithdrawalNotificationRepository withdrawalNotificationRepository = null, ISavingProductFeeRepository savingProductFeeRepository = null, ISubTellerProvisioningHistoryRepository subTellerProvioningHistoryRepository = null, IHolyDayRepository holyDayRepository = null)
        {
            _tellerOperationRepository = tellerOperationRepository;
            _mediator = mediator;
            _TransactionRepository = transactionRepository;
            _logger = logger;
            _accountRepository = accountRepository;
            _uow = uow;
            _userInfoToken = userInfoToken;
            _chargesWaivedRepository = chargesWaivedRepository;
            _withdrawalNotificationRepository = withdrawalNotificationRepository;
            _savingProductFeeRepository = savingProductFeeRepository;
            _subTellerProvioningHistoryRepository = subTellerProvioningHistoryRepository;
            _holyDayRepository = holyDayRepository;
        }

        public async Task<TransactionDto> Withdrawal(Teller teller, Account tellerAccount, BulkDeposit request, bool isInterBranchOperation, string sourceBranchId, string destinationBranchId, decimal customCharge, string Reference, Account customerAccount, Config config, bool IsOtherCashInOperation, DateTime accountingDate, bool isRemittance, Remittance remittance)
        {
            try
            {
                if (customerAccount.AccountType == AccountType.MobileMoneyMTN.ToString() || customerAccount.AccountType == AccountType.MobileMoneyORANGE.ToString())
                {

                    // Log an error indicating insufficient balance for physical person
                    var errorMessage = $"The cashout treatement of {customerAccount.AccountType} must be done as other cash operations. Go to CashDesk>>O Cash Operations>>Mobile Money. You can contact your administrator for assistance.";
                    _logger.LogError(errorMessage);
                    // Log an audit trail for the error
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 403, _userInfoToken.Token);
                    // Throw an exception indicating insufficient balance for physical person
                    throw new InvalidOperationException(errorMessage);



                }
                else
                {
                    string interOperationType = CurrencyNotesMapper.DetermineOperationType(isInterBranchOperation, request.OperationType);
                    if (!IsOtherCashInOperation)
                    {
                        // Retrieve withdrawal limit for the customer account
                        var depositLimit = await GetWithdrawalLimit(customerAccount.Product.WithdrawalParameters.ToList(), request.Amount, interOperationType);

                        // Check if there is any withdrawal notification for the customer account
                        var notification = await _withdrawalNotificationRepository.GetWithdrawalNotification(customerAccount.CustomerId, customerAccount.AccountNumber, request.Amount);

                        // Check if there are any charges waived for the customer account
                        var chargesWaived = await GetWhitelistedWaiver(customerAccount.CustomerId);

                        // Create transaction history for customer
                        var transactionEntity = await CreateTransactionHistory(request, customerAccount, teller, tellerAccount, depositLimit, request.Customer, isInterBranchOperation, sourceBranchId, destinationBranchId, customCharge, Reference, notification, chargesWaived, config, IsOtherCashInOperation, false, accountingDate, isRemittance, remittance);

                        // Map the Transaction entity to TransactionDto
                        var transactionDto = CurrencyNotesMapper.MapTransactionToDto(transactionEntity, request.currencyNotes, _userInfoToken);

                        // Update Teller property in TransactionDto
                        transactionDto.Teller = teller;

                        // Return TransactionDto with a success response
                        return transactionDto;
                    }
                    else
                    {
                        // Retrieve withdrawal limit for the customer account
                        var depositLimit = await GetWithdrawalLimit(customerAccount.Product.WithdrawalParameters.ToList(), request.Amount, interOperationType);

                        // Check if there is any withdrawal notification for the customer account
                        var notification = new WithdrawalNotification { };

                        // Check if there are any charges waived for the customer account
                        var chargesWaived = await GetWhitelistedWaiver(customerAccount.CustomerId);

                        // Create transaction history for customer
                        var transactionEntity = await CreateTransactionHistory(request, customerAccount, teller, tellerAccount, depositLimit, request.Customer, isInterBranchOperation, sourceBranchId, destinationBranchId, customCharge, Reference, notification, chargesWaived, config, IsOtherCashInOperation, false, accountingDate, isRemittance, remittance);

                        // Map the Transaction entity to TransactionDto
                        var transactionDto = CurrencyNotesMapper.MapTransactionToDto(transactionEntity, request.currencyNotes, _userInfoToken);

                        // Update Teller property in TransactionDto
                        transactionDto.Teller = teller;

                        // Return TransactionDto with a success response
                        return transactionDto;
                    }

                }

            }
            catch (Exception ex)
            {
                // Handle exceptions
                _logger.LogError($"Error occurred while processing withdrawal: {ex.Message}");
                throw; // Rethrow the exception for handling at a higher level
            }
        }
        private async Task<Transaction> CreateTransactionHistory(BulkDeposit request, Account account, Teller teller, Account tellerAccount, WithdrawalParameter withdrawalParameter, CustomerDto customer, bool isInterbranch, string sourceBranchId, string destinationBranchId, decimal customCharge, string Reference, WithdrawalNotification notification, ChargesWaived chargesWaived, Config config, bool IsOtherCashInOperation, bool isMobileMoneyOperation, DateTime accountingDate, bool isRemittance, Remittance remittance)
        {
            try
            {
                bool isMinorAccount = customer.AgeCategoryStatus==AgeCategoryStatus.Minor.ToString() ? true : false;

                if (!IsOtherCashInOperation && !isMobileMoneyOperation)
                {
                    WithdrawalCharges withdrawalCharges = new WithdrawalCharges();
                    // Calculate total amount to withdraw with charges
                    decimal totalAmountToWithdraw = 0;

                    if (!isRemittance)
                    {
                        // Calculate withdrawal charges
                        withdrawalCharges = await CalculateCharges(request.Amount, withdrawalParameter, customCharge, notification, chargesWaived, config, customer.LegalForm, accountingDate, isMinorAccount);
                        
                        // Calculate total amount to withdraw with charges
                        totalAmountToWithdraw = CalculateAmountWithCharges(request.Amount, withdrawalCharges.TotalCharges, customCharge, request.IsChargesInclussive);
                        // Check if the account balance is sufficient for withdrawal
                        _accountRepository.CheckBalanceForWithdrawalWithCharges(request.Amount, account, withdrawalParameter, totalAmountToWithdraw, customer.LegalForm);

                    }
                    else if (isRemittance)
                    
                    {
                        customCharge=0;
                        // Calculate withdrawal charges
                        withdrawalCharges = new WithdrawalCharges { TotalCharges=customCharge, ServiceCharge=customCharge };
                        if (remittance.ChargeType==ChargeType.Inclussive.ToString())
                        {
                            // Calculate total amount to withdraw with charges
                            totalAmountToWithdraw = remittance.InitailAmount-remittance.Fee;
                            request.Amount=totalAmountToWithdraw;

                        }
                   
                    }
                    // Check if the charge is an offline charge
                    bool isOfflineCharge = customCharge > 0;


                    // Check Teller Account Balance
                    await _accountRepository.CheckTellerBalance(teller, request.Amount);

                    // Check if account balance integrity is maintained
                    IsAccountBalanceIntegrity(account);

                    // Create transaction history for customer
                    Transaction transaction = CreateTransaction(request, account, teller, withdrawalParameter, withdrawalCharges, sourceBranchId, destinationBranchId, isInterbranch, Reference, isOfflineCharge, totalAmountToWithdraw, false, accountingDate, isRemittance, remittance);

                    // Add the new Transaction entity to the repository
                    _TransactionRepository.Add(transaction);

                    // Update Teller Account balances
                    UpdateTellerAccountBalance(teller, tellerAccount, transaction, Events.WithDrawalCharges_Fee.ToString(), isInterbranch, sourceBranchId, destinationBranchId, customCharge, customer.Name, accountingDate, request.IsChargesInclussive, isRemittance);

                    // Update customer account
                    UpdateCustomerAccount(account, transaction, customer);

                    // Update withdrawal notification
                    if (notification != null)
                    {
                      _withdrawalNotificationRepository.UpdateWithdrawalNotification(notification, transaction, teller, accountingDate);
                    }

                    // Update charges waived
                    if (chargesWaived != null)
                    {
                        UpdateChargeWaiver(chargesWaived, transaction, withdrawalCharges.NormalCharge, teller);
                    }

                    return transaction;
                }
                else
                {
                    if (!isMobileMoneyOperation)
                    {
                        // Calculate withdrawal charges
                        WithdrawalCharges withdrawalCharges = new WithdrawalCharges();

                        // Calculate total amount to withdraw with charges
                        decimal totalAmountToWithdraw = CalculateAmountWithCharges(request.Amount, withdrawalCharges.TotalCharges, customCharge, request.IsChargesInclussive);

                        // Check if the charge is an offline charge
                        bool isOfflineCharge = customCharge > 0;

                        // Check if the account balance is sufficient for withdrawal
                       _accountRepository.CheckBalanceForWithdrawalWithCharges(request.Amount, account, withdrawalParameter, totalAmountToWithdraw, customer.LegalForm);

                        // Check if account balance integrity is maintained
                        IsAccountBalanceIntegrity(account);

                        // Create transaction history for customer
                        Transaction transaction = CreateTransaction(request, account, teller, withdrawalParameter, withdrawalCharges, sourceBranchId, destinationBranchId, isInterbranch, Reference, isOfflineCharge, totalAmountToWithdraw, false, accountingDate, isRemittance, remittance);

                        // Add the new Transaction entity to the repository
                        _TransactionRepository.Add(transaction);

                        // Update customer account
                        UpdateCustomerAccount(account, transaction, customer);

                        return transaction;
                    }
                    else
                    {
                        // Calculate withdrawal charges
                        WithdrawalCharges withdrawalCharges = new WithdrawalCharges();

                        // Calculate total amount to withdraw with charges
                        decimal totalAmountToWithdraw = CalculateAmountWithCharges(request.Amount, withdrawalCharges.TotalCharges, customCharge, request.IsChargesInclussive);

                        // Check if the charge is an offline charge
                        bool isOfflineCharge = customCharge > 0;

                        // Create transaction history for customer
                        Transaction transaction = CreateTransaction(request, account, teller, withdrawalParameter, withdrawalCharges, sourceBranchId, destinationBranchId, isInterbranch, Reference, isOfflineCharge, totalAmountToWithdraw, isMobileMoneyOperation, accountingDate, isRemittance, remittance);

                        // Add the new Transaction entity to the repository
                        _TransactionRepository.Add(transaction);

                        // Update customer account
                        UpdateCustomerAccount(account, transaction, customer);
                        // Update customer account
                        UpdateTellerAccountBalance(teller, tellerAccount, transaction, TransactionType.WITHDRAWAL_MOBILEMONEY.ToString(), isInterbranch, sourceBranchId, destinationBranchId, customCharge, customer.Name, accountingDate, request.IsChargesInclussive, isRemittance);

                        return transaction;
                    }

                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                _logger.LogError($"Error occurred while creating transaction history: {ex.Message}");
                throw; // Rethrow the exception for handling at a higher level
            }
        }


        private decimal CalculateAmountWithCharges(decimal amount, decimal Charges, decimal customAmount, bool IsChargesInclussive)
        {
            if (IsChargesInclussive)
            {
                decimal amountWithCharges = amount + customAmount;
                return amountWithCharges;
            }
            else
            {
                decimal amountWithCharges = amount + Charges - customAmount;
                return amountWithCharges;
            }

        }


        private async Task<WithdrawalCharges> CalculateCharges(decimal amount, WithdrawalParameter withdrawalParameter, decimal customCharge, WithdrawalNotification notification, ChargesWaived chargesWaived, Config config, string legalForm, DateTime accountingDate,bool isMinorAccount)
        {
            //customCharge = 500;
            // Initialize a WithdrawalCharges object to store calculated charges
            WithdrawalCharges withdrawalCharges = new WithdrawalCharges();

            // Initialize variables for charge calculations
            decimal percentageChargedAmount = 0;
            decimal rangeChargeCalculated = 0;
            bool waiveCharge = false;
            decimal waivedAmount = 0;
            bool IsNotifiedBeforeWithdrawal = false;

            // Determine if the system is set for automatic charge deductions
            if (config.UseAutomaticChargingSystem)
            {
                withdrawalCharges.WithdrawalFormCharge = legalForm == LegalForms.Moral_Person.ToString() ? withdrawalParameter.MoralPersonWithdrawalFormFee : withdrawalParameter.PhysicalPersonWithdrawalFormFee;

                // Check if today is a holiday
                bool isHoliday = await _holyDayRepository.IsHoliday(accountingDate);
                string holidayMessage = isHoliday ? "holiday" : "non-holiday";
                bool isMoralForm = legalForm == LegalForms.Moral_Person.ToString();

                // Retrieve fees applicable for the withdrawal
                var Fees = await _savingProductFeeRepository
                    .FindBy(x => x.SavingProductId == withdrawalParameter.ProductId && x.Fee.IsAppliesOnHoliday == isHoliday && x.FeeType == "Withdrawal" && x.IsDeleted == false)
                    .Include(x => x.Fee)
                    .Include(x => x.Fee.FeePolicies)
                    .ToListAsync();

                // If no fees are found, throw an error
                if (!Fees.Any())
                {
                    string errorMessage = $"Withdrawal charges are not configured for {withdrawalParameter.Product.Name} on {holidayMessage} days. Contact your administrator to configure charges on {withdrawalParameter.Product.Name}.";
                    _logger.LogError(errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }

                // Retrieve applicable fees based on fee type
                var range = CurrencyNotesMapper.GetFeeByFeeType(Fees, "Range", isMoralForm);
                var rate = CurrencyNotesMapper.GetFeeByFeeType(Fees, "Rate", isMoralForm);

                // Calculate charges for moral form
                if (isMoralForm)
                {
                    rangeChargeCalculated = CurrencyNotesMapper.GetFeePolicyRange(range.Fee.FeePolicies.ToList(), amount).Charge;
                    withdrawalCharges.WithdrawalFormCharge = rangeChargeCalculated;
                    withdrawalCharges.TotalCharges = XAFWallet.CalculateCustomerWithdrawalCharges(withdrawalCharges.ServiceCharge, withdrawalCharges.WithdrawalFormCharge, isMinorAccount);
                }
                else
                {
                    
                    withdrawalCharges.NormalCharge += withdrawalCharges.WithdrawalFormCharge;

                    // Apply waived charges if applicable
                    if (chargesWaived != null)
                    {
                        waivedAmount = chargesWaived.CustomCharge;
                        waiveCharge = true;
                    }

                    // Check if there is a notification before withdrawal
                    if (notification != null)
                    {
                        IsNotifiedBeforeWithdrawal = true;
                    }

                    // Calculate service charge based on withdrawal fee rate
                    percentageChargedAmount = rate?.Fee?.FeePolicies?.FirstOrDefault()?.Value ?? 0;
                    if (rate != null)
                    {
                        decimal amo = (percentageChargedAmount / 100 * amount);
                        withdrawalCharges.NormalCharge += amo;
                        percentageChargedAmount = amo;
                    }
                    else
                    {
                        withdrawalCharges.NormalCharge += percentageChargedAmount;
                    }

                    // Check if a custom charge is applied
                    if (customCharge > 0)
                    {
                        withdrawalCharges.ServiceCharge = percentageChargedAmount;
                        customCharge -= percentageChargedAmount + withdrawalCharges.WithdrawalFormCharge;

                        // Check if notification is required for withdrawal
                        if (withdrawalParameter.MustNotifyOnWithdrawal)
                        {
                            if (IsNotifiedBeforeWithdrawal)
                            {
                                withdrawalCharges.WithdrawalChargeWithoutNotification = 0;
                                withdrawalCharges.WithdrawalFormCharge = 0;
                            }
                            else
                            {
                                withdrawalCharges.WithdrawalChargeWithoutNotification = percentageChargedAmount;
                            }
                        }
                    }
                    else
                    {
                        // Calculate service charge based on withdrawal fee rate or predefined ranges
                        if (percentageChargedAmount > 0)
                        {
                            withdrawalCharges.ServiceCharge = percentageChargedAmount;
                        }
                        else
                        {
                            rangeChargeCalculated = CurrencyNotesMapper.GetFeePolicyRange(range.Fee.FeePolicies.ToList(), amount).Charge;
                            withdrawalCharges.ServiceCharge = rangeChargeCalculated;
                        }

                        // Check if notification is required for withdrawal
                        if (withdrawalParameter.MustNotifyOnWithdrawal)
                        {
                            if (IsNotifiedBeforeWithdrawal)
                            {
                                withdrawalCharges.WithdrawalChargeWithoutNotification = 0;
                                withdrawalCharges.WithdrawalFormCharge = 0;
                                withdrawalCharges.ServiceCharge = 0;
                            }
                            else
                            {
                                withdrawalCharges.WithdrawalChargeWithoutNotification = percentageChargedAmount;
                            }
                        }
                    }

                    // Apply waived charges if applicable
                    if (waiveCharge)
                    {
                        withdrawalCharges.ServiceCharge = waivedAmount;
                        withdrawalCharges.WithdrawalFormCharge = 0;
                        withdrawalCharges.WithdrawalChargeWithoutNotification = 0;
                        withdrawalCharges.CloseOfAccountCharge = 0;
                    }

                    // Calculate total charges including service charge, form charge, and withdrawal charge without notification
                    withdrawalCharges.TotalCharges = XAFWallet.CalculateCustomerWithdrawalCharges(withdrawalCharges.ServiceCharge, withdrawalCharges.WithdrawalFormCharge, isMinorAccount);

                    // Validate the custom charge amount
                    ValidateCustomCharge(customCharge, isMinorAccount);
                }
            }
            else
            {
                // Check if a custom charge is applied
                if (customCharge > 0)
                {
                    //// Set the withdrawal form charge based on the legal form (Moral or Physical Person)
                    //withdrawalCharges.WithdrawalFormCharge = legalForm == LegalForms.Moral_Person.ToString() ?
                    //    withdrawalParameter.MoralPersonWithdrawalFormFee : withdrawalParameter.PhysicalPersonWithdrawalFormFee;

                    //// Check if today is a holiday
                    bool isHoliday = await _holyDayRepository.IsHoliday(accountingDate);
                    //string holidayMessage = isHoliday ? "holiday" : "non-holiday";

                    //// Determine if the legal form is Moral Person
                    //bool isMoralForm = legalForm == LegalForms.Moral_Person.ToString();


                    //// Retrieve fees applicable for the withdrawal
                    //var Fees = await _savingProductFeeRepository
                    //    .FindBy(x => x.SavingProductId == withdrawalParameter.ProductId && x.Fee.IsAppliesOnHoliday == isHoliday && x.FeeType == "Withdrawal" && x.IsDeleted == false)
                    //    .Include(x => x.Fee)
                    //    .Include(x => x.Fee.FeePolicies)
                    //    .ToListAsync();

                    //// If no fees are found, throw an error
                    //if (!Fees.Any())
                    //{
                    //    string errorMessage = $"Withdrawal charges are not configured for {withdrawalParameter.Product.Name} on {holidayMessage} days. Contact your administrator to configure charges on {withdrawalParameter.Product.Name}.";
                    //    _logger.LogError(errorMessage);
                    //    throw new InvalidOperationException(errorMessage);
                    //}

                    //// Retrieve applicable fees based on fee type
                    //var range = CurrencyNotesMapper.GetFeeByFeeType(Fees, "Range", isMoralForm);
                    //var rate = CurrencyNotesMapper.GetFeeByFeeType(Fees, "Rate", isMoralForm);

                    //// Calculate charges for moral form
                    //if (isMoralForm)
                    //{
                    //    rangeChargeCalculated = CurrencyNotesMapper.GetFeePolicyRange(range.Fee.FeePolicies.ToList(), amount).Charge;
                    //    withdrawalCharges.WithdrawalFormCharge = rangeChargeCalculated;
                    //    withdrawalCharges.TotalCharges = XAFWallet.CalculateCustomerWithdrawalCharges(withdrawalCharges.ServiceCharge, withdrawalCharges.WithdrawalFormCharge);
                    //}

                    //// Calculate the service charge by subtracting the withdrawal form charge from the custom charge
                    //withdrawalCharges.ServiceCharge = customCharge - withdrawalCharges.WithdrawalFormCharge;
                    //percentageChargedAmount = withdrawalCharges.ServiceCharge;

                    //// Update the custom charge by subtracting the calculated service charge and withdrawal form charge
                    //customCharge -= percentageChargedAmount + withdrawalCharges.WithdrawalFormCharge;

                    //// Check if notification is required for withdrawal
                    //if (withdrawalParameter.MustNotifyOnWithdrawal)
                    //{
                    //    if (IsNotifiedBeforeWithdrawal)
                    //    {
                    //        // If notified before withdrawal, set the charges without notification to 0
                    //        withdrawalCharges.WithdrawalChargeWithoutNotification = 0;
                    //        withdrawalCharges.WithdrawalFormCharge = 0;
                    //    }
                    //    else
                    //    {
                    //        // Otherwise, set the charge without notification to the calculated service charge
                    //        withdrawalCharges.WithdrawalChargeWithoutNotification = percentageChargedAmount;
                    //    }
                    //}




                    // Determine if the legal form is Moral Person
                    bool isMoralForm = legalForm == LegalForms.Moral_Person.ToString();

                    // Retrieve fees applicable for the withdrawal
                    var Fees = await _savingProductFeeRepository
                        .FindBy(x => x.SavingProductId == withdrawalParameter.ProductId &&
                                     x.Fee.IsAppliesOnHoliday == isHoliday &&
                                     x.FeeType == "Withdrawal" &&
                                     x.IsDeleted == false)
                        .Include(x => x.Fee)
                        .Include(x => x.Fee.FeePolicies)
                        .ToListAsync();

                    // If no fees are found, throw an error
                    if (!Fees.Any())
                    {
                        string holidayMessage = isHoliday ? "holiday" : "non-holiday";
                        string errorMessage = $"Withdrawal charges are not configured for {withdrawalParameter.Product.Name} on {holidayMessage} days. Contact your administrator to configure charges on {withdrawalParameter.Product.Name}.";
                        _logger.LogError(errorMessage);
                        throw new InvalidOperationException(errorMessage);
                    }

                    // Retrieve applicable fees based on fee type
                    var range = CurrencyNotesMapper.GetFeeByFeeType(Fees, "Range", isMoralForm);
                    var rate = CurrencyNotesMapper.GetFeeByFeeType(Fees, "Rate", isMoralForm);
                    // Calculate form charge based on legal form
                    decimal formCharge = isMoralForm ?
                        withdrawalParameter.MoralPersonWithdrawalFormFee : withdrawalParameter.PhysicalPersonWithdrawalFormFee;

                    // Calculate service fee, assuming rate.Fee.FeeAmount is a percentage
                    decimal serviceFee = 0;

                    if (isMoralForm)
                    {
                        rangeChargeCalculated = CurrencyNotesMapper.GetFeePolicyRange(range.Fee.FeePolicies.ToList(), amount).Charge;
                        formCharge = rangeChargeCalculated;
                    }
                    else
                    {
                        if (rate != null && rate.Fee.FeePolicies != null && rate.Fee.FeePolicies.Any())
                        {
                            serviceFee = rate.Fee.FeePolicies.FirstOrDefault()?.Value * amount / 100 ?? 0;
                        }
                        else
                        {
                            serviceFee = range?.Fee?.FeePolicies?.FirstOrDefault()?.Value ?? 0;
                        }
                    }

                    // Calculate the total expected charge
                    decimal expectedTotalCharge = formCharge + serviceFee;

                    // Validate the custom charge entered by the user
                    if (!isMinorAccount)
                    {
                        if (customCharge != expectedTotalCharge)
                        {
                            string errorMessage = $"The custom charge entered ({BaseUtilities.FormatCurrency(customCharge)}) is invalid. The expected charge should be exactly {BaseUtilities.FormatCurrency(expectedTotalCharge)} XAF, which includes a form charge of {BaseUtilities.FormatCurrency(formCharge)} and a service fee of {BaseUtilities.FormatCurrency(serviceFee)}.";
                            throw new InvalidOperationException(errorMessage);
                        }
                    }
                    

                    // Set the withdrawal form charge
                    withdrawalCharges.WithdrawalFormCharge = formCharge;

                    // Set the service charge
                    withdrawalCharges.ServiceCharge = serviceFee;

                    // Calculate total charges
                    withdrawalCharges.TotalCharges = expectedTotalCharge;

                    // Check if notification is required for withdrawal
                    if (withdrawalParameter.MustNotifyOnWithdrawal)
                    {
                        if (IsNotifiedBeforeWithdrawal)
                        {
                            // If notified before withdrawal, set the charges without notification to 0
                            withdrawalCharges.WithdrawalChargeWithoutNotification = 0;
                        }
                        else
                        {
                            // Otherwise, set the charge without notification to the calculated service fee
                            withdrawalCharges.WithdrawalChargeWithoutNotification = serviceFee;
                        }
                    }



                }


                // Calculate total charges including service charge, form charge, and withdrawal charge without notification
                withdrawalCharges.TotalCharges = XAFWallet.CalculateCustomerWithdrawalCharges(withdrawalCharges.ServiceCharge, withdrawalCharges.WithdrawalFormCharge,isMinorAccount);

                // Validate the custom charge amount
                //ValidateCustomCharge(customCharge);
            }
          
            // Return the calculated withdrawal charges
            return withdrawalCharges;
        }

        // Helper method to validate the custom charge amount
        private void ValidateCustomCharge(decimal customCharge,bool isMinorAccount)
        {
            if (!isMinorAccount)
            {
                if (customCharge < 0)
                {
                    throw new InvalidOperationException($"The charge entered is too low for this withdrawal. It is missing {BaseUtilities.FormatCurrency(Math.Abs(customCharge))} to complete the process.");
                }
                if (customCharge > 0)
                {
                    throw new InvalidOperationException($"The charge entered is too high for this withdrawal. It exceeds the expected charge by {BaseUtilities.FormatCurrency(Math.Abs(customCharge))}. Please recalculate the charge and enter it again, or let the system deduct the correct amount from the member's account.");
                }
            }
            
        }
        private Transaction CreateTransaction(BulkDeposit request, Account account, Teller teller, WithdrawalParameter withdrawalLimit, WithdrawalCharges withdrawalCharges, string sourceBranchId, string destinationBranchId, bool isInterBranch, string Reference, bool IsOfflineCharge, decimal TotalAmountToWithdrawWithCharges, bool isMobileMoneyOperation, DateTime accountingDate, bool isRemittance, Remittance remittance)
        {
            try
            {
                if (!isMobileMoneyOperation)
                {
                    // Calculate balance and credit based on original amount and fees, if IsOfflineCharge is true
                    decimal charges = withdrawalCharges.TotalCharges;
                    decimal balance = IsOfflineCharge ? account.Balance - request.Amount : account.Balance - TotalAmountToWithdrawWithCharges;
                    decimal debit = IsOfflineCharge ? request.Amount : request.Amount + charges;
                    decimal originalAmount = IsOfflineCharge ? charges + request.Amount : request.Amount;
                    if (request.IsChargesInclussive)
                    {
                        balance -= charges;
                        debit += charges;
                    }
                    if (isRemittance)
                    {

                        decimal sourceCommision = isInterBranch ? XAFWallet.CalculateCommission(withdrawalLimit.DestinationBranchOfficeShare, remittance.Fee) : 0;
                        decimal destinationCommision = isInterBranch ? XAFWallet.CalculateCommission(withdrawalLimit.SourceBrachOfficeShare, remittance.Fee) : remittance.Fee;
                        if (remittance.TransferType==TransferType.Incoming_International.ToString())
                        {

                        }
                        charges=0;
                        // Create the transaction entity
                        var transactionEntityEntryFee = new Transaction
                        {
                            Id = BaseUtilities.GenerateUniqueNumber(),
                            CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow),
                            Status = TransactionStatus.COMPLETED.ToString(),
                            TransactionReference = Reference,
                            TransactionType = withdrawalLimit.WithdrawalType,
                            Operation = TransactionType.WITHDRAWAL.ToString(),
                            AccountNumber = account.AccountNumber,
                            PreviousBalance = account.Balance,
                            AccountId = account.Id,
                            ProductId = account.ProductId,
                            SenderAccountId = account.Id,
                            ReceiverAccountId = account.Id,
                            BankId = teller.BankId,
                            CheckName = request.CheckName,
                            CheckNumber = request.CheckNumber,
                            IsSWS = request.IsSWS,
                            BranchId = teller.BranchId,
                            OriginalDepositAmount = originalAmount,// request.Amount,
                            Fee = charges,
                            AccountingDate = accountingDate,
                            CustomerId = account.CustomerId,
                            DepositerNote = request.Depositer.DepositerNote,
                            DepositerTelephone = request.Depositer.DepositerTelephone,
                            DepositorIDExpiryDate = request.Depositer.DepositorIDExpiryDate,
                            DepositorIDNumber = request.Depositer.DepositorIDNumber,
                            DepositorName = request.Depositer.DepositorName,
                            DepositorIDIssueDate = request.Depositer.DepositorIDIssueDate,
                            DepositorIDNumberPlaceOfIssue = request.Depositer.DepositorIDNumberPlaceOfIssue,
                            Tax = 0,
                            ExternalReference = request.ExternalReference,
                            CloseOfAccountCharge = withdrawalCharges.CloseOfAccountCharge,
                            OperationCharge = withdrawalCharges.ServiceCharge,
                            WithdrawalChargeWithoutNotification = withdrawalCharges.WithdrawalChargeWithoutNotification,
                            WithrawalFormCharge = withdrawalCharges.WithdrawalFormCharge,
                            Debit = debit,
                            Credit = 0,
                            Amount = -(debit),
                            Note = $"Note: {request.Note} - Statement: A remittance cashout of {BaseUtilities.FormatCurrency(originalAmount)} with charges: {BaseUtilities.FormatCurrency(charges)} was completed. Total amount disbursed to the receiver: {BaseUtilities.FormatCurrency(debit)}. Account Number: {account.AccountNumber}. Reference: {Reference}. Remittance Type: {remittance.RemittanceType}, Transfer Type: {remittance.TransferType}, Source Transfer: {remittance.TransferSource}",
                            OperationType = OperationType.Debit.ToString(),
                            FeeType = Events.WithDrawalCharges_Fee.ToString(),
                            TellerId = teller.Id,
                            Balance = balance,
                            IsInterBrachOperation = isInterBranch,
                            DestinationBrachId = destinationBranchId,
                            SourceBrachId = sourceBranchId,
                            DestinationBranchCommission = destinationCommision,
                            SourceBranchCommission = sourceCommision,
                            IsExternalOperation = request.IsExternalOperation, // Set external operation flag
                            ExternalApplicationName = request.ExternalApplicationName, // Set external application name
                            SourceType = remittance.TransferType, // Set source type
                            Currency = request.Currency, // Set currency
                            ReceiptTitle = $"Remittance Cashout Receipt ({remittance.RemittanceType}) - Reference: {Reference}",

                        };

                        return transactionEntityEntryFee; // Return the transaction entity


                    }
                    else
                    {
                        // Create the transaction entity
                        var transactionEntityEntryFee = new Transaction
                        {
                            Id = BaseUtilities.GenerateUniqueNumber(),
                            CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow),
                            Status = TransactionStatus.COMPLETED.ToString(),
                            TransactionReference = Reference,
                            TransactionType = withdrawalLimit.WithdrawalType,
                            Operation = TransactionType.WITHDRAWAL.ToString(),
                            AccountNumber = account.AccountNumber,
                            PreviousBalance = account.Balance,
                            AccountId = account.Id,
                            ProductId = account.ProductId,
                            SenderAccountId = account.Id,
                            ReceiverAccountId = account.Id,
                            BankId = teller.BankId,
                            CheckName = request.CheckName,
                            CheckNumber = request.CheckNumber,
                            IsSWS = request.IsSWS,
                            BranchId = teller.BranchId,
                            OriginalDepositAmount = originalAmount,// request.Amount,
                            Fee = charges,
                            AccountingDate = accountingDate,
                            CustomerId = account.CustomerId,
                            DepositerNote = request.Depositer.DepositerNote,
                            DepositerTelephone = request.Depositer.DepositerTelephone,
                            DepositorIDExpiryDate = request.Depositer.DepositorIDExpiryDate,
                            DepositorIDNumber = request.Depositer.DepositorIDNumber,
                            DepositorName = request.Depositer.DepositorName,
                            DepositorIDIssueDate = request.Depositer.DepositorIDIssueDate,
                            DepositorIDNumberPlaceOfIssue = request.Depositer.DepositorIDNumberPlaceOfIssue,
                            Tax = 0,
                            ExternalReference = request.ExternalReference,
                            CloseOfAccountCharge = withdrawalCharges.CloseOfAccountCharge,
                            OperationCharge = withdrawalCharges.ServiceCharge,
                            WithdrawalChargeWithoutNotification = withdrawalCharges.WithdrawalChargeWithoutNotification,
                            WithrawalFormCharge = withdrawalCharges.WithdrawalFormCharge,
                            Debit = debit,
                            Credit = 0,
                            Amount = -(TotalAmountToWithdrawWithCharges),
                            Note = $"Note:{request.Note} - Statement: A withdrawal of {BaseUtilities.FormatCurrency(originalAmount)} with charge: {BaseUtilities.FormatCurrency(charges)} was completed on account number {account.AccountNumber}, Total withdrawal: {BaseUtilities.FormatCurrency(TotalAmountToWithdrawWithCharges)}, Reference: {Reference}, Charges exclussive?: {IsOfflineCharge}", // Set transaction note
                            OperationType = OperationType.Debit.ToString(),
                            FeeType = Events.WithDrawalCharges_Fee.ToString(),
                            TellerId = teller.Id,
                            Balance = balance,
                            IsInterBrachOperation = isInterBranch,
                            DestinationBrachId = destinationBranchId,
                            SourceBrachId = sourceBranchId,
                            DestinationBranchCommission = isInterBranch ? XAFWallet.CalculateCommission(withdrawalLimit.DestinationBranchOfficeShare, charges) : 0,
                            SourceBranchCommission = isInterBranch ? XAFWallet.CalculateCommission(withdrawalLimit.SourceBrachOfficeShare, charges) : charges,
                            IsExternalOperation = request.IsExternalOperation, // Set external operation flag
                            ExternalApplicationName = request.ExternalApplicationName, // Set external application name
                            SourceType = request.SourceType, // Set source type
                            Currency = request.Currency, // Set currency
                            ReceiptTitle = withdrawalLimit.WithdrawalType == InterOperationType.Local_Check.ToString() ? "SWS Withdrawal Receipt Reference: " + Reference : "Cash Withdrawal Receipt Reference: " + Reference,

                        };

                        return transactionEntityEntryFee; // Return the transaction entity


                    }
                }
                else
                {
                    // Calculate balance and credit based on original amount and fees, if IsOfflineCharge is true
                    decimal charges = withdrawalCharges.TotalCharges;
                    decimal balance = IsOfflineCharge ? account.Balance + request.Amount : account.Balance + TotalAmountToWithdrawWithCharges;
                    decimal credit = IsOfflineCharge ? request.Amount : request.Amount + charges;
                    decimal originalAmount = IsOfflineCharge ? charges + request.Amount : request.Amount;

                    // Create the transaction entity
                    var transactionEntityEntryFee = new Transaction
                    {
                        Id = BaseUtilities.GenerateUniqueNumber(),
                        CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow),
                        Status = TransactionStatus.COMPLETED.ToString(),
                        TransactionReference = Reference,
                        TransactionType = withdrawalLimit.WithdrawalType,
                        Operation = TransactionType.WITHDRAWAL_MOBILEMONEY.ToString(),
                        AccountNumber = account.AccountNumber,
                        PreviousBalance = account.Balance,
                        AccountId = account.Id,
                        ProductId = account.ProductId,
                        SenderAccountId = account.Id,
                        ReceiverAccountId = account.Id,
                        BankId = teller.BankId,
                        AccountingDate = accountingDate,
                        BranchId = teller.BranchId,
                        OriginalDepositAmount = originalAmount,// request.Amount,
                        Fee = charges,
                        CustomerId = account.CustomerId,
                        DepositerNote = request.Depositer.DepositerNote,
                        DepositerTelephone = request.Depositer.DepositerTelephone,
                        DepositorIDExpiryDate = request.Depositer.DepositorIDExpiryDate,
                        DepositorIDNumber = request.Depositer.DepositorIDNumber,
                        DepositorName = request.Depositer.DepositorName,
                        DepositorIDIssueDate = request.Depositer.DepositorIDIssueDate,
                        DepositorIDNumberPlaceOfIssue = request.Depositer.DepositorIDNumberPlaceOfIssue,
                        Tax = 0,
                        ExternalReference = request.ExternalReference,
                        CloseOfAccountCharge = withdrawalCharges.CloseOfAccountCharge,
                        OperationCharge = withdrawalCharges.ServiceCharge,
                        WithdrawalChargeWithoutNotification = withdrawalCharges.WithdrawalChargeWithoutNotification,
                        WithrawalFormCharge = withdrawalCharges.WithdrawalFormCharge,
                        Debit = 0,
                        Credit = credit,
                        Amount = TotalAmountToWithdrawWithCharges,
                        Note = $"Note:{request.Note} - Statement: A CashIn of {BaseUtilities.FormatCurrency(originalAmount)} was completed on account number {account.AccountNumber}, Total: {BaseUtilities.FormatCurrency(TotalAmountToWithdrawWithCharges)}, Reference: {Reference}, Charges exclussive?: {IsOfflineCharge}", // Set transaction note
                        OperationType = OperationType.Debit.ToString(),
                        FeeType = Events.WithDrawalCharges_Fee.ToString(),
                        TellerId = teller.Id,
                        Balance = balance,
                        IsInterBrachOperation = isInterBranch,
                        DestinationBrachId = destinationBranchId,
                        SourceBrachId = sourceBranchId,
                        DestinationBranchCommission = isInterBranch ? XAFWallet.CalculateCommission(withdrawalLimit.DestinationBranchOfficeShare, charges) : 0,
                        SourceBranchCommission = isInterBranch ? XAFWallet.CalculateCommission(withdrawalLimit.SourceBrachOfficeShare, charges) : charges,
                        IsExternalOperation = request.IsExternalOperation, // Set external operation flag
                        ExternalApplicationName = request.ExternalApplicationName, // Set external application name
                        SourceType = request.SourceType, // Set source type
                        Currency = request.Currency, // Set currency
                        ReceiptTitle = $"{account.AccountType} Withdrawal Receipt Reference: " + Reference,

                    };

                    return transactionEntityEntryFee; // Return the transaction entity
                }
            }
            catch (Exception)
            {

                throw;
            }

        }


        /// <summary>
        /// Updates the teller account balance based on the transaction type (remittance or other transactions).
        /// Handles deposits, withdrawals, and custom charges for inter-branch and regular transactions.
        /// </summary>
        /// <param name="teller">The teller performing the transaction.</param>
        /// <param name="tellerAccount">The account associated with the teller.</param>
        /// <param name="transaction">The transaction details.</param>
        /// <param name="eventType">The type of event triggering the update.</param>
        /// <param name="isInterbranch">Indicates if the transaction is inter-branch.</param>
        /// <param name="sourceBranchId">The ID of the source branch.</param>
        /// <param name="destinationBranchId">The ID of the destination branch.</param>
        /// <param name="customCharge">Custom charge applied to the transaction.</param>
        /// <param name="Name">The name of the individual initiating the transaction.</param>
        /// <param name="accountingDate">The date for accounting the transaction.</param>
        /// <param name="isChargeInclussive">Indicates if the custom charge is inclusive of the transaction amount.</param>
        /// <param name="isRemittance">Indicates if the transaction is a remittance.</param>
        /// <param name="remittance">Details of the remittance transaction, if applicable.</param>
        private void UpdateTellerAccountBalance(Teller teller, Account tellerAccount, Transaction transaction, string eventType, bool isInterbranch, string sourceBranchId, string destinationBranchId, decimal customCharge, string Name, DateTime accountingDate, bool isChargeInclussive, bool isRemittance)
        {
            string withdrawalType = isRemittance ? TransactionType.CASH_W_Remittance.ToString() : TransactionType.CASH_WITHDRAWAL.ToString();

            if (!isChargeInclussive && customCharge > 0)
            {
                // Calculate the absolute transaction amount.
                decimal amount = Math.Abs(transaction.Amount);

                // Update the previous balance of the teller account.
                tellerAccount.PreviousBalance = tellerAccount.Balance;

                // Credit the teller account with the custom charge.
                _accountRepository.CreditAccount(tellerAccount, customCharge);
                string feetype = isRemittance ? TransactionType.FEE_Remittance.ToString() : TransactionType.FEE.ToString();
                // Create a teller operation record for the fee.
                var tellerOperationFee = CreateTellerOperation(
                    customCharge, OperationType.Credit, teller, tellerAccount, tellerAccount.Balance, transaction, feetype
                   , isInterbranch, sourceBranchId, destinationBranchId, Name, accountingDate);

                // Add the fee operation to the repository.
                _tellerOperationRepository.Add(tellerOperationFee);

                // Adjust the balance by subtracting the custom charge and transaction amount.
                tellerAccount.PreviousBalance = tellerAccount.Balance - customCharge;
                _accountRepository.DebitAccount(tellerAccount, amount);
                // Create a teller operation record for the withdrawal.
                var tellerOperationDeposit = CreateTellerOperation(
                    amount, OperationType.Debit, teller, tellerAccount, tellerAccount.Balance, transaction,
                    withdrawalType, isInterbranch, sourceBranchId, destinationBranchId, Name, accountingDate);

                // Add the withdrawal operation to the repository.
                _tellerOperationRepository.Add(tellerOperationDeposit);
            }
            else
            {
                // Calculate the amount to be debited, factoring in the custom charge.
                decimal amount = Math.Abs(transaction.Amount) - customCharge;

                // Update the previous balance of the teller account.
                tellerAccount.PreviousBalance = tellerAccount.Balance;

                // Debit the teller account with the calculated amount.
                _accountRepository.DebitAccount(tellerAccount, amount);

                // Create a teller operation record for the withdrawal.
                var tellerOperationDeposit = CreateTellerOperation(
                    amount, OperationType.Debit, teller, tellerAccount, tellerAccount.Balance, transaction,
                    withdrawalType, isInterbranch, sourceBranchId, destinationBranchId, Name, accountingDate);

                // Add the withdrawal operation to the repository.
                _tellerOperationRepository.Add(tellerOperationDeposit);
            }

        }





        
        private TellerOperation CreateTellerOperation(decimal amount, OperationType operationType, Teller teller, Account tellerAccount, decimal currentBalance, Transaction transaction, string eventType, bool isInterBranch, string sourceBranchId, string destinationBranchId, string Name, DateTime accountingDate)
        {
            var data = new TellerOperation
            {
                Id = BaseUtilities.GenerateUniqueNumber(),
                AccountID = tellerAccount.Id,
                AccountNumber = tellerAccount.AccountNumber,
                Amount = operationType == OperationType.Debit ? -(amount) : amount,
                OperationType = operationType.ToString(),
                BranchId = tellerAccount.BranchId, AccountingDate=accountingDate, IsCashOperation=true, 
                BankId = tellerAccount.BankId,
                OpenOfDayReference = tellerAccount.OpenningOfDayReference,
                CurrentBalance = currentBalance,
                Date = accountingDate,
                PreviousBalance = tellerAccount.PreviousBalance,
                TellerID = teller.Id,
                EventName = eventType,
                UserID = _userInfoToken.Id,
                MemberName = Name,
                TransactionReference = transaction.TransactionReference,
                TransactionType = eventType,
                ReferenceId = ReferenceNumberGenerator.GenerateReferenceNumber(_userInfoToken.BranchCode),
                CustomerId = transaction.CustomerId,
                MemberAccountNumber = transaction.AccountNumber,
                DestinationBrachId = destinationBranchId,
                SourceBranchId = sourceBranchId,
                IsInterBranch = isInterBranch,
                Description = $"{eventType} of {BaseUtilities.FormatCurrency(amount)} from Account number: {transaction.AccountNumber} to Account Number: {tellerAccount.AccountNumber}",
                DestinationBranchCommission = transaction.DestinationBranchCommission,
                SourceBranchCommission = transaction.SourceBranchCommission,

            };
            return data;
        }
        private void UpdateCustomerAccount(Account account, Transaction transactionEntityEntryFee, CustomerDto customer)
        {
            // Check if the account status is 'In Progress'
            if (account.Status == AccountStatus.Inprogress.ToString())
            {
                // If in progress, update account details for activation
                account.Balance = transactionEntityEntryFee.Amount; // Set balance to the transaction amount
                account.PreviousBalance = 0; // Previous balance is 0
                account.Status = AccountStatus.Active.ToString(); // Update account status to 'Active'
                account.AccountName = $"{account.Product.Name} [{customer.FirstName} {customer.LastName}]"; // Update account name
                account.EncryptedBalance = BalanceEncryption.Encrypt(account.Balance.ToString(), account.AccountNumber); // Encrypt balance
                account.BranchCode = customer.BranchCode;
                account.CustomerName = $"{customer.FirstName} {customer.LastName}";

            }
            else
            {
                // If not in progress, update account details for normal transaction
                account.PreviousBalance = account.Balance; // Set previous balance to current balance
                account.Balance = transactionEntityEntryFee.Balance; // Set balance to the balance brought forward from the transaction
                account.BranchCode = customer.BranchCode;
                account.CustomerName = $"{customer.FirstName} {customer.LastName}";
                account.EncryptedBalance = BalanceEncryption.Encrypt(account.Balance.ToString(), account.AccountNumber); // Encrypt balance


            }

            // Update the account in the repository
            _accountRepository.Update(account);
        }
       
        private void UpdateChargeWaiver(ChargesWaived chargesWaived, Transaction transaction, decimal normalCharge, Teller teller)
        {


            chargesWaived.IsWaiverDone = true;
            chargesWaived.DateOfWaived = transaction.CreatedDate;
            chargesWaived.TellerId = transaction.TellerId;
            chargesWaived.TellerCaise = teller.Name;
            chargesWaived.NormalCharge = normalCharge;
            chargesWaived.TransactionReference = transaction.TransactionReference;
            chargesWaived.TellerName = _userInfoToken.FullName;
            // Update the withdrawal notification in the repository
            _chargesWaivedRepository.Update(chargesWaived);
        }

        private async Task<WithdrawalParameter> GetWithdrawalLimit(List<WithdrawalParameter> withdrawalParameters, decimal Amount, string interOperationType)
        {

            // Find the withdrawal limit for cash withdrawals
            var withdrawal = withdrawalParameters.FirstOrDefault(dl => dl.WithdrawalType == interOperationType);
            if (withdrawal == null)
            {
                var errorMessage = $"{interOperationType} Withdrawal not configured for {withdrawalParameters.FirstOrDefault().Product.AccountType} account. Contact your system administrator to setup {interOperationType} withdrawal.";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);

            }

            // Check if the withdrawal amount is within the allowed range
            if (DecimalComparer.IsLessThan(Amount, withdrawal.MinAmount) || DecimalComparer.IsGreaterThan(Amount, withdrawal.MaxAmount))
            {
                // If withdrawal amount is out of range, throw an exception
                var errorMessage = $"Failed to withdraw amount: {Amount}. Withdrawal amount must be between {withdrawal.MinAmount} and {withdrawal.MaxAmount}.";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            // Return the withdrawal parameter if everything is within limits
            return withdrawal;
        }
        public async Task<ChargesWaived> GetWhitelistedWaiver(string customerId)
        {
            // Find if there are any entries in the ChargesWaived table for the specified customer
            var waivers = await _chargesWaivedRepository.FindBy(x => x.CustomerId == customerId && x.IsWaiverDone == false).ToListAsync();

            // Check if there are any active waivers
            foreach (var waiver in waivers)
            {
                // Check if the waiver is not expired and is approved
                if (waiver.DateOfWaiverRequest.Date >= BaseUtilities.UtcNowToDoualaTime().Date)
                {
                    // Return the whitelisted waiver object
                    return waiver;
                }
            }

            return null; // No whitelisted waiver found for the member
        }

        public static decimal CalculateCharge(List<FeePolicyDto> feePolicies, decimal amount)
        {
            // Find the applicable fee policy based on the given amount
            var applicablePolicy = feePolicies.FirstOrDefault(x => amount >= x.AmountFrom && amount <= x.AmountTo);

            // If a policy is found, return its charge; otherwise, return a default value (0 in this case)
            return applicablePolicy != null ? applicablePolicy.Charge : 0;
        }

        private bool IsAccountBalanceIntegrity(Account customerAccount)
        {
            // Verify the integrity of the account balance
            bool status = BalanceEncryption.VerifyBalanceIntegrity(customerAccount.Balance.ToString(), customerAccount.EncryptedBalance, customerAccount.AccountNumber);

            // If the verification fails, throw an exception
            if (!status)
            {
                // Throw an exception indicating that the account balance has been modified
                throw new InvalidOperationException($"The integrity of the account balance for account number {customerAccount.AccountNumber} has been compromised. Please contact your administrator immediately for assistance.");
            }

            // Return the status of the verification
            return status;
        }





    }
}
