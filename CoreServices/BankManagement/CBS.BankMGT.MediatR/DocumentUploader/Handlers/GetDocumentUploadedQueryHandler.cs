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
    /// Handles the request to retrieve a specific Division based on its unique identifier.
    /// </summary>
    public class GetDivisionQueryHandler : IRequestHandler<GetDivisionQuery, ServiceResponse<DivisionDto>>
    {
        private readonly IDivisionRepository _DivisionRepository; // Repository for accessing Division data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetDivisionQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetDivisionQueryHandler.
        /// </summary>
        /// <param name="DivisionRepository">Repository for Division data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetDivisionQueryHandler(
            IDivisionRepository DivisionRepository,
            IMapper mapper,
            ILogger<GetDivisionQueryHandler> logger)
        {
            _DivisionRepository = DivisionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetDivisionQuery to retrieve a specific Division.
        /// </summary>
        /// <param name="request">The GetDivisionQuery containing Division ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<DivisionDto>> Handle(GetDivisionQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the Division entity with the specified ID from the repository
                var existingDivision = await _DivisionRepository.AllIncluding(c => c.Subdivisions, cy => cy.Region).FirstOrDefaultAsync(c => c.Id == request.Id);
                if (existingDivision != null)
                {
                    // Map the Division entity to DivisionDto and return it with a success response
                    var DivisionDto = _mapper.Map<DivisionDto>(existingDivision);
                    return ServiceResponse<DivisionDto>.ReturnResultWith200(DivisionDto);
                }
                else
                {
                    // If the Division entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Division not found.");
                    return ServiceResponse<DivisionDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Division: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<DivisionDto>.Return500(e);
            }
        }
    }

}
