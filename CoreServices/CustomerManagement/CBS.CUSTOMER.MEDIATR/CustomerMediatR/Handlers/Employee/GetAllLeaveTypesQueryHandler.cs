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
    /// Handles the request to retrieve a specific AllLeaveTypes based on its unique identifier.
    /// </summary>
    public class GetAllLeaveTypesQueryHandler : IRequestHandler<GetAllLeaveTypeQuery, ServiceResponse<List<GetLeaveType>>>
    {
        private readonly ILeaveTypeRepository _LeaveTypeRepository; // Repository for accessingAllLeaveTypes data.

      
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllLeaveTypesQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetLeaveTypeQueryHandler.
        /// </summary>
        /// <param name="AllLeaveTypeLeavesRepository">Repository forAllLeaveTypes data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllLeaveTypesQueryHandler(
            ILeaveTypeRepository LeaveTypeRepository,
        
            IMapper mapper,
            ILogger<GetAllLeaveTypesQueryHandler> logger)
        {
            _LeaveTypeRepository = LeaveTypeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllLeaveTypeQuery to retrieve a specificAllLeaveTypes.
        /// </summary>
        /// <param name="request">The GetLeaveTypeQuery containing AllLeaveTypesAllLeaveTypes ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<GetLeaveType>>> Handle(GetAllLeaveTypeQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve theAllLeaveTypes entity with the specified ID from the repository
                var entity = await _LeaveTypeRepository.All.ToListAsync();
                if (entity != null)
                {
                    // Map theAllLeaveTypes entity toAllLeaveTypes and return it with a success response
                    var AllLeaveTypes = _mapper.Map<List<GetLeaveType>>(entity);

               


                    return ServiceResponse<List<GetLeaveType>>.ReturnResultWith200(AllLeaveTypes);
                }
                else
                {
                    // If theAllLeaveTypes entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("AllLeaveTypeLeaves not found.");
                    return ServiceResponse<List<GetLeaveType>>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while gettingAllLeaveTypes: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<List<GetLeaveType>>.Return500(e);
            }
        }
    }

}
