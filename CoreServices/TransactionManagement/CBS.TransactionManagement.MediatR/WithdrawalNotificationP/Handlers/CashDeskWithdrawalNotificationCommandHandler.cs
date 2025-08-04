using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Repository.WithdrawalNotificationP;
using CBS.TransactionManagement.Commands.WithdrawalNotificationP;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Repository.AccountServices;
using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Entity.WithdrawalNotificationP;
using CBS.TransactionManagement.Repository.AccountingDayOpening;
using CBS.TransactionManagement.MediatR.UtilityServices;

namespace CBS.TransactionManagement.Handlers.WithdrawalNotificationValidationP
{

    public class CashDeskWithdrawalNotificationCommandHandler : IRequestHandler<CashDeskWithdrawalNotificationCommand, ServiceResponse<TransactionDto>>
    {
        private readonly IWithdrawalNotificationRepository _WithdrawalNotificationRepository; // Repository for accessing WithdrawalNotification data.
        private readonly ILogger<CashDeskWithdrawalNotificationCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow; // Unit of work for transaction management.
        private readonly ITellerRepository _tellerRepository; // Repository for accessing Teller data.
        private readonly UserInfoToken _userInfoToken; // User info token for accessing user details.
        private readonly IAccountRepository _accountRepository; // Repository for accessing account data.
        private readonly IDailyTellerRepository _dailyTellerRepository; // Repository for accessing teller provisioning account data.
        private readonly ISubTellerProvisioningHistoryRepository _subTellerProvioningHistoryRepository; // Repository for accessing teller data.
        private readonly IDepositServices _depositServices;
        private readonly IConfigRepository _configRepository;
        private readonly IAccountingDayRepository _accountingDayRepository;
        private readonly ISavingProductRepository _savingProductRepository;
        private readonly IAPIUtilityServicesRepository _utilityServicesRepository;

        public IMediator _mediator { get; set; } // Mediator for handling requests.

