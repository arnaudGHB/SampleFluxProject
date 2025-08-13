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
    /// Handles the request to retrieve a specific WithdrawalLimits based on its unique identifier.
    /// </summary>
    public class GetWithdrawalLimitsQueryHandler : IRequestHandler<GetWithdrawalLimitsQuery, ServiceResponse<WithdrawalParameterDto>>
    {
        private readonly IWithdrawalLimitsRepository _WithdrawalLimitsRepository; // Repository for accessing WithdrawalLimits data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetWithdrawalLimitsQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetWithdrawalLimitsQueryHandler.
        /// </summary>
        /// <param name="WithdrawalLimitsRepository">Repository for WithdrawalLimits data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetWithdrawalLimitsQueryHandler(
            IWithdrawalLimitsRepository WithdrawalLimitsRepository,
            IMapper mapper,
            ILogger<GetWithdrawalLimitsQueryHandler> logger)
        {
            _WithdrawalLimitsRepository = WithdrawalLimitsRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetWithdrawalLimitsQuery to retrieve a specific WithdrawalLimits.
        /// </summary>
        /// <param name="request">The GetWithdrawalLimitsQuery containing WithdrawalLimits ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<WithdrawalParameterDto>> Handle(GetWithdrawalLimitsQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the WithdrawalLimits entity with the specified ID from the repository, include the product
                var entity = await _WithdrawalLimitsRepository.FindBy(a => a.Id == request.Id).Include(a => a.Product).FirstOrDefaultAsync();
                if (entity != null)
                {
                    // Map the WithdrawalLimits entity to WithdrawalLimitsDto and return it with a success response
                    var WithdrawalLimitsDto = _mapper.Map<WithdrawalParameterDto>(entity);
                    return ServiceResponse<WithdrawalParameterDto>.ReturnResultWith200(WithdrawalLimitsDto);
                }
                else
                {
                    // If the WithdrawalLimits entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("WithdrawalLimits not found.");
                    return ServiceResponse<WithdrawalParameterDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting WithdrawalLimits: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<WithdrawalParameterDto>.Return500(e);
            }
        }

    }

}
