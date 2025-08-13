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
    /// Handles the request to retrieve a specific EntryFeeParameter based on its unique identifier.
    /// </summary>
    public class GetEntryFeeParameterQueryHandler : IRequestHandler<GetEntryFeeParameterQuery, ServiceResponse<EntryFeeParameterDto>>
    {
        private readonly IEntryFeeParameterRepository _EntryFeeParameterRepository; // Repository for accessing EntryFeeParameter data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetEntryFeeParameterQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetEntryFeeParameterQueryHandler.
        /// </summary>
        /// <param name="EntryFeeParameterRepository">Repository for EntryFeeParameter data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetEntryFeeParameterQueryHandler(
            IEntryFeeParameterRepository EntryFeeParameterRepository,
            IMapper mapper,
            ILogger<GetEntryFeeParameterQueryHandler> logger)
        {
            _EntryFeeParameterRepository = EntryFeeParameterRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetEntryFeeParameterQuery to retrieve a specific EntryFeeParameter.
        /// </summary>
        /// <param name="request">The GetEntryFeeParameterQuery containing EntryFeeParameter ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
       public async Task<ServiceResponse<EntryFeeParameterDto>> Handle(GetEntryFeeParameterQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the EntryFeeParameter entity with the specified ID from the repository
                var entity = await _EntryFeeParameterRepository.FindBy(a => a.Id == request.Id).Include(a => a.Product).FirstAsync();
                if (entity != null)
                {
                    // Map the EntryFeeParameter entity to EntryFeeParameterDto and return it with a success response
                    var EntryFeeParameterDto = _mapper.Map<EntryFeeParameterDto>(entity);
                    return ServiceResponse<EntryFeeParameterDto>.ReturnResultWith200(EntryFeeParameterDto);
                }
                else
                {
                    // If the EntryFeeParameter entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("EntryFeeParameter not found.");
                    return ServiceResponse<EntryFeeParameterDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting EntryFeeParameter: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<EntryFeeParameterDto>.Return500(e);
            }
        }

    }

}
