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
    /// Handles the request to retrieve a specific Town based on its unique identifier.
    /// </summary>
    public class GetTownQueryHandler : IRequestHandler<GetTownQuery, ServiceResponse<TownDto>>
    {
        private readonly ITownRepository _TownRepository; // Repository for accessing Town data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetTownQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetTownQueryHandler.
        /// </summary>
        /// <param name="TownRepository">Repository for Town data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetTownQueryHandler(
            ITownRepository TownRepository,
            IMapper mapper,
            ILogger<GetTownQueryHandler> logger)
        {
            _TownRepository = TownRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetTownQuery to retrieve a specific Town.
        /// </summary>
        /// <param name="request">The GetTownQuery containing Town ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<TownDto>> Handle(GetTownQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the Town entity with the specified ID from the repository
                var existingTown = await _TownRepository.AllIncluding(c => c.Subdivision).FirstOrDefaultAsync(c => c.Id == request.Id);
                if (existingTown != null)
                {
                    // Map the Town entity to TownDto and return it with a success response
                    var TownDto = _mapper.Map<TownDto>(existingTown);
                    return ServiceResponse<TownDto>.ReturnResultWith200(TownDto);
                }
                else
                {
                    // If the Town entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Town not found.");
                    return ServiceResponse<TownDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Town: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<TownDto>.Return500(e);
            }
        }
    }

}
