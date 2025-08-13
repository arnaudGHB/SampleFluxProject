using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using System.Linq.Expressions;
using System.Xml;
using CBS.TransactionManagement.Data;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Transaction based on the GetAllTransactionsByCustomerIdQuery.
    /// </summary>
    public class GetAllTransactionsByDatesAndAccountNumberQueryHandler : IRequestHandler<GetAllTransactionsByDatesAndAccountNumberQuery, ServiceResponse<List<TransactionDto>>>
    {
        private readonly ITransactionRepository _TransactionRepository; // Repository for accessing Transactions data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllTransactionsByDatesAndAccountNumberQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllTransactionsByDatesAndAccountNumberQueryHandler.
        /// </summary>
        /// <param name="TransactionRepository">Repository for Transactions data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllTransactionsByDatesAndAccountNumberQueryHandler(
            ITransactionRepository TransactionRepository,
            IMapper mapper, ILogger<GetAllTransactionsByDatesAndAccountNumberQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _TransactionRepository = TransactionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllTransactionsByCustomerIdQuery to retrieve all Transactions.
        /// </summary>
        /// <param name="request">The GetAllTransactionsByCustomerIdQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<TransactionDto>>> Handle(GetAllTransactionsByDatesAndAccountNumberQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var entities = await _TransactionRepository.FindBy(x=>x.CreatedDate.Date>=request.DateFrom.Date && x.CreatedDate.Date<=request.DateTo && x.AccountNumber==request.AccountNumber).Include(x=>x.Account).ThenInclude(x=>x.Product).Include(x=>x.Teller).ToListAsync();
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
