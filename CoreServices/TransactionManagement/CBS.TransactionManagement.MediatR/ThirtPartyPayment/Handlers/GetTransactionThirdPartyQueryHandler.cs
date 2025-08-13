using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.ThirtPartyPayment.Queries;
using CBS.TransactionManagement.Data.Dto.Transaction;
using CBS.TransactionManagement.Data;

namespace CBS.TransactionManagement.ThirtPartyPayment.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Transaction based on the GetTransactionThirdPartyQuery.
    /// </summary>
    public class GetTransactionThirdPartyQueryHandler : IRequestHandler<GetTransactionThirdPartyQuery, ServiceResponse<List<TransactionThirdPartyDto>>>
    {
        private readonly ITransactionRepository _TransactionRepository; // Repository for accessing Transactions data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetTransactionThirdPartyQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetTransactionThirdPartyQueryHandler.
        /// </summary>
        /// <param name="TransactionRepository">Repository for Transactions data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetTransactionThirdPartyQueryHandler(
            ITransactionRepository TransactionRepository,
            IMapper mapper, ILogger<GetTransactionThirdPartyQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _TransactionRepository = TransactionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetTransactionThirdPartyQuery to retrieve all Transactions.
        /// </summary>
        /// <param name="request">The GetTransactionThirdPartyQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<TransactionThirdPartyDto>>> Handle(GetTransactionThirdPartyQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all Transactions entities from the repository
                var entities = await _TransactionRepository.FindBy(x => x.AccountNumber == request.AccountNumber).Include(x=>x.Teller).AsNoTracking().ToListAsync();
                var transactionThirdPartyDtos = MapTransactionsToDto(entities);
                return ServiceResponse<List<TransactionThirdPartyDto>>.ReturnResultWith200(transactionThirdPartyDtos);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Transactions: {e.Message}");
                return ServiceResponse<List<TransactionThirdPartyDto>>.Return500(e, "Failed to get all Transactions");
            }
        }

        public List<TransactionThirdPartyDto> MapTransactionsToDto(List<Transaction> transactions)
        {
            var transactionDtos = transactions.Select(transaction => new TransactionThirdPartyDto
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                AccountNumber = transaction.AccountNumber ?? string.Empty,
                Status = transaction.Status,
                TransactionReference = transaction.TransactionReference,
                TransactionType = transaction.OperationType,
                Note = transaction.Note ?? string.Empty,
                TelephoneNumber = transaction.DepositerTelephone ?? string.Empty,
                Fee = transaction.Fee,
                TotalCharge = transaction.WithrawalFormCharge + transaction.OperationCharge + transaction.WithdrawalChargeWithoutNotification + transaction.CloseOfAccountCharge,
                TransactionDate = transaction.CreatedDate,
                AmountInWord =BaseUtilities.ConvertToWords(transaction.Amount), // Assuming you have a method to convert amount to words
                BranchCode = transaction.BranchId,
                TellerCode = transaction.Teller.Code,
                ExternalReference = transaction.ExternalReference ?? string.Empty,
                ExternalApplicationName = transaction.ExternalApplicationName ?? string.Empty
            }).ToList();

            return transactionDtos;
        }

        
    }
}
