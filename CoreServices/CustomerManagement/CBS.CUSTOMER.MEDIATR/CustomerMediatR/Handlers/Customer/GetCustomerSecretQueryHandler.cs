using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.DATA.Entity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using CBS.CUSTOMER.DATA.Dto.Customers;

namespace CBS.CUSTOMER.MEDIATR
{
    /// <summary>
    /// Handles the request to retrieve a specific Customer based on its unique identifier.
    /// </summary>
    public class GetCustomerSecretQueryHandler : IRequestHandler<GetCustomerSecretByPhoneQuery, ServiceResponse<GetCustomerSecret>>
    {
        private readonly ICustomerRepository _CustomerRepository; // Repository for accessing Customer data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetCustomerSecretQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetCustomerSecretQueryHandler.
        /// </summary>
        /// <param name="CustomerRepository">Repository for Customer data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetCustomerSecretQueryHandler(
            ICustomerRepository CustomerRepository,
            IMapper mapper,
            ILogger<GetCustomerSecretQueryHandler> logger)
        {
            _CustomerRepository = CustomerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetCustomerSecretQuery to retrieve a specific Customer.
        /// </summary>
        /// <param name="request">The GetCustomerSecretQuery containing Customer ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<GetCustomerSecret>> Handle(GetCustomerSecretByPhoneQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
               
                // Retrieve the Customer entity with the specified ID from the repository
                var entity =  _CustomerRepository.FindBy( s=>s.CustomerId==request.CustomerId &&  s.IsDeleted == false);
                if (entity != null)
                {

             
                    var Customer = _mapper.Map<GetCustomerSecret>(entity);
                    return ServiceResponse<GetCustomerSecret>.ReturnResultWith200(Customer);
                }
                else
                {
                    // If the Customer entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Customer not found.");
                    return ServiceResponse<GetCustomerSecret>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Customer: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<GetCustomerSecret>.Return500(e);
            }
        }
    }

}
