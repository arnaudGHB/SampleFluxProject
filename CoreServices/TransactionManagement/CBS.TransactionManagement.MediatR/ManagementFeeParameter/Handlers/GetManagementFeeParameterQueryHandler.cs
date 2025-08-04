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
    /// Handles the request to retrieve a specific ManagementFeeParameter based on its unique identifier.
    /// </summary>
    public class GetManagementFeeParameterQueryHandler : IRequestHandler<GetManagementFeeParameterQuery, ServiceResponse<ManagementFeeParameterDto>>
    {
        private readonly IManagementFeeParameterRepository _ManagementFeeParameterRepository; // Repository for accessing ManagementFeeParameter data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetManagementFeeParameterQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetManagementFeeParameterQueryHandler.
        /// </summary>
        /// <param name="ManagementFeeParameterRepository">Repository for ManagementFeeParameter data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetManagementFeeParameterQueryHandler(
            IManagementFeeParameterRepository ManagementFeeParameterRepository,
            IMapper mapper,
            ILogger<GetManagementFeeParameterQueryHandler> logger)
        {
            _ManagementFeeParameterRepository = ManagementFeeParameterRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetManagementFeeParameterQuery to retrieve a specific ManagementFeeParameter.
        /// </summary>
        /// <param name="request">The GetManagementFeeParameterQuery containing ManagementFeeParameter ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
       public async Task<ServiceResponse<ManagementFeeParameterDto>> Handle(GetManagementFeeParameterQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the ManagementFeeParameter entity with the specified ID from the repository
                var entity = await _ManagementFeeParameterRepository.FindBy(a => a.Id == request.Id).Include(a => a.Product).FirstAsync();
                if (entity != null)
                {
                    // Map the ManagementFeeParameter entity to ManagementFeeParameterDto and return it with a success response
                    var ManagementFeeParameterDto = _mapper.Map<ManagementFeeParameterDto>(entity);
                    return ServiceResponse<ManagementFeeParameterDto>.ReturnResultWith200(ManagementFeeParameterDto);
                }
                else
                {
                    // If the ManagementFeeParameter entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("ManagementFeeParameter not found.");
                    return ServiceResponse<ManagementFeeParameterDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting ManagementFeeParameter: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<ManagementFeeParameterDto>.Return500(e);
            }
        }

    }

}