        /// <summary>
        /// Constructor for initializing the CashDeskWithdrawalNotificationCommandHandler.
        /// </summary>
        /// <param name="WithdrawalNotificationValidationRepository">Repository for WithdrawalNotification data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="uow">Unit of work for transaction management (optional).</param>
        /// <param name="userInfoToken">User info token for accessing user details (optional).</param>
        /// <param name="tellerRepository">Repository for accessing Teller data (optional).</param>
        public CashDeskWithdrawalNotificationCommandHandler(
            IWithdrawalNotificationRepository WithdrawalNotificationValidationRepository,
            ILogger<CashDeskWithdrawalNotificationCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null,
            UserInfoToken userInfoToken = null,
            ITellerRepository tellerRepository = null,
            IAccountRepository accountRepository = null,
            IDailyTellerRepository tellerProvisioningAccountRepository = null,
            ISubTellerProvisioningHistoryRepository subTellerProvioningHistoryRepository = null,
            IDepositServices depositServices = null,
            IConfigRepository configRepository = null,
            IMediator mediator = null,
            IAccountingDayRepository accountingDayRepository = null,
            ISavingProductRepository savingProductRepository = null,
            IAPIUtilityServicesRepository utilityServicesRepository = null)
        {
            // Assign provided dependencies to local variables.
            _WithdrawalNotificationRepository = WithdrawalNotificationValidationRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
            _userInfoToken = userInfoToken;
            _tellerRepository = tellerRepository;
            _accountRepository = accountRepository;
            _dailyTellerRepository = tellerProvisioningAccountRepository;
            _subTellerProvioningHistoryRepository = subTellerProvioningHistoryRepository;
            _depositServices = depositServices;
            _configRepository = configRepository;
            _mediator = mediator;
            _accountingDayRepository = accountingDayRepository;
            _savingProductRepository = savingProductRepository;
            _utilityServicesRepository=utilityServicesRepository;
        }
        /// <summary>
        /// Handles the withdrawal notification command, processes payment for the withdrawal form fee, 
        /// and posts related accounting entries.
        /// </summary>
        /// <param name="request">The withdrawal notification command containing transaction details.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>Returns a service response containing the transaction details or an error message.</returns>
        public async Task<ServiceResponse<TransactionDto>> Handle(CashDeskWithdrawalNotificationCommand request, CancellationToken cancellationToken)
        {
            try
            {

                // Step 1: Retrieve the current accounting day for the branch
                var accountingDate = _accountingDayRepository.GetCurrentAccountingDay(_userInfoToken.BranchID);
                _logger.LogInformation("Retrieved accounting date: {AccountingDate}", accountingDate);

                // Step 2: Retrieve the withdrawal notification entity using the provided ID
                var existingWithdrawalNotification = await _WithdrawalNotificationRepository.FindAsync(request.Id);
                if (existingWithdrawalNotification == null)
                {
                    string notFoundMessage = $"The withdrawal notification with ID {request.Id} was not found.";
                    _logger.LogWarning(notFoundMessage);
                    await BaseUtilities.LogAndAuditAsync(notFoundMessage, request, HttpStatusCodeEnum.NotFound, LogAction.WithdrawalNotification, LogLevelInfo.Warning);
                    return ServiceResponse<TransactionDto>.Return404(notFoundMessage);
                }
                // Step 3: Validate if the notification has already been paid
                if (existingWithdrawalNotification.IsNotificationPaid)
                {
                    string alreadyPaidMessage = $"The withdrawal notification with ID {request.Id} has already been paid.";
                    _logger.LogWarning(alreadyPaidMessage);
                    await BaseUtilities.LogAndAuditAsync(alreadyPaidMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.WithdrawalNotification, LogLevelInfo.Warning);
                    return ServiceResponse<TransactionDto>.Return403(alreadyPaidMessage);
                }

                // Step 4: Verify system configuration and teller status
                var config = await _configRepository.GetConfigAsync(OperationSourceType.Web_Portal.ToString());
                _logger.LogInformation("System configuration retrieved successfully.");

                var tellerProvision = await _dailyTellerRepository.GetAnActiveSubTellerForTheDate();
                _logger.LogInformation("Active sub-teller provision retrieved: {TellerProvision}", tellerProvision);

                await _subTellerProvioningHistoryRepository.CheckIfDayIsStillOpened(tellerProvision.TellerId, accountingDate);
                _logger.LogInformation("Verified that the accounting day is open for teller ID: {TellerId}", tellerProvision.TellerId);

                // Step 5: Retrieve teller details and the associated teller account
                var teller = await _tellerRepository.RetrieveTeller(tellerProvision);
                _logger.LogInformation("Retrieved teller details: {Teller}", teller);

                var tellerAccount = await _accountRepository.RetrieveTellerAccount(teller);
                _logger.LogInformation("Teller account retrieved: {TellerAccount}", tellerAccount);

                // Step 6: Retrieve customer details and their branch information
                var customer = await _utilityServicesRepository.GetCustomer(existingWithdrawalNotification.CustomerId);
                _logger.LogInformation("Retrieved customer details: {Customer}", customer);

                var branch = await _utilityServicesRepository.GetBranch(customer.BranchId);
                _logger.LogInformation("Retrieved branch details: {Branch}", branch);

                // Step 7: Map the request data to the existing withdrawal notification entity
                _mapper.Map(request, existingWithdrawalNotification);

                // Step 8: Update the withdrawal notification to mark it as paid
                existingWithdrawalNotification.IsNotificationPaid = true;
                existingWithdrawalNotification.IsWithdrawalDone = false;
                existingWithdrawalNotification.DateFormFeeWasPaid = BaseUtilities.UtcNowToDoualaTime();
                existingWithdrawalNotification.TellerId_fee = teller.Id;
                existingWithdrawalNotification.TellerCaise_fee = teller.Name;
                existingWithdrawalNotification.TellerName_fee = _userInfoToken.FullName;

                _WithdrawalNotificationRepository.Update(existingWithdrawalNotification);
                _logger.LogInformation("Withdrawal notification updated as paid: {Notification}", existingWithdrawalNotification);

                // Step 9: Calculate the currency notes for the withdrawal form fee
                var currencyNote = await CalculateCurrencyNotes(existingWithdrawalNotification.FormNotificationCharge, existingWithdrawalNotification.TransactionReference);
                _logger.LogInformation("Currency notes calculated for transaction reference: {TransactionReference}", existingWithdrawalNotification.TransactionReference);

                // Step 10: Retrieve the customer's account details
                var customerAccount = await _accountRepository.GetAccount(existingWithdrawalNotification.AccountNumber, TransactionType.DEPOSIT.ToString());
                _logger.LogInformation("Customer account retrieved: {CustomerAccount}", customerAccount);

                // Step 11: Process the deposit for the withdrawal form fee
                var transactionDto = await _depositServices.DepositWithdrawalFormFee(
                    teller,
                    tellerAccount,
                    existingWithdrawalNotification,
                    currencyNote,
                    false,
                    teller.BranchId,
                    customer.BranchId,
                    existingWithdrawalNotification.TransactionReference,
                    customerAccount,
                    customer.Name,
                    false, false,
                    accountingDate);
                _logger.LogInformation("Deposit processed successfully for withdrawal form fee. Transaction details: {TransactionDto}", transactionDto);

                // Step 12: Save changes to the database
                await _uow.SaveAsync();
                _logger.LogInformation("Database changes saved successfully.");

                // Step 13: Send an SMS notification to the customer
                await SendSMSNotification(existingWithdrawalNotification, customer, branch);
                _logger.LogInformation("SMS notification sent to customer: {Customer}", customer);

                // Step 14: Retrieve the event code based on the customer's legal form
                var savingProduct = await _savingProductRepository.FindAsync(customerAccount.ProductId);
                string eventCode = customer.LegalForm == LegalForms.Physical_Person.ToString()
                    ? savingProduct.EventCodePhysicalPersonWithdrawalFormFee
                    : savingProduct.EventCodeMoralPersonWithdrawalFormFee;
                _logger.LogInformation("Event code determined: {EventCode}", eventCode);

                // Step 15: Post the accounting entries for the transaction
                string accountingResponse = await PostAccounting(existingWithdrawalNotification.FormNotificationCharge, eventCode, existingWithdrawalNotification.TransactionReference, accountingDate, customer);

                // Step 16: Prepare the service response
                var response = ServiceResponse<TransactionDto>.ReturnResultWith200(transactionDto, "Payment completed.");

                if (accountingResponse == "OK")
                {
                    _logger.LogInformation("Accounting entries posted successfully for transaction reference: {TransactionReference}", existingWithdrawalNotification.TransactionReference);
                    return response;
                }

                _logger.LogWarning("Payment completed, but there was an error posting the accounting event.");
                return ServiceResponse<TransactionDto>.ReturnResultWith200(transactionDto, "Payment completed with error posting event code.");
            }
            catch (Exception e)
            {
                // Step 17: Log the error and return a 500 Internal Server Error response
                string errorMessage = $"An error occurred while processing the withdrawal notification: {e.Message}";
                _logger.LogError(errorMessage, e);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.WithdrawalNotification, LogLevelInfo.Error);
                return ServiceResponse<TransactionDto>.Return500(e);
            }
        }

