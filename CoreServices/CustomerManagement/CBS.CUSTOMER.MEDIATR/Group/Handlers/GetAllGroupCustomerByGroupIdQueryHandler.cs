using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.MEDIATR;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace CBS.GroupCustomer.MEDIATR.GroupCustomerMediatR
{
    /// <summary>
    /// Handles the retrieval of all GroupCustomers based on the GetAllGroupCustomerByGroupIdQuery.
    /// </summary>
    public class GetAllGroupCustomerByGroupIdQueryHandler : IRequestHandler<GetAllGroupCustomerByGroupIdQuery, ServiceResponse<List<CUSTOMER.DATA.Entity.GroupCustomer>>>
    {
        private readonly IGroupCustomerRepository _GroupCustomerRepository; // Repository for accessing GroupCustomers data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllGroupCustomerByGroupIdQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllGroupCustomerByGroupIdQueryHandler.
        /// </summary>
        /// <param name="GroupCustomerRepository">Repository for GroupCustomers data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllGroupCustomerByGroupIdQueryHandler(
            IGroupCustomerRepository GroupCustomerRepository,
            IMapper mapper, ILogger<GetAllGroupCustomerByGroupIdQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _GroupCustomerRepository = GroupCustomerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllGroupCustomerByGroupIdQuery to retrieve all GroupCustomers.
        /// </summary>
        /// <param name="request">The GetAllGroupCustomerByGroupIdQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<CUSTOMER.DATA.Entity.GroupCustomer>>> Handle(GetAllGroupCustomerByGroupIdQuery request, CancellationToken cancellationToken)
        {
            try
            {

                // Retrieve all GroupCustomers entities from the repository
                var entities = _mapper.Map<List<CUSTOMER.DATA.Entity.GroupCustomer>>(await _GroupCustomerRepository.All.Where(x=>x.GroupId==request.GroupId).FirstOrDefaultAsync(s =>s.IsDeleted == false));

                return ServiceResponse<List<CUSTOMER.DATA.Entity.GroupCustomer>>.ReturnResultWith200(entities);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all GroupCustomers: {e.Message}");
                return ServiceResponse<List<CUSTOMER.DATA.Entity.GroupCustomer>>.Return500(e, "Failed to get all GroupCustomers");
            }
        }
    }
}
