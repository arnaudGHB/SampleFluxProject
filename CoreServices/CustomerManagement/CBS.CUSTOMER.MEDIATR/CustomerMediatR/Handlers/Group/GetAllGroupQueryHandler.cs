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
    /// Handles the retrieval of all Groups based on the GetAllGroupQuery.
    /// </summary>
    public class GetAllGroupQueryHandler : IRequestHandler<GetAllGroupQuery, ServiceResponse<List<CUSTOMER.DATA.Entity.Group>>>
    {
        private readonly IGroupRepository _GroupRepository; // Repository for accessing Groups data.
        private readonly IGroupCustomerRepository _GroupCustomerRepository; // Repository for accessing GroupCustomers data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllGroupQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllGroupQueryHandler.
        /// </summary>
        /// <param name="GroupRepository">Repository for Groups data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllGroupQueryHandler(
            IGroupRepository GroupRepository,
            IGroupCustomerRepository GroupCustomerRepository,
        IMapper mapper, ILogger<GetAllGroupQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _GroupRepository = GroupRepository;
            _GroupCustomerRepository = GroupCustomerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllGroupQuery to retrieve all Groups.
        /// </summary>
        /// <param name="request">The GetAllGroupQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<CUSTOMER.DATA.Entity.Group>>> Handle(GetAllGroupQuery request, CancellationToken cancellationToken)
        {
            try
            {
             //   await _GroupRepository.AllIncluding(x => x.GroupCustomers, s => s.ProfileRequirements).ToListAsync();
                // Retrieve all Groups entities from the repository
                var entities = _mapper.Map<List<CUSTOMER.DATA.Entity.Group>>(await _GroupRepository.All.ToListAsync());

            /*    foreach(var entity in entities)
                {
                   

                    List<GroupRequirement> GroupRequirements= new List<GroupRequirement>();

                    GroupRequirements=await _GroupRequirementRepository.All.Where(e => e.GroupId == entity.GroupId).ToListAsync();
               
                    if (GroupRequirements.Any())
                    {
                        entity.ProfileRequirements = GroupRequirements;
                    }

                    List<CUSTOMER.DATA.Entity.GroupCustomer> GroupCustomers = new List<CUSTOMER.DATA.Entity.GroupCustomer>();

                    GroupCustomers = await _GroupCustomerRepository.All.Where(e => e.GroupId == entity.GroupId).ToListAsync();
                    if (GroupCustomers.Any())
                    {
                        entity.GroupCustomers = GroupCustomers;
                    }
                }
*/
                

                return ServiceResponse<List<CUSTOMER.DATA.Entity.Group>>.ReturnResultWith200(entities);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Groups: {e.Message}");
                return ServiceResponse<List<CUSTOMER.DATA.Entity.Group>>.Return500(e, "Failed to get all Groups");
            }
        }
    }
}
