// Ignore Spelling: Mediat MEDIATR

using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.MEDIATR;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;
using CBS.CUSTOMER.REPOSITORY.GroupRepo;

namespace CBS.Group.MEDIATR.GroupMediatR
{
    /// <summary>
    /// Handles the retrieval of all Groups based on the GetAllGroupTypeQuery.
    /// </summary>
    public class GetAllGroupTypeQueryHandler : IRequestHandler<GetAllGroupTypeQuery, ServiceResponse<List<CUSTOMER.DATA.Entity.GroupType>>>
    {
        private readonly IGroupTypeRepository _GroupTypeRepository; // Repository for accessing Groups data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllGroupTypeQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllGroupTypeQueryHandler.
        /// </summary>
        /// <param name="GroupRepository">Repository for Groups data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllGroupTypeQueryHandler(
            IGroupTypeRepository GroupTypeRepository,
        IMapper mapper, ILogger<GetAllGroupTypeQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _GroupTypeRepository = GroupTypeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllGroupTypeQuery to retrieve all Groups.
        /// </summary>
        /// <param name="request">The GetAllGroupTypeQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<CUSTOMER.DATA.Entity.GroupType>>> Handle(GetAllGroupTypeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var entities = _mapper.Map<List<CUSTOMER.DATA.Entity.GroupType>>(await _GroupTypeRepository.All.ToListAsync());            
                return ServiceResponse<List<CUSTOMER.DATA.Entity.GroupType>>.ReturnResultWith200(entities);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Groups: {e.Message}");
                return ServiceResponse<List<CUSTOMER.DATA.Entity.GroupType>>.Return500(e, "Failed to get all Groups");
            }
        }
    }
}
