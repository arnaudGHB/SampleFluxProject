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
    /// Handles the request to retrieve a specific AllDepartments based on its unique identifier.
    /// </summary>
    public class GetAllDepartmentsQueryHandler : IRequestHandler<GetAllDepartmentQuery, ServiceResponse<List<GetDepartment>>>
    {
        private readonly IDepartmentRepository _DepartmentRepository; // Repository for accessingAllDepartments data.

      
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllDepartmentsQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetDepartmentQueryHandler.
        /// </summary>
        /// <param name="AllDepartmentLeavesRepository">Repository forAllDepartments data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllDepartmentsQueryHandler(
            IDepartmentRepository DepartmentRepository,
        
            IMapper mapper,
            ILogger<GetAllDepartmentsQueryHandler> logger)
        {
            _DepartmentRepository = DepartmentRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllDepartmentQuery to retrieve a specificAllDepartments.
        /// </summary>
        /// <param name="request">The GetDepartmentQuery containing AllDepartmentsAllDepartments ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<GetDepartment>>> Handle(GetAllDepartmentQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve theAllDepartments entity with the specified ID from the repository
                var entity = await _DepartmentRepository.All.ToListAsync();
                if (entity != null)
                {
                    // Map theAllDepartments entity toAllDepartments and return it with a success response
                    var AllDepartments = _mapper.Map<List<GetDepartment>>(entity);

               


                    return ServiceResponse<List<GetDepartment>>.ReturnResultWith200(AllDepartments);
                }
                else
                {
                    // If theAllDepartments entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("AllDepartmentLeaves not found.");
                    return ServiceResponse<List<GetDepartment>>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while gettingAllDepartments: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<List<GetDepartment>>.Return500(e);
            }
        }
    }

}
