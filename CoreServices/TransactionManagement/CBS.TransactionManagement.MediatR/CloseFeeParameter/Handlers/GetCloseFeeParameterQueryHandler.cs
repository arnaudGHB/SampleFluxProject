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
    /// Handles the request to retrieve a specific CloseFeeParameter based on its unique identifier.
    /// </summary>
    public class GetCloseFeeParameterQueryHandler : IRequestHandler<GetCloseFeeParameterQuery, ServiceResponse<CloseFeeParameterDto>>
    {
        private readonly ICloseFeeParameterRepository _CloseFeeParameterRepository; // Repository for accessing CloseFeeParameter data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetCloseFeeParameterQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetCloseFeeParameterQueryHandler.
        /// </summary>
        /// <param name="CloseFeeParameterRepository">Repository for CloseFeeParameter data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetCloseFeeParameterQueryHandler(
            ICloseFeeParameterRepository CloseFeeParameterRepository,
            IMapper mapper,
            ILogger<GetCloseFeeParameterQueryHandler> logger)
        {
            _CloseFeeParameterRepository = CloseFeeParameterRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetCloseFeeParameterQuery to retrieve a specific CloseFeeParameter.
        /// </summary>
        /// <param name="request">The GetCloseFeeParameterQuery containing CloseFeeParameter ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
       public async Task<ServiceResponse<CloseFeeParameterDto>> Handle(GetCloseFeeParameterQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the CloseFeeParameter entity with the specified ID from the repository
                var entity = await _CloseFeeParameterRepository.FindBy(a => a.Id == request.Id).Include(a => a.Product).FirstAsync();
                if (entity != null)
                {
                    // Map the CloseFeeParameter entity to CloseFeeParameterDto and return it with a success response
                    var CloseFeeParameterDto = _mapper.Map<CloseFeeParameterDto>(entity);
                    return ServiceResponse<CloseFeeParameterDto>.ReturnResultWith200(CloseFeeParameterDto);
                }
                else
                {
                    // If the CloseFeeParameter entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("CloseFeeParameter not found.");
                    return ServiceResponse<CloseFeeParameterDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting CloseFeeParameter: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<CloseFeeParameterDto>.Return500(e);
            }
        }

    }

}
