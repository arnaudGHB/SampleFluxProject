using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Helper.Helper;
using CBS.TransactionManagement.MediatR.TellerP.Commands;
using CBS.TransactionManagement.Repository.AccountingDayOpening;
using CBS.TransactionManagement.Data.Entity.AccountingDayOpening;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Repository.DailyStatisticBoard;

namespace CBS.TransactionManagement.MediatR.SubTellerProvissioningP.Handlers
{

    /// <summary>
    /// Handles the command to update a Teller based on ProvisionSubTellerCommand.
    /// </summary>
    public class OpenningOfDaySubTellerCommandHandler : IRequestHandler<OpenningOfDaySubTellerCommand, ServiceResponse<SubTellerProvioningHistoryDto>>
    {
        private readonly ITellerRepository _TellerRepository; // Repository for accessing Teller data.
        private readonly IDailyTellerRepository _dailyTellerRepository; // Repository for accessing Teller data.
        private readonly IAccountRepository _AccountRepository;
        private readonly ILogger<OpenningOfDaySubTellerCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly ISubTellerProvisioningHistoryRepository _subTellerProvioningHistoryRepository;
        private readonly IPrimaryTellerProvisioningHistoryRepository _primaryTellerProvisioningHistoryRepository;
        private readonly IAccountingDayRepository _accountingDayRepository;
        private readonly IGeneralDailyDashboardRepository _generalDailyDashboardRepository; // Repository for accessing account data.
        private readonly IMediator _mediator;

        // Constructor for initializing the OpenningOfDaySubTellerCommandHandler.
        public OpenningOfDaySubTellerCommandHandler(
            ITellerRepository TellerRepository,
            IAccountRepository AccountRepository,
            ILogger<OpenningOfDaySubTellerCommandHandler> logger,
            UserInfoToken userInfoToken,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null,
            ISubTellerProvisioningHistoryRepository subTellerProvioningHistoryRepository = null,
            IMediator mediator = null,
            IPrimaryTellerProvisioningHistoryRepository primaryTellerProvisioningHistoryRepository = null,
            IDailyTellerRepository dailyTellerRepository = null,
            IAccountingDayRepository accountingDayRepository = null,
            IGeneralDailyDashboardRepository generalDailyDashboardRepository = null)
        {
            _TellerRepository = TellerRepository;
            _AccountRepository = AccountRepository;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _uow = uow;
            _subTellerProvioningHistoryRepository = subTellerProvioningHistoryRepository;
            _mediator = mediator;
            _primaryTellerProvisioningHistoryRepository = primaryTellerProvisioningHistoryRepository;
            _dailyTellerRepository = dailyTellerRepository;
            _accountingDayRepository = accountingDayRepository;
            _generalDailyDashboardRepository = generalDailyDashboardRepository;
        }

