using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Transaction based on the GetAllTransactionsByAccountNumberQuery.
    /// </summary>
    public class GetAllTransactionsByAccountNumberQueryHandler : IRequestHandler<GetAllTransactionsByAccountNumberQuery, ServiceResponse<List<TransactionDto>>>
    {
        private readonly ITransactionRepository _TransactionRepository; // Repository for accessing Transactions data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllTransactionsByAccountNumberQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllTransactionsByAccountNumberQueryHandler.
        /// </summary>
        /// <param name="TransactionRepository">Repository for Transactions data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllTransactionsByAccountNumberQueryHandler(
            ITransactionRepository TransactionRepository,
            IMapper mapper, ILogger<GetAllTransactionsByAccountNumberQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _TransactionRepository = TransactionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllTransactionsByAccountNumberQuery to retrieve all Transactions.
        /// </summary>
        /// <param name="request">The GetAllTransactionsByAccountNumberQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<TransactionDto>>> Handle(GetAllTransactionsByAccountNumberQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all Transactions entities from the repository
                var entities = await _TransactionRepository
             .AllIncluding(x => x.Account, t => t.Teller).Include(x=>x.Account.Product)
             .Where(a => a.AccountNumber == request.AccountNumber)
             .ToListAsync();

                return ServiceResponse<List<TransactionDto>>.ReturnResultWith200(_mapper.Map<List<TransactionDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Transactions: {e.Message}");
                return ServiceResponse<List<TransactionDto>>.Return500(e, "Failed to get all Transactions");
            }
        }
    }
}
