
using MediatR;
using AutoMapper;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DOMAIN.Context;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.REPOSITORY;
using Microsoft.EntityFrameworkCore;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.DATA.Dto;
using CBS.Customer.MEDIATR;

namespace CBS.LeaveType.MEDIATR.LeaveTypeMediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new LeaveType.
    /// </summary>
    public class AddLeaveTypeCommandHandler : IRequestHandler<AddLeaveTypeCommand, ServiceResponse<CreateLeaveType>>
    {
        private readonly ILeaveTypeRepository _LeaveTypeRepository; // Repository for accessing LeaveType data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddLeaveTypeCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddLeaveTypeCommandHandler.
        /// </summary>
        /// <param name="LeaveTypeRepository">Repository for LeaveType data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddLeaveTypeCommandHandler(
            ILeaveTypeRepository LeaveTypeRepository,
            IMapper mapper,
            ILogger<AddLeaveTypeCommandHandler> logger,
            IUnitOfWork<POSContext> uow)
        {
            _LeaveTypeRepository = LeaveTypeRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddLeaveTypeCommand to add a new LeaveType.
        /// </summary>
        /// <param name="request">The AddLeaveTypeCommand containing LeaveType data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CreateLeaveType>> Handle(AddLeaveTypeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a LeaveType with the same name already exists (case-insensitive)
                var existingLeaveType = await _LeaveTypeRepository.FindBy(c => c.LeaveTypeName == (request.LeaveTypeName)).FirstOrDefaultAsync();

                // If a LeaveType with the same name already exists, return a conflict response
                if (existingLeaveType != null)
                {
                    var errorMessage = $"LeaveType {(request.LeaveTypeName)} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<CreateLeaveType>.Return409(errorMessage);
                }

                // Map the AddLeaveTypeCommand to a LeaveType entity
                var LeaveTypeEntity = _mapper.Map<CUSTOMER.DATA.Entity.LeaveType>(request);

               

                LeaveTypeEntity.CreatedDate = BaseUtilities.UtcToLocal(DateTime.UtcNow);

                //Auto generateKey For Unique Identification
                //LeaveTypeEntity.LeaveTypeId = BaseUtilities.GenerateUniqueNumber();
 
                // Add the new LeaveType entity to the repository
                _LeaveTypeRepository.Add(LeaveTypeEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<CreateLeaveType>.Return500();
                }
                // Map the LeaveType entity to CreateLeaveType and return it with a success response
                var CreateLeaveType = _mapper.Map<CreateLeaveType>(LeaveTypeEntity);
                return ServiceResponse<CreateLeaveType>.ReturnResultWith200(CreateLeaveType);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving LeaveType: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<CreateLeaveType>.Return500(e);
            }
        }
    }

}