        // Handle method to process the OpenningOfDaySubTellerCommand
        public async Task<ServiceResponse<SubTellerProvioningHistoryDto>> Handle(OpenningOfDaySubTellerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Fetch the active sub-teller record for the current day from the repository.
                // Ensures that a sub-teller has been approved to operate today, critical for initializing the sub-teller's daily operations.
                var dailyTeller = await _dailyTellerRepository.GetAnActiveSubTellerForTheDate();

                // Step 2: Retrieve the current accounting day based on the branch the user is associated with.
                // This is essential for keeping track of the financial day across various operations.
                var accountingDate = _accountingDayRepository.GetCurrentAccountingDay(_userInfoToken.BranchID);

                // Step 3: Validate and retrieve sub-teller details using the active teller's ID.
                // This ensures that the sub-teller exists in the system and can proceed with the day's opening operations.
                var teller = await _TellerRepository.GetTeller(dailyTeller.TellerId);

                // Step 4: Fetch the sub-teller's account from the repository.
                // The sub-teller account is needed to verify the balance and perform further operations related to the opening process.
                var subTellerAccount = await _AccountRepository.RetrieveTellerAccount(teller);

                // Step 5: Validate that the primary teller's provisioning account has been opened for the day.
                // This ensures the primary teller is active and can support the sub-teller's operations for the day.
                var primaryTellerProvisioning = await _primaryTellerProvisioningHistoryRepository.CheckIfPrimaryTellerIsOpened(accountingDate);

                // Step 6: Retrieve the primary teller's details based on the provisioning information.
                // The primary teller is responsible for provisioning sub-tellers and handling the larger balance operations.
                var primaryTeller = await _TellerRepository.GetTeller(primaryTellerProvisioning.TellerId);

                // Step 7: Retrieve the primary teller's account for further operations.
                // The primary teller's account will be used to debit amounts when provisioning sub-tellers.
                var primaryTellerAccount = await _AccountRepository.RetrieveTellerAccount(primaryTeller);

                // Step 8: Generate a unique reference ID for the day's operations.
                // This reference is crucial for tracking all operations related to the sub-teller’s day opening.
                string Reference = primaryTellerProvisioning.ReferenceId;

                // Step 9: Retrieve currency notes provided by the sub-teller in the opening request.
                // This information is required to validate the denominations and amounts used during the day's opening.
                var notes = request.CurrencyNotes;

                // Step 10: Create a command to add currency notes to the system and send it using the _mediator.
                // This step ensures that the currency notes are properly tracked and stored for the opening process.
                var addCurrencyNotesCommand = new AddCurrencyNotesCommand { CurrencyNote = notes, Reference = Reference };
                var currencyNoteResponse = await _mediator.Send(addCurrencyNotesCommand);

                // Step 11: Validate the response from adding the currency notes.
                // If the operation was not successful (status code is not 200), return a 403 forbidden response.
                if (currencyNoteResponse.StatusCode != 200)
                {
                    return ServiceResponse<SubTellerProvioningHistoryDto>.Return403(currencyNoteResponse.Message);
                }

                // Step 12: Retrieve the total amount of currency notes added from the response.
                // The total is used to ensure the correct amount of funds are transferred to the sub-teller's account.
                var currencyNote = currencyNoteResponse.Data;
                decimal Total = 0;
                if (currencyNote.Any())
                {
                    Total = currencyNote.FirstOrDefault().Total;
                }

                // Step 13: Validate that the sub-teller's initial requested amount does not exceed available funds.
                // This step prevents the sub-teller from opening with more funds than are available in the primary teller’s account.
                //ValidateTransactionAmount(request.InitialAmount, subTellerAccount.PreviousBalance, Total);

                // Step 14: Create a provisioning history entry for the sub-teller’s opening of day.
                // This step records the details of the day’s opening, including amounts and notes for auditing purposes.
                var subTellerProvisioning = CreateSubTellerProvisioningHistory(request, teller, subTellerAccount, Reference, dailyTeller, accountingDate);

                // Step 15: Record the opening of the day for the sub-teller and store the provisioning details.
                // This officially logs the day’s opening, saving the currency notes and other relevant information in the database.
                var subTellerProvisioningHistory = _subTellerProvioningHistoryRepository.OpenDay(notes, subTellerProvisioning);

                // Step 16: Debit the primary teller's account by the initial amount being provisioned to the sub-teller.
                // This moves the requested funds from the primary teller's account to the sub-teller’s account, initializing their available balance.
                _AccountRepository.DebitAccount(primaryTellerAccount, request.InitialAmount);

                // Step 17: Record the cash-out operation for the primary teller, including denominations.
                // This ensures that the primary teller’s account is accurately updated based on the cash withdrawn for sub-teller provisioning.
                await _primaryTellerProvisioningHistoryRepository.CashOutByDinomination(request.InitialAmount, notes, primaryTellerProvisioning.TellerId, accountingDate, Reference);

                // Step 18: Reset the sub-teller's account balance to reflect the initial amount provisioned.
                // This step updates the sub-teller's account, enabling them to start operations for the day with the allocated amount.
                _AccountRepository.ResetAccountBalance(subTellerAccount, request.InitialAmount);

                // Step 19: Mark the day as opened for the sub-teller’s account, linking it to the reference.
                // This finalizes the opening process for the sub-teller, ensuring the account is ready for use throughout the day.
                await _AccountRepository.OpenDay(Reference, subTellerAccount, accountingDate);

                //var TellerBranch = await GetBranch(teller.BranchId);
                //var cashOperation = new CashOperation(teller.BranchId, request.InitialAmount, 0, TellerBranch.name, TellerBranch.branchCode, CashOperationType.OpenOfDaySubTill, LogAction.OpenOfTills, subTellerProvisioningHistory);
                //await _generalDailyDashboardRepository.UpdateOrCreateDashboardAsync(cashOperation);


                // Step 20: Save all changes to the database to ensure the operation is fully committed.
                // This is the final step of the operation, ensuring that all database transactions related to the opening process are persisted.
                await _uow.SaveAsync();

                // Step 21: Generate a success message and log the successful opening of the day.
                // This message will inform the user and system administrators that the sub-teller’s day has been opened successfully.
                string msg = $"Open of the sub-till for accounting day of {accountingDate} was successful with the sum of {BaseUtilities.FormatCurrency(request.InitialAmount)} for Teller {teller.Name}, operated by {dailyTeller.UserName}.";

                // Step 22: Map the provisioning history to a DTO object to return to the client.
                // The DTO contains all relevant details about the sub-teller’s provisioning history for client consumption.
                var subTellerProvisioningHistoryDto = _mapper.Map<SubTellerProvioningHistoryDto>(subTellerProvisioningHistory);
                subTellerProvisioningHistoryDto.Teller = teller;

                // Step 23: Log the successful operation and return a 200 success response.
                // This concludes the process, logging the result and sending the success response back to the client.
                _logger.LogInformation(msg);
                await BaseUtilities.LogAndAuditAsync(msg, request, HttpStatusCodeEnum.OK, LogAction.OpenOfTills, LogLevelInfo.Information);

                // Return the final response with a 200 success code, containing the provisioning history DTO.
                return ServiceResponse<SubTellerProvioningHistoryDto>.ReturnResultWith200(subTellerProvisioningHistoryDto, msg);
            }
            catch (InvalidOperationException ex)
            {
                string messageError = $"Validation error occurred while processing opening of the sub till: {ex.Message}";
                _logger.LogError(messageError);
                await BaseUtilities.LogAndAuditAsync(messageError, request, HttpStatusCodeEnum.InternalServerError, LogAction.OpenOfTills, LogLevelInfo.Error);
                return ServiceResponse<SubTellerProvioningHistoryDto>.Return400(ex.Message);
            }
            catch (Exception e)
            {
                // Step 24: Handle any exceptions that occur during the process.
                // If an error is encountered, it is logged for troubleshooting, and the user is notified with a 500 error response.
                string errorMessage = $"Error occurred while opening the sub till: {e.Message}";
                _logger.LogError(errorMessage);

                // Step 25: Log the error with detailed audit information for tracking and accountability.
                // This audit log ensures the failure is properly recorded with all relevant details.
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.OpenOfTills, LogLevelInfo.Error);

