using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.REPOSITORY;
using Microsoft.EntityFrameworkCore;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR
{
    /// <summary>
    /// Handles the request to retrieve a specific AllJobTitles based on its unique identifier.
    /// </summary>
    public class GetAllJobTitlesQueryHandler : IRequestHandler<GetAllJobTitleQuery, ServiceResponse<List<GetJobTitle>>>
    {
        private readonly IJobTitleRepository _JobTitleRepository; // Repository for accessingAllJobTitles data.

      
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllJobTitlesQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetJobTitleQueryHandler.
        /// </summary>
        /// <param name="AllJobTitleLeavesRepository">Repository forAllJobTitles data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllJobTitlesQueryHandler(
            IJobTitleRepository JobTitleRepository,
        
            IMapper mapper,
            ILogger<GetAllJobTitlesQueryHandler> logger)
        {
            _JobTitleRepository = JobTitleRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllJobTitleQuery to retrieve a specificAllJobTitles.
        /// </summary>
        /// <param name="request">The GetJobTitleQuery containing AllJobTitlesAllJobTitles ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<GetJobTitle>>> Handle(GetAllJobTitleQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve theAllJobTitles entity with the specified ID from the repository
                var entity = await _JobTitleRepository.All.ToListAsync();
                if (entity != null)
                {
                    // Map theAllJobTitles entity toAllJobTitles and return it with a success response
                    var AllJobTitles = _mapper.Map<List<GetJobTitle>>(entity);

               


                    return ServiceResponse<List<GetJobTitle>>.ReturnResultWith200(AllJobTitles);
                }
                else
                {
                    // If theAllJobTitles entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("AllJobTitleLeaves not found.");
                    return ServiceResponse<List<GetJobTitle>>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while gettingAllJobTitles: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<List<GetJobTitle>>.Return500(e);
            }
        }
    }

}
