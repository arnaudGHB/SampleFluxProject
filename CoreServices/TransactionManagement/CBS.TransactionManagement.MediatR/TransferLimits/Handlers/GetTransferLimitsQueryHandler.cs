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
    /// Handles the request to retrieve a specific TransferLimits based on its unique identifier.
    /// </summary>
    public class GetTransferLimitsQueryHandler : IRequestHandler<GetTransferLimitsQuery, ServiceResponse<TransferParameterDto>>
    {
        private readonly ITransferLimitsRepository _TransferLimitsRepository; // Repository for accessing TransferLimits data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetTransferLimitsQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetTransferLimitsQueryHandler.
        /// </summary>
        /// <param name="TransferLimitsRepository">Repository for TransferLimits data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetTransferLimitsQueryHandler(
            ITransferLimitsRepository TransferLimitsRepository,
            IMapper mapper,
            ILogger<GetTransferLimitsQueryHandler> logger)
        {
            _TransferLimitsRepository = TransferLimitsRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetTransferLimitsQuery to retrieve a specific TransferLimits.
        /// </summary>
        /// <param name="request">The GetTransferLimitsQuery containing TransferLimits ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<TransferParameterDto>> Handle(GetTransferLimitsQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the TransferLimits entity with the specified ID from the repository
                var entity = await _TransferLimitsRepository.FindBy(a => a.Id == request.Id).Include(a => a.Product).FirstOrDefaultAsync();
                if (entity != null)
                {
                    // Map the TransferLimits entity to TransferLimitsDto and return it with a success response
                    var TransferLimitsDto = _mapper.Map<TransferParameterDto>(entity);
                    return ServiceResponse<TransferParameterDto>.ReturnResultWith200(TransferLimitsDto);
                }
                else
                {
                    // If the TransferLimits entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("TransferLimits not found.");
                    return ServiceResponse<TransferParameterDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting TransferLimits: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<TransferParameterDto>.Return500(e);
            }
        }

    }

}