                // Return a 500 error response, indicating a server-side failure in the day-opening process.
                return ServiceResponse<SubTellerProvioningHistoryDto>.Return500(errorMessage);
            }
        }

        private async Task<BranchDto> GetBranch(string branchid)
        {
            var branchCommandQuery = new GetBranchByIdCommand { BranchId = branchid }; // Create command to get branch.
            var branchResponse = await _mediator.Send(branchCommandQuery); // Send command to _mediator.

            // Check if branch information retrieval was successful
            if (branchResponse.StatusCode != 200)
            {
                var errorMessage = $"Failed to retrieve branch information: {branchResponse.Message}";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            return branchResponse.Data; // Return branch data.
        }


        // Method to validate transaction amount
        private void ValidateTransactionAmount(decimal initialAmount, decimal balance, decimal denomination_Nate_Balance)
        {
            if (initialAmount < 0)
                throw new InvalidOperationException("Teller Initial Amount must be greater than 0.");

            if (balance < 0)
                throw new InvalidOperationException($"Insufficient funds. Make cash requisition to your primary teller");

            if (balance != denomination_Nate_Balance)
                throw new InvalidOperationException($"Opening balance does not match the denomination total.");

            if (balance != initialAmount)
                throw new InvalidOperationException($"Opening balance does not match the entered amount.");
        }

        // Method to create sub teller provisioning history
        private SubTellerProvioningHistory CreateSubTellerProvisioningHistory(OpenningOfDaySubTellerCommand request, Teller teller, Account account, string ReferenceId, DailyTeller dailyTeller, DateTime accountingDate)
        {
            var subtellerHistory = new SubTellerProvioningHistory
            {
                Id = BaseUtilities.GenerateUniqueNumber(10),
                TellerId = teller.Id,
                UserIdInChargeOfThisTeller = dailyTeller.UserName,
                ProvisionedBy = _userInfoToken.FullName,
                OpenedDate = accountingDate,
                ClossedDate = BaseUtilities.UtcToDoualaTime(new DateTime()),
                OpenOfDayAmount = request.InitialAmount,
                CashAtHand = request.InitialAmount,
                EndOfDayAmount = 0,
                AccountBalance = account.Balance,
                PrimaryTellerComment = string.Empty,
                LastUserID = string.Empty,
                BankId = teller.BankId,
                BranchId = teller.BranchId,
                PreviouseBalance = account.Balance,
                ReferenceId = ReferenceId,
                PrimaryTellerConfirmationStatus = CloseOfDayActions.Open_Of_The_Day.ToString(),
                ClossedStatus = CloseOfDayStatus.OOD.ToString(),
                SubTellerComment = $"Opening of the day [{accountingDate}] with the some of {BaseUtilities.FormatCurrency(request.InitialAmount)}, Dated: {DateTime.Now}",
                LastOPerationAmount = request.InitialAmount,
                Note = $"Opening of the day [{accountingDate}] with the some of {BaseUtilities.FormatCurrency(request.InitialAmount)}, Dated: {DateTime.Now}",
                LastOperationType = "Open of day",
                ReplenishedAmount = 0, 
                DailyTellerId = dailyTeller.Id
            };

            return subtellerHistory;

        }



    }

}
