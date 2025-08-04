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
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands;

namespace CBS.Group.MEDIATR
{
    /// <summary>
    /// Handles the request to retrieve a specific Group based on its unique identifier.
    /// </summary>
    public class DeleteGroupCustomerQueryHandler : IRequestHandler<DeleteCustomerCommand, ServiceResponse<bool>>
    {
        private readonly IGroupRepository _GroupRepository; // Repository for accessing Group data.
        private readonly IGroupCustomerRepository _GroupCustomerRepository; // Repository for accessing GroupCustomers data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<DeleteGroupCustomerQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteGroupCustomerQueryHandler.
        /// </summary>
        /// <param name="GroupRepository">Repository for Group data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteGroupCustomerQueryHandler(
            IGroupRepository GroupRepository,
            IGroupCustomerRepository GroupCustomerRepository,
            IMapper mapper,
            ILogger<DeleteGroupCustomerQueryHandler> logger,
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
        public async Task<ServiceResponse<bool>> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {

                var groupCustomer = _GroupCustomerRepository.FindBy(x => x.GroupId == request.Id && x.IsDeleted == false).FirstOrDefault();

                if (groupCustomer != null)
                {

                    groupCustomer.IsDeleted = true;
                    groupCustomer.DeletedBy = request.UserId;
                    _GroupCustomerRepository.Update(groupCustomer);
                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<bool>.Return500();
                    }
                    return ServiceResponse<bool>.ReturnResultWith200(true);
                }
                else
                {
                    // If the Group entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Group Customer CustomerId  not found.");
                    return ServiceResponse<bool>.Return404("Group Customer CustomerId  not found.");
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
