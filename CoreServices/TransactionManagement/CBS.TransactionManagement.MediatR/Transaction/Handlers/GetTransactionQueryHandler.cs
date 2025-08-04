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
    public class GetTransactionQueryHandler : IRequestHandler<GetTransactionQuery, ServiceResponse<TransactionDto>>
    {
        private readonly ITransactionRepository _TransactionRepository; // Repository for accessing Transaction data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetTransactionQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly ICurrencyNotesRepository _currencyNotesRepository;
        /// <summary>
        /// Constructor for initializing the GetTransactionQueryHandler.
        /// </summary>
        /// <param name="TransactionRepository">Repository for Transaction data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetTransactionQueryHandler(
            ITransactionRepository TransactionRepository,
            IMapper mapper,
            ILogger<GetTransactionQueryHandler> logger,
            ICurrencyNotesRepository currencyNotesRepository = null)
        {
            _TransactionRepository = TransactionRepository;
            _mapper = mapper;
            _logger = logger;
            _currencyNotesRepository = currencyNotesRepository;
        }

        /// <summary>
        /// Handles the GetTransactionQuery to retrieve a specific Transaction.
        /// </summary>
        /// <param name="request">The GetTransactionQuery containing Transaction ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<TransactionDto>> Handle(GetTransactionQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the Transaction entity with the specified ID from the repository
                var entity = await _TransactionRepository.AllIncluding(x=>x.Account, b=>b.Teller,o=>o.TellerOperations).Include(x=>x.Account.Product).Where(x=>x.Id==request.Id).FirstOrDefaultAsync();
                if (entity != null)
                {
                    var note = _currencyNotesRepository.FindBy(x => x.ReferenceId == entity.TransactionReference).ToList();
                    // Map the Transaction entity to TransactionDto and return it with a success response
                    var TransactionDto = _mapper.Map<TransactionDto>(entity);
                    TransactionDto.CurrencyNotes= _mapper.Map<List<CurrencyNotesDto>>(note);
                    return ServiceResponse<TransactionDto>.ReturnResultWith200(TransactionDto);
                }
                else
                {
                    // If the Transaction entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Transaction not found.");
                    return ServiceResponse<TransactionDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Transaction: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<TransactionDto>.Return500(e);
            }
        }

    }

}
