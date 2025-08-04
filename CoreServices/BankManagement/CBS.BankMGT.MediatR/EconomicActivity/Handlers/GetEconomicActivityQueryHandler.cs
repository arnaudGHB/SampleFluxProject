using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Queries;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Repository;
using CBS.BankMGT.Data.Dto;
using Microsoft.EntityFrameworkCore;

namespace CBS.BankMGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific EconomicActivity based on its unique identifier.
    /// </summary>
    public class GetEconomicActivityQueryHandler : IRequestHandler<GetEconomicActivityQuery, ServiceResponse<EconomicActivityDto>>
    {
        private readonly IEconomicActivityRepository _EconomicActivityRepository; // Repository for accessing EconomicActivity data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetEconomicActivityQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetEconomicActivityQueryHandler.
        /// </summary>
        /// <param name="EconomicActivityRepository">Repository for EconomicActivity data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetEconomicActivityQueryHandler(
            IEconomicActivityRepository EconomicActivityRepository,
            IMapper mapper,
            ILogger<GetEconomicActivityQueryHandler> logger)
        {
            _EconomicActivityRepository = EconomicActivityRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetEconomicActivityQuery to retrieve a specific EconomicActivity.
        /// </summary>
        /// <param name="request">The GetEconomicActivityQuery containing EconomicActivity ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<EconomicActivityDto>> Handle(GetEconomicActivityQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the EconomicActivity entity with the specified ID from the repository
                var entity = await _EconomicActivityRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    // Map the EconomicActivity entity to EconomicActivityDto and return it with a success response
                    var EconomicActivityDto = _mapper.Map<EconomicActivityDto>(entity);
                    return ServiceResponse<EconomicActivityDto>.ReturnResultWith200(EconomicActivityDto);
                }
                else
                {
                    // If the EconomicActivity entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("EconomicActivity not found.");
                    return ServiceResponse<EconomicActivityDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting EconomicActivity: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<EconomicActivityDto>.Return500(e);
            }
        }
    }

}
