using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.AccountingEntryManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific AccountingEntry based on its unique identifier.
    /// </summary>
    public class GetAccountingEntryQueryHandler : IRequestHandler<GetAccountingEntryQuery, ServiceResponse<AccountingEntryDto>>
    {
        private readonly  IAccountingEntryRepository _accountingEntryRepository; // Repository for accessing AccountingEntry data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAccountingEntryQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAccountingEntryQueryHandler.
        /// </summary>
        /// <param name="AccountingEntryRepository">Repository for AccountingEntry data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAccountingEntryQueryHandler(
            IAccountingEntryRepository AccountingEntryRepository,
            IMapper mapper,
            ILogger<GetAccountingEntryQueryHandler> logger)
        {
            _accountingEntryRepository = AccountingEntryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAccountingEntryQuery to retrieve a specific AccountingEntry.
        /// </summary>
        /// <param name="request">The GetAccountingEntryQuery containing AccountingEntry ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AccountingEntryDto>> Handle(GetAccountingEntryQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the AccountingEntry entity with the specified ID from the repository
                var entity = await _accountingEntryRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    if (entity.IsDeleted)
                    {
                        string message = "AccountingEntry has been deleted.";
                        // If the AccountingEntryCategory entity was not found, log the error and return a 404 Not Found response
                        _logger.LogError(message);
                        return ServiceResponse<AccountingEntryDto>.Return404(message);
                    }
                    else
                    {
                        // Map the AccountingEntry entity to AccountingEntryDto and return it with a success response
                        var AccountingEntryDto = _mapper.Map<AccountingEntryDto>(entity);
                        return ServiceResponse<AccountingEntryDto>.ReturnResultWith200(AccountingEntryDto);

                    }
                }
                else
                {
                    // If the AccountingEntry entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("AccountingEntry not found.");
                    return ServiceResponse<AccountingEntryDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting AccountingEntry: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<AccountingEntryDto>.Return500(e);
            }
        }
    }
}