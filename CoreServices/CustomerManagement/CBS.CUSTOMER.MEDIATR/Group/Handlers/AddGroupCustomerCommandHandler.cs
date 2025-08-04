
using MediatR;
using AutoMapper;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DOMAIN.Context;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.DATA.Dto;
using CBS.Customer.MEDIATR;
using CBS.Customer.MEDIATR.CustomerMediatR;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.REPOSITORY.GroupRepo;
using CBS.CUSTOMER.DATA.Entity;

namespace CBS.GroupCustomer.MEDIATR.GroupCustomerMediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new GroupCustomer.
    /// </summary>
    public class AddGroupCustomerCommandHandler : IRequestHandler<AddGroupCustomerCommand, ServiceResponse<List<CreateGroupCustomer>>>
    {
        private readonly IGroupCustomerRepository _GroupCustomerRepository; // Repository for accessing GroupCustomer data.
        private readonly IGroupRepository _GroupRepository; // Repository for accessing Group data.
        private readonly ICustomerRepository _CustomerRepository; // Repository for accessing Customer data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddGroupCustomerCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddGroupCustomerCommandHandler.
        /// </summary>
        /// <param name="GroupCustomerRepository">Repository for GroupCustomer data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddGroupCustomerCommandHandler(
            IGroupCustomerRepository GroupCustomerRepository,
            IMapper mapper,
            ILogger<AddGroupCustomerCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            ICustomerRepository customerRepository,
            IGroupRepository groupRepository)
        {
            _GroupCustomerRepository = GroupCustomerRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _CustomerRepository = customerRepository;
            _GroupRepository = groupRepository;
        }

        /// <summary>
        /// Handles the AddGroupCustomerCommand to add a new GroupCustomer.
        /// </summary>
        /// <param name="request">The AddGroupCustomerCommand containing GroupCustomer data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<CreateGroupCustomer>>> Handle(AddGroupCustomerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var GroupId = request.GroupId;

                var verfiyIfGroupExist = _GroupRepository.Find(GroupId);

                if (verfiyIfGroupExist == null)
                {
                    return ServiceResponse<List<CreateGroupCustomer>>.Return404("Group Record with CustomerId :  " + GroupId + " Does Not Exist");
                }

                if (request.CustomerIds == null && request.CustomerIds.Count == 0)
                {
                    return ServiceResponse<List<CreateGroupCustomer>>.Return404("Group CustomerIds is empty");
                }

                List<CUSTOMER.DATA.Entity.Customer> verifyIfAtList2CustomerExist = new List<CUSTOMER.DATA.Entity.Customer>();

                foreach (var customerId in request.CustomerIds)
                {
                    var verfiyIfCustomerExist = _CustomerRepository.Find(customerId);

                    if (verfiyIfCustomerExist != null)
                    {
                        verifyIfAtList2CustomerExist.Add(verfiyIfCustomerExist);
                    }

                }

                /* if (verifyIfAtList2CustomerExist.Count <= 1)
                 {
                     return ServiceResponse<CreateGroupCustomer>.Return404("Group Must Contain AtLeast 2 Existing Customers");
                 }*/

                List<CUSTOMER.DATA.Entity.GroupCustomer> GroupCustomersResult = new List<CUSTOMER.DATA.Entity.GroupCustomer>();

                foreach (var Customer in verifyIfAtList2CustomerExist)
                {
                    var GroupContainsCustomer = _GroupCustomerRepository.FindBy(x => x.GroupId == request.GroupId && x.CustomerId == Customer.CustomerId && x.IsDeleted == false).FirstOrDefault();

                    if (GroupContainsCustomer == null)
                    {
                        var GroupCustomerEntity = new CUSTOMER.DATA.Entity.GroupCustomer()
                        {
                            GroupCustomerId = BaseUtilities.GenerateUniqueNumber(),
                            CustomerId = Customer.CustomerId,
                            GroupId = GroupId,
                            DateOfJoining = BaseUtilities.UtcToLocal(DateTime.UtcNow),
                            IsGroupLeader = false,
                            CreatedDate = BaseUtilities.UtcToLocal(DateTime.UtcNow)
                        };

                        // Add the new GroupCustomer entity to the repository
                        _GroupCustomerRepository.Add(GroupCustomerEntity);
                        GroupCustomersResult.Add(GroupCustomerEntity);
                    }

                }

                if (request.commit)
                {
                    await _uow.SaveAsync();
                }


                var CreateGroupCustomer = _mapper.Map<List<CreateGroupCustomer>>(GroupCustomersResult);
                return ServiceResponse<List<CreateGroupCustomer>>.ReturnResultWith200(CreateGroupCustomer);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving GroupCustomer: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<List<CreateGroupCustomer>>.Return500(e);
            }
        }
    }

}
