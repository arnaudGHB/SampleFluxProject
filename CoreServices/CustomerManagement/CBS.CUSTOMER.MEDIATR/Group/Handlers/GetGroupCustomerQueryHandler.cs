using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;
using Microsoft.EntityFrameworkCore;

namespace CBS.GroupCustomer.MEDIATR.GroupCustomerMediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific GroupCustomer based on its unique identifier.
    /// </summary>
    public class GetGroupCustomerQueryHandler : IRequestHandler<GetGroupCustomerQuery, ServiceResponse<CUSTOMER.DATA.Entity.GroupCustomer>>
    {
        private readonly IGroupCustomerRepository _GroupCustomerRepository; // Repository for accessing GroupCustomer data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetGroupCustomerQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetGroupCustomerQueryHandler.
        /// </summary>
        /// <param name="GroupCustomerRepository">Repository for GroupCustomer data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetGroupCustomerQueryHandler(
            IGroupCustomerRepository GroupCustomerRepository,
            IMapper mapper,
            ILogger<GetGroupCustomerQueryHandler> logger)
        {
            _GroupCustomerRepository = GroupCustomerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetGroupCustomerQuery to retrieve a specific GroupCustomer.
        /// </summary>
        /// <param name="request">The GetGroupCustomerQuery containing GroupCustomer ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CUSTOMER.DATA.Entity.GroupCustomer>> Handle(GetGroupCustomerQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the GroupCustomer entity with the specified ID from the repository
                var entity = await _GroupCustomerRepository.FindAsync(request.GroupCustomerId);
                if (entity != null)
                {

                    var GroupCustomer = _mapper.Map<CUSTOMER.DATA.Entity.GroupCustomer>(entity);
                    return ServiceResponse<CUSTOMER.DATA.Entity.GroupCustomer>.ReturnResultWith200(GroupCustomer);
                }
                else
                {
                    // If the GroupCustomer entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("GroupCustomer not found.");
                    return ServiceResponse<CUSTOMER.DATA.Entity.GroupCustomer>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting GroupCustomer: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<CUSTOMER.DATA.Entity.GroupCustomer>.Return500(e);
            }
        }
    }

}
