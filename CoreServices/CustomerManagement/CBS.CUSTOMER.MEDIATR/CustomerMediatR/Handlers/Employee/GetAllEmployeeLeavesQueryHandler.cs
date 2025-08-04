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
    /// Handles the request to retrieve a specific AllEmployeeLeaves based on its unique identifier.
    /// </summary>
    public class GetAllEmployeeLeavesQueryHandler : IRequestHandler<GetAllEmployeeLeaveQuery, ServiceResponse<List<GetEmployeeLeave>>>
    {
        private readonly IEmployeeLeaveRepository _EmployeeLeaveRepository; // Repository for accessingAllEmployeeLeaves data.

      
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllEmployeeLeavesQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetEmployeeLeaveQueryHandler.
        /// </summary>
        /// <param name="AllEmployeeLeaveLeavesRepository">Repository forAllEmployeeLeaves data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllEmployeeLeavesQueryHandler(
            IEmployeeLeaveRepository EmployeeLeaveRepository,
        
            IMapper mapper,
            ILogger<GetAllEmployeeLeavesQueryHandler> logger)
        {
            _EmployeeLeaveRepository = EmployeeLeaveRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllEmployeeLeaveQuery to retrieve a specificAllEmployeeLeaves.
        /// </summary>
        /// <param name="request">The GetEmployeeLeaveQuery containing AllEmployeeLeavesAllEmployeeLeaves ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<GetEmployeeLeave>>> Handle(GetAllEmployeeLeaveQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve theAllEmployeeLeaves entity with the specified ID from the repository
                var entity = await _EmployeeLeaveRepository.All.ToListAsync();
                if (entity != null)
                {
                    // Map theAllEmployeeLeaves entity toAllEmployeeLeaves and return it with a success response
                    var AllEmployeeLeaves = _mapper.Map<List<GetEmployeeLeave>>(entity);

               


                    return ServiceResponse<List<GetEmployeeLeave>>.ReturnResultWith200(AllEmployeeLeaves);
                }
                else
                {
                    // If theAllEmployeeLeaves entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("AllEmployeeLeaveLeaves not found.");
                    return ServiceResponse<List<GetEmployeeLeave>>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while gettingAllEmployeeLeaves: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<List<GetEmployeeLeave>>.Return500(e);
            }
        }
    }

}
