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
    /// Handles the request to retrieve a specific Subdivision based on its unique identifier.
    /// </summary>
    public class GetSubdivisionQueryHandler : IRequestHandler<GetSubdivisionQuery, ServiceResponse<SubdivisionDto>>
    {
        private readonly ISubdivisionRepository _SubdivisionRepository; // Repository for accessing Subdivision data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetSubdivisionQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetSubdivisionQueryHandler.
        /// </summary>
        /// <param name="SubdivisionRepository">Repository for Subdivision data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetSubdivisionQueryHandler(
            ISubdivisionRepository SubdivisionRepository,
            IMapper mapper,
            ILogger<GetSubdivisionQueryHandler> logger)
        {
            _SubdivisionRepository = SubdivisionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetSubdivisionQuery to retrieve a specific Subdivision.
        /// </summary>
        /// <param name="request">The GetSubdivisionQuery containing Subdivision ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<SubdivisionDto>> Handle(GetSubdivisionQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the Subdivision entity with the specified ID from the repository
                var existingSubdivision = await _SubdivisionRepository.AllIncluding(c => c.Towns, cy => cy.Division).FirstOrDefaultAsync(c => c.Id == request.Id);
                if (existingSubdivision != null)
                {
                    // Map the Subdivision entity to SubdivisionDto and return it with a success response
                    var SubdivisionDto = _mapper.Map<SubdivisionDto>(existingSubdivision);
                    return ServiceResponse<SubdivisionDto>.ReturnResultWith200(SubdivisionDto);
                }
                else
                {
                    // If the Subdivision entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Subdivision not found.");
                    return ServiceResponse<SubdivisionDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Subdivision: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<SubdivisionDto>.Return500(e);
            }
        }
    }

}
