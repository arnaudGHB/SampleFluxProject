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
    /// Handles the retrieval of all Transaction based on the GetAllTransactionsByAccountQuery.
    /// </summary>
    public class GetAllTransactionsByAccountQueryHandler : IRequestHandler<GetAllTransactionsByAccountQuery, ServiceResponse<List<TransactionDto>>>
    {
        private readonly ITransactionRepository _TransactionRepository; // Repository for accessing Transactions data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllTransactionsByAccountQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllTransactionsByAccountQueryHandler.
        /// </summary>
        /// <param name="TransactionRepository">Repository for Transactions data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllTransactionsByAccountQueryHandler(
            ITransactionRepository TransactionRepository,
            IMapper mapper, ILogger<GetAllTransactionsByAccountQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _TransactionRepository = TransactionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllTransactionsByAccountQuery to retrieve all Transactions.
        /// </summary>
        /// <param name="request">The GetAllTransactionsByAccountQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<TransactionDto>>> Handle(GetAllTransactionsByAccountQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all Transactions entities from the repository
                var entities = await _TransactionRepository.FindBy(a=>a.AccountId == request.AccountId).ToListAsync();
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
