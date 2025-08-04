using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;
using CBS.CUSTOMER.REPOSITORY.GroupRepo;
using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.REPOSITORY;
using Microsoft.EntityFrameworkCore;
using CBS.CUSTOMER.MEDIATR;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DOMAIN.Context;

namespace CBS.Group.MEDIATR.GroupMediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Group based on its unique identifier.
    /// </summary>
    public class DeleteGroupQueryHandler : IRequestHandler<DeleteGroupCommand, ServiceResponse<bool>>
    {
        private readonly IGroupRepository _GroupRepository; // Repository for accessing Group data.
        private readonly IGroupCustomerRepository _GroupCustomerRepository; // Repository for accessing GroupCustomers data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<DeleteGroupQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the GetGroupQueryHandler.
        /// </summary>
        /// <param name="GroupRepository">Repository for Group data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteGroupQueryHandler(
            IGroupRepository GroupRepository,
            IGroupCustomerRepository GroupCustomerRepository,
            IMapper mapper,
            ILogger<DeleteGroupQueryHandler> logger,
            IUnitOfWork<POSContext> uow)
        {
            _GroupRepository = GroupRepository;
            _GroupCustomerRepository = GroupCustomerRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the GetGroupQuery to retrieve a specific Group.
        /// </summary>
        /// <param name="request">The GetGroupQuery containing Group ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                /*await _CustomerRepository.AllIncluding(x => x.MembershipNextOfKings, r => r.CustomerDocuments, t=> t.CardSignatureSpecimens,z=>z.CustomerCategory).FirstOrDefaultAsync(  s => s.CustomerId == request.CustomerId &&  s.IsDeleted == false);*/
                // Retrieve the Group entity with the specified ID from the repository
                var entity = await _GroupRepository.AllIncluding(x=>x.GroupCustomers,y =>y.GroupDocuments).FirstOrDefaultAsync(r=>r.GroupId ==request.Id && r.IsDeleted==false);
                if (entity != null)
                {
                    var groupCustomers =  _GroupCustomerRepository.All.Where(x=>x.GroupId==request.Id && x.IsDeleted==false).ToList();
                    if(groupCustomers.Any())
                    {
                        return ServiceResponse<bool>.Return500("Group has active members");
                    }
                    // Map the Group entity to Group and return it with a success response
                    //var Group = _mapper.Map<CUSTOMER.DATA.Entity.Group>(entity);
                    entity.IsDeleted = true;
                    entity.DeletedBy = request.UserId;
                    _GroupRepository.Update(entity);
                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<bool>.Return500();
                    }
                    return ServiceResponse<bool>.ReturnResultWith200(true);
                }
                else
                {
                    // If the Group entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Group not found.");
                    return ServiceResponse<bool>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Group: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
