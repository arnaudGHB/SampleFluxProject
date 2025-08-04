using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;
using CBS.CUSTOMER.REPOSITORY.GroupRepo;
using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.REPOSITORY;
using Microsoft.EntityFrameworkCore;

namespace CBS.Group.MEDIATR.GroupMediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Group based on its unique identifier.
    /// </summary>
    public class GetGroupQueryHandler : IRequestHandler<GetGroupQuery, ServiceResponse<CUSTOMER.DATA.Entity.Group>>
    {
        private readonly IGroupRepository _GroupRepository; // Repository for accessing Group data.
        private readonly IGroupCustomerRepository _GroupCustomerRepository; // Repository for accessing GroupCustomers data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetGroupQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetGroupQueryHandler.
        /// </summary>
        /// <param name="GroupRepository">Repository for Group data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetGroupQueryHandler(
            IGroupRepository GroupRepository,
            IGroupCustomerRepository GroupCustomerRepository,
            IMapper mapper,
            ILogger<GetGroupQueryHandler> logger)
        {
            _GroupRepository = GroupRepository;
            _GroupCustomerRepository = GroupCustomerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetGroupQuery to retrieve a specific Group.
        /// </summary>
        /// <param name="request">The GetGroupQuery containing Group ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CUSTOMER.DATA.Entity.Group>> Handle(GetGroupQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the Group entity with the specified ID from the repository
                var entity = await _GroupRepository.FindAsync(request.GroupId);
                if (entity != null)
                {
                    // Map the Group entity to Group and return it with a success response
                    var Group = _mapper.Map<CUSTOMER.DATA.Entity.Group>(entity);

                   

                    return ServiceResponse<CUSTOMER.DATA.Entity.Group>.ReturnResultWith200(Group);
                }
                else
                {
                    // If the Group entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Group not found.");
                    return ServiceResponse<CUSTOMER.DATA.Entity.Group>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Group: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<CUSTOMER.DATA.Entity.Group>.Return500(e);
            }
        }
    }

}
