
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

namespace CBS.EmployeeLeave.MEDIATR.EmployeeLeaveMediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new EmployeeLeave.
    /// </summary>
    public class AddEmployeeLeaveCommandHandler : IRequestHandler<AddEmployeeLeaveCommand, ServiceResponse<CreateEmployeeLeave>>
    {
        private readonly IEmployeeLeaveRepository _EmployeeLeaveRepository; // Repository for accessing EmployeeLeave data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddEmployeeLeaveCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddEmployeeLeaveCommandHandler.
        /// </summary>
        /// <param name="EmployeeLeaveRepository">Repository for EmployeeLeave data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddEmployeeLeaveCommandHandler(
            IEmployeeLeaveRepository EmployeeLeaveRepository,
            IMapper mapper,
            ILogger<AddEmployeeLeaveCommandHandler> logger,
            IUnitOfWork<POSContext> uow)
        {
            _EmployeeLeaveRepository = EmployeeLeaveRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddEmployeeLeaveCommand to add a new EmployeeLeave.
        /// </summary>
        /// <param name="request">The AddEmployeeLeaveCommand containing EmployeeLeave data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CreateEmployeeLeave>> Handle(AddEmployeeLeaveCommand request, CancellationToken cancellationToken)
        {
            try
            {
          
                // Map the AddEmployeeLeaveCommand to a EmployeeLeave entity
                var EmployeeLeaveEntity = _mapper.Map<CUSTOMER.DATA.Entity.EmployeeLeave>(request);

               

                EmployeeLeaveEntity.CreatedDate = BaseUtilities.UtcToLocal(DateTime.UtcNow);

                //Auto generateKey For Unique Identification
                //EmployeeLeaveEntity.EmployeeLeaveId = BaseUtilities.GenerateUniqueNumber();
 
                // Add the new EmployeeLeave entity to the repository
                _EmployeeLeaveRepository.Add(EmployeeLeaveEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<CreateEmployeeLeave>.Return500();
                }
                // Map the EmployeeLeave entity to CreateEmployeeLeave and return it with a success response
                var CreateEmployeeLeave = _mapper.Map<CreateEmployeeLeave>(EmployeeLeaveEntity);
                return ServiceResponse<CreateEmployeeLeave>.ReturnResultWith200(CreateEmployeeLeave);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving EmployeeLeave: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<CreateEmployeeLeave>.Return500(e);
            }
        }
    }

}
