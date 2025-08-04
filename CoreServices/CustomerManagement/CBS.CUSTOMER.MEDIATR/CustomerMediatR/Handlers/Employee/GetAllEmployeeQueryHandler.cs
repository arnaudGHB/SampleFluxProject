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
    /// Handles the request to retrieve a specific AllEmployee based on its unique identifier.
    /// </summary>
    public class GetAllEmployeeQueryHandler : IRequestHandler<GetAllEmployeeQuery, ServiceResponse<List<GetAllEmployees>>>
    {
        private readonly IEmployeeRepository _EmployeeRepository; // Repository for accessing AllEmployee data.

      
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllEmployeeQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllEmployeeQueryHandler.
        /// </summary>
        /// <param name="AllEmployeeRepository">Repository for AllEmployee data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllEmployeeQueryHandler(
            IEmployeeRepository EmployeeRepository,
        
            IMapper mapper,
            ILogger<GetAllEmployeeQueryHandler> logger)
        {
            _EmployeeRepository = EmployeeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllEmployeeQuery to retrieve a specific AllEmployee.
        /// </summary>
        /// <param name="request">The GetAllEmployeeQuery containing AllEmployee ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<GetAllEmployees>>> Handle(GetAllEmployeeQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the AllEmployee entity with the specified ID from the repository
                var entity = await _EmployeeRepository.All.ToListAsync();
                if (entity != null)
                {
                    // Map the AllEmployee entity to AllEmployee and return it with a success response
                    var AllEmployee = _mapper.Map<List<GetAllEmployees>>(entity);

               


                    return ServiceResponse<List<GetAllEmployees>>.ReturnResultWith200(AllEmployee);
                }
                else
                {
                    // If the AllEmployee entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("AllEmployee not found.");
                    return ServiceResponse<List<GetAllEmployees>>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting AllEmployee: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<List<GetAllEmployees>>.Return500(e);
            }
        }
    }

}
