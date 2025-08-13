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
    /// Handles the retrieval of all WithdrawalLimits based on the GetAllWithdrawalLimitsQuery.
    /// </summary>
    public class GetAllWithdrawalLimitsQueryHandler : IRequestHandler<GetAllWithdrawalLimitsQuery, ServiceResponse<List<WithdrawalParameterDto>>>
    {
        private readonly IWithdrawalLimitsRepository _WithdrawalLimitsRepository; // Repository for accessing WithdrawalLimitss data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllWithdrawalLimitsQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllWithdrawalLimitsQueryHandler.
        /// </summary>
        /// <param name="WithdrawalLimitsRepository">Repository for WithdrawalLimitss data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllWithdrawalLimitsQueryHandler(
            IWithdrawalLimitsRepository WithdrawalLimitsRepository,
            IMapper mapper, ILogger<GetAllWithdrawalLimitsQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _WithdrawalLimitsRepository = WithdrawalLimitsRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllWithdrawalLimitsQuery to retrieve all WithdrawalLimitss.
        /// </summary>
        /// <param name="request">The GetAllWithdrawalLimitsQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<WithdrawalParameterDto>>> Handle(GetAllWithdrawalLimitsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all WithdrawalLimitss entities from the repository
                var entities = await _WithdrawalLimitsRepository.All.Where(x=>!x.IsDeleted).Include(a => a.Product).ToListAsync();
                return ServiceResponse<List<WithdrawalParameterDto>>.ReturnResultWith200(_mapper.Map<List<WithdrawalParameterDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all WithdrawalLimitss: {e.Message}");
                return ServiceResponse<List<WithdrawalParameterDto>>.Return500(e, "Failed to get all WithdrawalLimitss");
            }
        }
    }
}
