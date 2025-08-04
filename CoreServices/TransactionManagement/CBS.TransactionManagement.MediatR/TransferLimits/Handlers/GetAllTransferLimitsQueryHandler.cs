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
    /// Handles the retrieval of all TempAccount based on the GetAllTempAccountQuery.
    /// </summary>
    public class GetAllTransferLimitsQueryHandler : IRequestHandler<GetAllTransferLimitsQuery, ServiceResponse<List<TransferParameterDto>>>
    {
        private readonly ITransferLimitsRepository _TransferLimitsRepository; // Repository for accessing TransferLimits data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllTransferLimitsQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllTempAccountQueryHandler.
        /// </summary>
        /// <param name="TempAccountRepository">Repository for TransferLimits data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllTransferLimitsQueryHandler(
            ITransferLimitsRepository TransferLimitsRepository,
            IMapper mapper, ILogger<GetAllTransferLimitsQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _TransferLimitsRepository = TransferLimitsRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllTempAccountQuery to retrieve all TransferLimits.
        /// </summary>
        /// <param name="request">The GetAllTempAccountQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<TransferParameterDto>>> Handle(GetAllTransferLimitsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all TransferLimits entities from the repository
                var entities = await _TransferLimitsRepository.All.Include(a => a.Product).ToListAsync();
                return ServiceResponse<List<TransferParameterDto>>.ReturnResultWith200(_mapper.Map<List<TransferParameterDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all TransferLimits: {e.Message}");
                return ServiceResponse<List<TransferParameterDto>>.Return500(e, "Failed to get all TransferLimits");
            }
        }

    }
}