        public async Task<ServiceResponse<TransactionDto>> HandleX(CashDeskWithdrawalNotificationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var accountingDate = _accountingDayRepository.GetCurrentAccountingDay(_userInfoToken.BranchID);

                // Retrieve the existing WithdrawalNotification entity based on the provided ID
                var existingWithdrawalNotification = await _WithdrawalNotificationRepository.FindAsync(request.Id);

                // If the entity with the specified ID does not exist, return a 404 Not Found response
                if (existingWithdrawalNotification == null)
                {
                    return ServiceResponse<TransactionDto>.Return404($"{request.Id} was not found to be updated.");
                }
                // Checking status...
                if (existingWithdrawalNotification.IsNotificationPaid)
                {
                    return ServiceResponse<TransactionDto>.Return403($"Notification already paid");
                }

                bool isInterBranchOperation = false; // Flag to indicate if the operation is inter-branch.
                bool isFirstDeposit = false; // Flag to indicate if it's the first deposit for membership activation.

                // Check if system configuration is set
                var config = await _configRepository.GetConfigAsync(OperationSourceType.Web_Portal.ToString());

                // Check if the user account serves as a teller today
                var tellerProvision = await _dailyTellerRepository.GetAnActiveSubTellerForTheDate();

                // Check if the accounting day is still open
                await _subTellerProvioningHistoryRepository.CheckIfDayIsStillOpened(tellerProvision.TellerId, accountingDate);

                // Retrieve teller information
                var teller = await _tellerRepository.RetrieveTeller(tellerProvision);

                // Use AutoMapper to map properties from the request to the existingWithdrawalNotification
                _mapper.Map(request, existingWithdrawalNotification);

                // Retrieve sub teller account
                var tellerAccount = await _accountRepository.RetrieveTellerAccount(teller);

                // Retrieve customer information
                var customer = await _utilityServicesRepository.GetCustomer(existingWithdrawalNotification.CustomerId);

                // Retrieve branch information
                var branch = await _utilityServicesRepository.GetBranch(customer.BranchId);

                // Update the notification as paid and set relevant fields
                existingWithdrawalNotification.IsNotificationPaid = true;
                existingWithdrawalNotification.IsWithdrawalDone = false;
                existingWithdrawalNotification.DateFormFeeWasPaid = BaseUtilities.UtcNowToDoualaTime();
                existingWithdrawalNotification.TellerId_fee = teller.Id;
                existingWithdrawalNotification.TellerCaise_fee = teller.Name;
                existingWithdrawalNotification.TellerName_fee = _userInfoToken.FullName;

                // Use the repository to update the existing WithdrawalNotification entity
                _WithdrawalNotificationRepository.Update(existingWithdrawalNotification);

                // Retrieve currency notes
                var currencyNote = await CalculateCurrencyNotes(existingWithdrawalNotification.FormNotificationCharge, existingWithdrawalNotification.TransactionReference);

                // Get customer account information
                var customerAccount = await _accountRepository.GetAccount(existingWithdrawalNotification.AccountNumber, TransactionType.DEPOSIT.ToString());

                //customerAccount.Product = null;
                // Process the deposit for the withdrawal form fee
                var transactionDto = await _depositServices.DepositWithdrawalFormFee(
                    teller,
                    tellerAccount,
                    existingWithdrawalNotification,
                    currencyNote,
                    isInterBranchOperation,
                    teller.BranchId,
                    customer.BranchId,
                    existingWithdrawalNotification.TransactionReference,
                    customerAccount, customer.Name, false, false, accountingDate
                );

                // Save changes to the database
                await _uow.SaveAsync();

                // Send SMS notification to the customer
                await SendSMSNotification(existingWithdrawalNotification, customer, branch);

                // Determine the appropriate event code
                var savingProduct = await _savingProductRepository.FindAsync(customerAccount.ProductId);
                string eventCode = customer.LegalForm == LegalForms.Physical_Person.ToString()
                    ? savingProduct.EventCodePhysicalPersonWithdrawalFormFee
                    : savingProduct.EventCodeMoralPersonWithdrawalFormFee;
                // Post accounting entries
                string accountingResponse = await PostAccounting(existingWithdrawalNotification.FormNotificationCharge, eventCode, existingWithdrawalNotification.TransactionReference,accountingDate, customer);

                // Prepare the service response
                var response = ServiceResponse<TransactionDto>.ReturnResultWith200(transactionDto, "Payment completed.");

                // Check the accounting response and return the appropriate service response
                if (accountingResponse == "OK")
                {
                    return response;
                }

                return ServiceResponse<TransactionDto>.ReturnResultWith200(transactionDto, "Payment completed with error posting event code.");
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while paying withdrawal form fee: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<TransactionDto>.Return500(e);
            }
        }
       
        // Sends SMS notification to the customer
        private async Task SendSMSNotification(WithdrawalNotification withdrawalNotification, CustomerDto customer, BranchDto branch)
        {
            // Extract branch name
            string bankName = branch.name;
            // Extract transaction reference
            string reference = withdrawalNotification.TransactionReference;
            // Format the charge amount to a currency format
            string formattedCharge = BaseUtilities.FormatCurrency(withdrawalNotification.FormNotificationCharge);
            // Convert intended withdrawal date and grace period date to short date string format
            string intendedWithdrawalDate = withdrawalNotification.DateOfIntendedWithdrawal.ToShortDateString();
            string gracePeriodDate = withdrawalNotification.GracePeriodDate.ToShortDateString();
            // Convert the current date and time to local time and then to string
            string dateTimeNow = BaseUtilities.UtcNowToDoualaTime().ToString();
            // Construct the SMS message in English
            string msg = $"{customer.FirstName} {customer.LastName}, you have successfully paid {formattedCharge} for your savings withdrawal notification. Your withdrawal is scheduled between {intendedWithdrawalDate} and {gracePeriodDate}. If the withdrawal is not completed within this period, additional charges may apply. Transaction Reference: {reference}. Date and Time: {dateTimeNow}. Thank you for banking with us. Sincerely, {bankName}.";
            // If the customer's preferred language is French, construct the SMS message in French
            if (customer.Language.ToLower() == "french")
            {
                msg = $"{customer.FirstName} {customer.LastName}, vous avez payé avec succès {formattedCharge} pour votre notification de retrait d'épargne. Votre retrait est prévu entre le {intendedWithdrawalDate} et le {gracePeriodDate}. Si le retrait n'est pas effectué pendant cette période, des frais supplémentaires peuvent s'appliquer. Référence de la transaction : {reference}. Date et heure : {dateTimeNow}. Merci de faire affaire avec nous. Sincèrement, {bankName}.";
            }
            // Create a new SendSMSPICallCommand with the constructed message and the customer's phone number
            var sMSPICallCommand = new SendSMSPICallCommand
            {
                messageBody = msg,
                recipient = customer.Phone
            };
            // Send the command to the _mediator to send the SMS
            await _mediator.Send(sMSPICallCommand);
        }


        /// <summary>
        /// Posts an accounting event for a specified transaction.
        /// </summary>
        /// <param name="Amount">The transaction amount to be posted.</param>
        /// <param name="eventCode">The event code representing the transaction type.</param>
        /// <param name="TransactionRefenceId">The unique reference ID for the transaction.</param>
        /// <param name="accountDate">The accounting date for the transaction.</param>
        /// <param name="customer">The customer details associated with the transaction.</param>
        /// <returns>Returns "OK" if successful; otherwise, returns the error messages encountered.</returns>
        private async Task<string> PostAccounting(decimal Amount, string eventCode, string TransactionRefenceId, DateTime accountDate, CustomerDto customer)
        {
            try
            {
                string accountingResponseMessages = null; // Initialize variable to store response messages.

                // Create an accounting posting request using the helper method.
                var apiRequest = MakeAccountingPosting(Amount, eventCode, TransactionRefenceId, accountDate, customer.CustomerId, customer.Name);

                // Send the accounting request to the mediator for processing.
                var result = await _mediator.Send(apiRequest);

                // Handle the response from the accounting service.
                if (result.StatusCode != 200)
                {
                    string errorMessage = $"Accounting posting failed: {result.Message}. TransactionRefenceId: {TransactionRefenceId}";
                    accountingResponseMessages += $"{result.Message}, "; // Append error message to the response messages.
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, apiRequest, HttpStatusCodeEnum.InternalServerError, LogAction.AccountingPosting, LogLevelInfo.Error);
                    return accountingResponseMessages; // Return the accumulated error messages.
                }

                // Log the successful accounting posting.
                string successMessage = $"Accounting posting succeeded for TransactionRefenceId: {TransactionRefenceId}, " +
                                        $"Customer: {customer.Name} (ID: {customer.CustomerId}).";
                _logger.LogInformation(successMessage);
                await BaseUtilities.LogAndAuditAsync(successMessage, apiRequest, HttpStatusCodeEnum.OK, LogAction.AccountingPosting, LogLevelInfo.Information);

                return "OK"; // Return "OK" indicating success.
            }
            catch (Exception ex)
            {
                // Handle unexpected exceptions and log the details.
                string exceptionMessage = $"An error occurred during accounting posting: {ex.Message}. TransactionRefenceId: {TransactionRefenceId}";
                _logger.LogError(exceptionMessage, ex);
                await BaseUtilities.LogAndAuditAsync(exceptionMessage, new { Amount, eventCode, TransactionRefenceId, customer }, HttpStatusCodeEnum.InternalServerError, LogAction.AccountingPosting, LogLevelInfo.Error);
                throw; // Re-throw the exception for higher-level handling.
            }
        }




        // Calculates currency notes based on the provided amount and reference
        private async Task<List<CurrencyNotesDto>> CalculateCurrencyNotes(decimal amount, string reference)
        {
            var notes = CurrencyNotesMapper.CalculateCurrencyNotes(amount);
            var addCurrencyNotesCommand = new AddCurrencyNotesCommand { CurrencyNote = notes, Reference = reference };
            var currencyNoteResponse = await _mediator.Send(addCurrencyNotesCommand);
            // Check if currency notes calculation was successful
            if (currencyNoteResponse.StatusCode != 200)
                throw new InvalidOperationException(currencyNoteResponse.Message);
            return currencyNoteResponse.Data;
        }
        // Method to retrieve customer information
        private async Task<CustomerDto> GetCustomer(string customerId)
        {
            var customerCommandQuery = new GetCustomerCommandQuery { CustomerID = customerId }; // Create command to get customer.
            var customerResponse = await _mediator.Send(customerCommandQuery); // Send command to _mediator.

            // Check if customer information retrieval was successful
            if (customerResponse.StatusCode != 200)
            {
                var errorMessage = "Failed getting member's information";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }

            var customer = customerResponse.Data; // Get customer data from response.
            customer.Name = $"{customer.FirstName} {customer.LastName}";

            return customer; // Return customer data.
        }
        /// <summary>
        /// Creates an accounting posting command for auto-posting events.
        /// </summary>
        /// <param name="amount">The amount for the event.</param>
        /// <param name="eventCode">The event code associated with the posting.</param>
        /// <param name="transactionReferenceId">Unique transaction reference ID.</param>
        /// <param name="accountDate">The accounting date for the transaction.</param>
        /// <param name="MemberReference">Member reference ID associated with the transaction.</param>
        /// <param name="MemberName">Name of the member associated with the transaction.</param>
        /// <returns>Returns an instance of AutoPostingEventCommand with the populated details.</returns>
        private AutoPostingEventCommand MakeAccountingPosting(decimal amount, string eventCode, string transactionReferenceId, DateTime accountDate, string MemberReference, string MemberName)
        {
            // Create a new AutoPostingEventCommand instance.
            var addAccountingPostingCommand = new AutoPostingEventCommand
            {
                Source = TellerSources.Physical_Teller.ToString(), // Set the source of the transaction.
                AmountEventCollections = new List<AmountEventCollection>(), // Initialize the event collection list.
                TransactionReferenceId = transactionReferenceId, // Set the unique transaction reference ID.
                TransactionDate = accountDate, // Set the transaction date.
                MemberReference = MemberReference, // Set the member reference ID.
            };

            // Construct a detailed narration for the event.
            string narration =
                $"Payment for saving withdrawal form: " +
                $"[Amount: {BaseUtilities.FormatCurrency(amount)}, Event Code: {eventCode}, Member: {MemberName} ({MemberReference})].";

            // Add a new event collection entry with detailed information.
            addAccountingPostingCommand.AmountEventCollections.Add(new AmountEventCollection
            {
                Amount = amount, // Set the transaction amount.
                EventCode = eventCode, // Set the event code.
                Naration = narration // Use the constructed detailed narration.
            });

            return addAccountingPostingCommand; // Return the prepared command instance.
        }



    }

}
