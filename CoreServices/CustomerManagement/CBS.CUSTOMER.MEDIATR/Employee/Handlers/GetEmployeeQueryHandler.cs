using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;
using CBS.CUSTOMER.REPOSITORY;

using Microsoft.EntityFrameworkCore;
using CBS.CUSTOMER.DATA.Dto;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Employee based on its unique identifier.
    /// </summary>
    public class GetEmployeeQueryHandler : IRequestHandler<GetEmployeeQuery, ServiceResponse<GetEmployee>>
    {
        private readonly IEmployeeRepository _EmployeeRepository; // Repository for accessing Employee data.
      
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetEmployeeQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetEmployeeQueryHandler.
        /// </summary>
        /// <param name="EmployeeRepository">Repository for Employee data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetEmployeeQueryHandler(
            IEmployeeRepository EmployeeRepository,
        
            IMapper mapper,
            ILogger<GetEmployeeQueryHandler> logger)
        {
            _EmployeeRepository = EmployeeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetEmployeeQuery to retrieve a specific Employee.
        /// </summary>
        /// <param name="request">The GetEmployeeQuery containing Employee ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<GetEmployee>> Handle(GetEmployeeQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the Employee entity with the specified ID from the repository
                var entity = await _EmployeeRepository.AllIncluding(e =>e.Department,r=>r.JobTitle,t=>t.EmployeeLeaves,o=>o.EmployeeTrainings).FirstOrDefaultAsync(x=>x.EmployeeId==request.EmployeeId && x.IsDeleted==false);
                if (entity != null)
                {
                    // Map the Employee entity to Employee and return it with a success response
                    var Employee = _mapper.Map<GetEmployee>(entity);

               


                    return ServiceResponse<GetEmployee>.ReturnResultWith200(Employee);
                }
                else
                {
                    // If the Employee entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Employee not found.");
                    return ServiceResponse<GetEmployee>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Employee: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<GetEmployee>.Return500(e);
            }
        }
    }

}
