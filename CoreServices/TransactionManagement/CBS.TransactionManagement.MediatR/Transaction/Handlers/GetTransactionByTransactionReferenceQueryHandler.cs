using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using Microsoft.EntityFrameworkCore;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Transaction based on its unique identifier.
    /// </summary>
    public class GetTransactionByTransactionReferenceQueryHandler : IRequestHandler<GetTransactionByTransactionReferenceQuery, ServiceResponse<List<TransactionDto>>>
    {
        private readonly ITransactionRepository _TransactionRepository; // Repository for accessing Transaction data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetTransactionByTransactionReferenceQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly ICurrencyNotesRepository _currencyNotesRepository;
        /// <summary>
        /// Constructor for initializing the GetTransactionByTransactionReferenceQueryHandler.
        /// </summary>
        /// <param name="TransactionRepository">Repository for Transaction data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetTransactionByTransactionReferenceQueryHandler(
            ITransactionRepository TransactionRepository,
            IMapper mapper,
            ILogger<GetTransactionByTransactionReferenceQueryHandler> logger,
            ICurrencyNotesRepository currencyNotesRepository = null)
        {
            _TransactionRepository = TransactionRepository;
            _mapper = mapper;
            _logger = logger;
            _currencyNotesRepository = currencyNotesRepository;
        }

        /// <summary>
        /// Handles the GetTransactionByTransactionReferenceQuery to retrieve a specific Transaction.
        /// </summary>
        /// <param name="request">The GetTransactionByTransactionReferenceQuery containing Transaction ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<TransactionDto>>> Handle(GetTransactionByTransactionReferenceQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the Transaction entity with the specified ID from the repository
                var entity = await _TransactionRepository.AllIncluding(x=>x.Account, b=>b.Teller,o=>o.TellerOperations).Include(x=>x.Account.Product).Where(x=>x.Id==request.TransactionReference).ToListAsync();
                if (entity.Any())
                {
                    var note = _currencyNotesRepository.FindBy(x => x.ReferenceId == request.TransactionReference).ToList();
                    // Map the Transaction entity to TransactionDto along with currency notes
                    var transactionDto = _mapper.Map<List<TransactionDto>>(entity);
                    transactionDto.ForEach(t => t.CurrencyNotes = _mapper.Map<List<CurrencyNotesDto>>(note));
                    return ServiceResponse<List<TransactionDto>>.ReturnResultWith200(transactionDto);
                }
                else
                {
                    // If the Transaction entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Transaction not found.");
                    return ServiceResponse<List<TransactionDto>>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Transaction: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<List<TransactionDto>>.Return500(e);
            }
        }

    }

}
