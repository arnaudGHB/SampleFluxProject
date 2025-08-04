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
    /// Handles the request to retrieve a specific Region based on its unique identifier.
    /// </summary>
    public class GetRegionQueryHandler : IRequestHandler<GetRegionQuery, ServiceResponse<RegionDto>>
    {
        private readonly IRegionRepository _RegionRepository; // Repository for accessing Region data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetRegionQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetRegionQueryHandler.
        /// </summary>
        /// <param name="RegionRepository">Repository for Region data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetRegionQueryHandler(
            IRegionRepository RegionRepository,
            IMapper mapper,
            ILogger<GetRegionQueryHandler> logger)
        {
            _RegionRepository = RegionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetRegionQuery to retrieve a specific Region.
        /// </summary>
        /// <param name="request">The GetRegionQuery containing Region ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<RegionDto>> Handle(GetRegionQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the Region entity with the specified ID from the repository
                var entity = await _RegionRepository.AllIncluding(c => c.Divisions,cy=>cy.Country).FirstOrDefaultAsync(c => c.Id == request.Id);
                if (entity != null)
                {
                    // Map the Region entity to RegionDto and return it with a success response
                    var RegionDto = _mapper.Map<RegionDto>(entity);
                    return ServiceResponse<RegionDto>.ReturnResultWith200(RegionDto);
                }
                else
                {
                    // If the Region entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Region not found.");
                    return ServiceResponse<RegionDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Region: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<RegionDto>.Return500(e);
            }
        }
    }

}
