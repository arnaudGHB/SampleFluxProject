using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Command;
using DocumentFormat.OpenXml.Drawing.ChartDrawing;
using CBS.TransactionManagement.Data;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Transaction based on the GetAllTransactionQuery.
    /// </summary>
    public class GetAllTransactionsByDatesAndBranchQueryHandler : IRequestHandler<GetAllTransactionsByDatesAndBranchQuery, ServiceResponse<List<TransactionDto>>>
    {
        private readonly ITransactionRepository _TransactionRepository; // Repository for accessing Transactions data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllTransactionsByDatesAndBranchQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IDailyTellerRepository _dailyTellerRepository; // Repository for accessing Transactions data.
        private readonly IMediator _mediator; // Repository for accessing Transactions data.

        /// <summary>
        /// Constructor for initializing the GetAllTransactionQueryHandler.
        /// </summary>
        /// <param name="TransactionRepository">Repository for Transactions data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllTransactionsByDatesAndBranchQueryHandler(
            ITransactionRepository TransactionRepository,
            IMapper mapper, ILogger<GetAllTransactionsByDatesAndBranchQueryHandler> logger, IDailyTellerRepository dailyTellerRepository = null, IMediator mediator = null)
        {
            // Assign provided dependencies to local variables.
            _TransactionRepository = TransactionRepository;
            _mapper = mapper;
            _logger = logger;
            _dailyTellerRepository = dailyTellerRepository;
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the GetAllTransactionQuery to retrieve all Transactions.
        /// </summary>
        /// <param name="request">The GetAllTransactionQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<TransactionDto>>> Handle(GetAllTransactionsByDatesAndBranchQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Base query: filter non-deleted transactions
                var query = _TransactionRepository
                    .FindBy(x => !x.IsDeleted)
                    .Include(x => x.Account)
                    .Include(x => x.Teller)
                    .AsQueryable();

                // Filter by BranchID if provided
                if (!string.IsNullOrEmpty(request.BranchID) && request.BranchID!="n/a")
                {
                    query = query.Where(x => x.BranchId == request.BranchID);
                }

                // Filter by Date Range
                if (request.IsByDate)
                {
                    query = request.UseAccountingDate
                        ? query.Where(x => x.AccountingDate.Date >= request.DateFrom.Date && x.AccountingDate.Date <= request.DateTo.Date)
                        : query.Where(x => x.CreatedDate.Date >= request.DateFrom.Date && x.CreatedDate.Date <= request.DateTo.Date);
                }

                // Filter by TellerId if applicable
                if (request.ByTellerId && !string.IsNullOrEmpty(request.TellerId))
                {
                    query = query.Where(x => x.TellerId == request.TellerId);
                }

                // Fetch transactions efficiently and map to DTOs
                var transactions = await query
                    .Select(transaction => new TransactionDto
                    {
                        Id = transaction.Id,
                        Account = transaction.Account,
                        Amount = transaction.Amount,
                        OriginalDepositAmount = transaction.OriginalDepositAmount,
                        Debit = transaction.Debit,
                        Credit = transaction.Credit,
                        ExternalReference = transaction.ExternalReference,
                        IsExternalOperation = transaction.IsExternalOperation,
                        ExternalApplicationName = transaction.ExternalApplicationName,
                        Currency = transaction.Currency,
                        TransactionReference = transaction.TransactionReference,
                        AccountType = transaction.Account.AccountType,
                        CustomerId = transaction.CustomerId,
                        AccountId = transaction.AccountId,
                        AccountNumber = transaction.AccountNumber,
                        TransactionType = transaction.TransactionType,
                        OperationType = transaction.OperationType,
                        Status = transaction.Status,
                        Tax = transaction.Tax,
                        Operation = transaction.Operation,
                        PreviousBalance = transaction.PreviousBalance,
                        SourceBranchCommission = transaction.SourceBranchCommission,
                        DestinationBranchCommission = transaction.DestinationBranchCommission,
                        Note = transaction.Note,
                        SenderAccountId = transaction.SenderAccountId,
                        ReceiverAccountId = transaction.ReceiverAccountId,
                        DepositorIDNumber = transaction.DepositorIDNumber,
                        DepositerTelephone = transaction.DepositerTelephone,
                        DepositorName = transaction.DepositorName,
                        DepositorIDIssueDate = transaction.DepositorIDIssueDate,
                        DepositorIDExpiryDate = transaction.DepositorIDExpiryDate,
                        DepositorIDNumberPlaceOfIssue = transaction.DepositorIDNumberPlaceOfIssue,
                        DepositerNote = transaction.DepositerNote,
                        IsDepositDoneByAccountOwner = transaction.IsDepositDoneByAccountOwner,
                        IsInterBrachOperation = transaction.IsInterBrachOperation,
                        SourceBrachId = transaction.SourceBrachId,
                        DestinationBrachId = transaction.DestinationBrachId,
                        Balance = transaction.Balance,
                        ProductId = transaction.ProductId,
                        Fee = transaction.Fee,
                        WithrawalFormCharge = transaction.WithrawalFormCharge,
                        OperationCharge = transaction.OperationCharge,
                        WithdrawalChargeWithoutNotification = transaction.WithdrawalChargeWithoutNotification,
                        CloseOfAccountCharge = transaction.CloseOfAccountCharge,
                        FeeType = transaction.FeeType,
                        SourceType = transaction.SourceType,
                        CreatedDate = transaction.CreatedDate,
                        AccountingDate = transaction.AccountingDate,
                        CreatedBy = transaction.CreatedBy,
                        BankId = transaction.BankId,
                        BranchId = transaction.BranchId,
                        TellerId = transaction.TellerId,
                        Teller = transaction.Teller
                    })
                    .ToListAsync(cancellationToken);

               
                // Return the result
                return ServiceResponse<List<TransactionDto>>.ReturnResultWith200(transactions);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to get all Transactions: {e.Message}");
                return ServiceResponse<List<TransactionDto>>.Return500(e, "Failed to get all Transactions");
            }
        }

    }
}
