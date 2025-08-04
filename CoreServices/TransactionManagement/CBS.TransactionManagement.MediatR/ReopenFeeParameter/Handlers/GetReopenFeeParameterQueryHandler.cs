using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository.ReopenFeeParameterP;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific ReopenFeeParameter based on its unique identifier.
    /// </summary>
    public class GetReopenFeeParameterQueryHandler : IRequestHandler<GetReopenFeeParameterQuery, ServiceResponse<ReopenFeeParameterDto>>
    {
        private readonly IReopenFeeParameterRepository _ReopenFeeParameterRepository; // Repository for accessing ReopenFeeParameter data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetReopenFeeParameterQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetReopenFeeParameterQueryHandler.
        /// </summary>
        /// <param name="ReopenFeeParameterRepository">Repository for ReopenFeeParameter data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetReopenFeeParameterQueryHandler(
            IReopenFeeParameterRepository ReopenFeeParameterRepository,
            IMapper mapper,
            ILogger<GetReopenFeeParameterQueryHandler> logger)
        {
            _ReopenFeeParameterRepository = ReopenFeeParameterRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetReopenFeeParameterQuery to retrieve a specific ReopenFeeParameter.
        /// </summary>
        /// <param name="request">The GetReopenFeeParameterQuery containing ReopenFeeParameter ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
       public async Task<ServiceResponse<ReopenFeeParameterDto>> Handle(GetReopenFeeParameterQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the ReopenFeeParameter entity with the specified ID from the repository
                var entity = await _ReopenFeeParameterRepository.FindBy(a => a.Id == request.Id).Include(a => a.Product).FirstAsync();
                if (entity != null)
                {
                    // Map the ReopenFeeParameter entity to ReopenFeeParameterDto and return it with a success response
                    var ReopenFeeParameterDto = _mapper.Map<ReopenFeeParameterDto>(entity);
                    return ServiceResponse<ReopenFeeParameterDto>.ReturnResultWith200(ReopenFeeParameterDto);
                }
                else
                {
                    // If the ReopenFeeParameter entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("ReopenFeeParameter not found.");
                    return ServiceResponse<ReopenFeeParameterDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting ReopenFeeParameter: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<ReopenFeeParameterDto>.Return500(e);
            }
        }

    }

}
