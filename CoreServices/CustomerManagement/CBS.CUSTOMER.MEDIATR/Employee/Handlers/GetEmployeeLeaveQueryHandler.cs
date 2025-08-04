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
    /// Handles the request to retrieve a specific EmployeeLeave based on its unique identifier.
    /// </summary>
    public class GetEmployeeLeaveQueryHandler : IRequestHandler<GetEmployeeLeaveQuery, ServiceResponse<GetEmployeeLeave>>
    {
        private readonly IEmployeeLeaveRepository _EmployeeLeaveRepository; // Repository for accessing EmployeeLeave data.
       
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetEmployeeLeaveQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetEmployeeLeaveQueryHandler.
        /// </summary>
        /// <param name="EmployeeLeaveRepository">Repository for EmployeeLeave data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetEmployeeLeaveQueryHandler(
            IEmployeeLeaveRepository EmployeeLeaveRepository,
        
            IMapper mapper,
            ILogger<GetEmployeeLeaveQueryHandler> logger)
        {
            _EmployeeLeaveRepository = EmployeeLeaveRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetEmployeeLeaveQuery to retrieve a specific EmployeeLeave.
        /// </summary>
        /// <param name="request">The GetEmployeeLeaveQuery containing EmployeeLeave ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<GetEmployeeLeave>> Handle(GetEmployeeLeaveQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the EmployeeLeave entity with the specified ID from the repository
                var entity =  _EmployeeLeaveRepository.FindBy(x=>x.EmployeeLeaveId==request.EmployeeLeaveId && x.IsDeleted == false);
                if (entity != null)
                {
                    // Map the EmployeeLeave entity to EmployeeLeave and return it with a success response
                    var EmployeeLeave = _mapper.Map<GetEmployeeLeave>(entity);

               


                    return ServiceResponse<GetEmployeeLeave>.ReturnResultWith200(EmployeeLeave);
                }
                else
                {
                    // If the EmployeeLeave entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("EmployeeLeave not found.");
                    return ServiceResponse<GetEmployeeLeave>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting EmployeeLeave: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<GetEmployeeLeave>.Return500(e);
            }
        }
    }

}
