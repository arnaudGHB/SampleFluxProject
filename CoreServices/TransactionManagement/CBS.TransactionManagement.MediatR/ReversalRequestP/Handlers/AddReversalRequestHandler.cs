using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data.Entity.ReversalRequestP;
using CBS.TransactionManagement.Data.Dto.ReversalRequestP;
using CBS.TransactionManagement.MediatR.Commands.ReversalRequestP;
using CBS.TransactionManagement.Repository.AccountingDayOpening;
using System.IO.Pipes;

namespace CBS.TransactionManagement.MediatR.Handlers.ReversalRequestP
{
    /// <summary>
    /// Handles the command to add a new Reversal Request.
    /// </summary>
    public class AddReversalRequestHandler : IRequestHandler<AddReversalRequestCommand, ServiceResponse<ReversalRequestDto>>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IReversalRequestRepository _reversalRequestRepository;
        private readonly IDailyTellerRepository _dailyTellerRepository;
        private readonly ITellerRepository _tellerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AddReversalRequestHandler> _logger;
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly IAccountingDayRepository _accountingDayRepository;
        public IMediator _mediator { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddReversalRequestHandler"/> class.
        /// </summary>
        public AddReversalRequestHandler(
            UserInfoToken UserInfoToken,
            IMapper mapper,
            ILogger<AddReversalRequestHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            IMediator mediator = null,
            ITellerRepository tellerRepository = null,
            IDailyTellerRepository dailyTellerRepository = null,
            ITransactionRepository transactionRepository = null,
            IReversalRequestRepository reversalRequestRepository = null,
            IAccountingDayRepository accountingDayRepository = null)
        {
            _mapper = mapper;
            _userInfoToken = UserInfoToken;
            _logger = logger;
            _uow = uow;
            _mediator = mediator;
            _tellerRepository = tellerRepository;
            _dailyTellerRepository = dailyTellerRepository;
            _transactionRepository = transactionRepository;
            _reversalRequestRepository = reversalRequestRepository;
            _accountingDayRepository = accountingDayRepository;
        }

        /// <summary>
        /// Handles the AddReversalRequestCommand to create a new Reversal Request.
        /// </summary>
        /// <param name="request">The AddReversalRequestCommand containing reversal request details.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<ReversalRequestDto>> Handle(AddReversalRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the current accounting day for the branch
                var accountingDate = _accountingDayRepository.GetCurrentAccountingDay(_userInfoToken.BranchID);

                // Step 2: Retrieve the active sub-teller for the day
                var dailyTeller = await _dailyTellerRepository.GetAnActiveSubTellerForTheDate();
                if (dailyTeller == null)
                {
                    string errorMessage = "No active sub-teller found for the day. Please ensure a teller session is active.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.ReversalProcessed, LogLevelInfo.Error);
                    return ServiceResponse<ReversalRequestDto>.Return404(errorMessage);
                }

                // Step 3: Check if a reversal request already exists for the transaction
                var existingReversalRequest = await _reversalRequestRepository.FindBy(x => x.TransactionReference == request.TransactionId).FirstOrDefaultAsync();
                if (existingReversalRequest != null)
                {
                    string errorMessage = "A pending reversal request for this transaction already exists and must be validated before submitting a new one.";
                    _logger.LogWarning(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Conflict, LogAction.ReversalProcessed, LogLevelInfo.Warning);
                    return ServiceResponse<ReversalRequestDto>.Return409(errorMessage);
                }

                // Step 4: Verify that the transaction exists before proceeding
                var transactions = await _transactionRepository.FindBy(x => x.TransactionReference == request.TransactionId).Include(x=>x.Account).ToListAsync();
                if (!transactions.Any())
                {
                    string errorMessage = $"Invalid transaction reference: {request.TransactionId}. The specified transaction does not exist.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.ReversalProcessed, LogLevelInfo.Error);
                    return ServiceResponse<ReversalRequestDto>.Return404(errorMessage);
                }

                // Step 5: Retrieve the teller processing the reversal
                var teller = await _tellerRepository.GetTeller(dailyTeller.TellerId);
                if (teller == null)
                {
                    string errorMessage = "Teller details could not be retrieved. Please verify your teller session.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.ReversalProcessed, LogLevelInfo.Error);
                    return ServiceResponse<ReversalRequestDto>.Return404(errorMessage);
                }
                decimal amount = transactions.Sum(x => Math.Abs(x.Amount)) + transactions.Sum(x => Math.Abs(x.Fee));
                string accountNumbers = string.Join(",", transactions.Select(t => t.AccountNumber));
                string AccountIds = string.Join(",", transactions.Select(t => t.AccountId));
                string AccountUsed = string.Join(",", transactions.Select(t => t.Account.AccountType));
                // Step 6: Map request data to ReversalRequest entity
                var reversalRequest = _mapper.Map<ReversalRequest>(request);
                reversalRequest.Id = BaseUtilities.GenerateUniqueNumber();
                reversalRequest.TransactionReference = request.TransactionId;
                reversalRequest.Status = Status.Pending.ToString();
                reversalRequest.ApprovedBy = "N/A";
                reversalRequest.ApprovedComment = "N/A";
                reversalRequest.ValidationComment = "N/A";
                reversalRequest.ValidatedBy = "N/A";
                reversalRequest.ApprovedDate = DateTime.MaxValue;
                reversalRequest.ValidationDate = DateTime.MaxValue;
                reversalRequest.RequestDate = accountingDate;
                reversalRequest.InitiatedBy = _userInfoToken.FullName;
                reversalRequest.CustomerId = transactions.FirstOrDefault().CustomerId;
                reversalRequest.AccountNumber = accountNumbers;
                reversalRequest.AccountId = AccountIds;
                reversalRequest.BranchId = teller.BranchId;
                reversalRequest.TellerId = teller.Id;
                reversalRequest.AccountUsed = AccountUsed;
                reversalRequest.DebitDirection=OperationType.Debit.ToString();
                if (transactions.FirstOrDefault().OperationType==OperationType.Debit.ToString())
                {
                    reversalRequest.DebitDirection=OperationType.Credit.ToString();
                }
                reversalRequest.Amount=Math.Abs(amount);
                // Step 7: Save the reversal request
                _reversalRequestRepository.Add(reversalRequest);
                await _uow.SaveAsync();

                // Step 8: Log and audit the successful creation of the reversal request
                string successMessage = $"Transaction reversal of {Math.Abs(amount)} has been successfully initiated.";
                _logger.LogInformation(successMessage);
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.ReversalProcessed, LogLevelInfo.Information);

                // Step 9: Map the ReversalRequest entity to a DTO and return a success response
                var reversalRequestDto = _mapper.Map<ReversalRequestDto>(reversalRequest);
                return ServiceResponse<ReversalRequestDto>.ReturnResultWith200(reversalRequestDto, successMessage);
            }
            catch (Exception e)
            {
                // Step 10: Handle errors gracefully and log exceptions
                string errorMessage = $"An error occurred while processing the reversal request: {e.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.ReversalProcessed, LogLevelInfo.Error);
                return ServiceResponse<ReversalRequestDto>.Return500(e, errorMessage);
            }
        }
    }

}
