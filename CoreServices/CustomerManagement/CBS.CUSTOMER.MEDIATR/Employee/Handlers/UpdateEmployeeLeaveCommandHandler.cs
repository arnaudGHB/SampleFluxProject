
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

namespace CBS.CUSTOMER.MEDIATR
{
    /// <summary>
    /// Handles the command to Update a new EmployeeLeave.
    /// </summary>
    public class UpdateEmployeeLeaveCommandHandler : IRequestHandler<UpdateEmployeeLeaveCommand, ServiceResponse<UpdateEmployeeLeave>>
    {
        private readonly IEmployeeLeaveRepository _EmployeeLeaveRepository; // Repository for accessing EmployeeLeave data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<UpdateEmployeeLeaveCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        /// <summary>
        /// Constructor for initializing the UpdateEmployeeLeaveCommandHandler.
        /// </summary>
        /// <param name="EmployeeLeaveRepository">Repository for EmployeeLeave data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public UpdateEmployeeLeaveCommandHandler(
            IEmployeeLeaveRepository EmployeeLeaveRepository,
            IMapper mapper,
            ILogger<UpdateEmployeeLeaveCommandHandler> logger,
            IUnitOfWork<POSContext> uow)
        {
            _EmployeeLeaveRepository = EmployeeLeaveRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateEmployeeLeaveCommand to Update a new EmployeeLeave.
        /// </summary>
        /// <param name="request">The UpdateEmployeeLeaveCommand containing EmployeeLeave data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<UpdateEmployeeLeave>> Handle(UpdateEmployeeLeaveCommand request, CancellationToken cancellationToken)
        {
            try
            {
                /*// Check if a EmployeeLeave with the same name already exists (case-insensitive)
                var existingEmployeeLeave = await _EmployeeLeaveRepository.FindBy(c => c.E == (request.EmployeeLeaveName)).FirstOrDefaultAsync();

                // If a EmployeeLeave with the same name already exists, return a conflict response
                if (existingEmployeeLeave != null)
                {
                    var errorMessage = $"EmployeeLeave {(request.EmployeeLeaveName)} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<UpdateEmployeeLeave>.Return409(errorMessage);
                }*/

                // Map the UpdateEmployeeLeaveCommand to a EmployeeLeave entity
                var EmployeeLeaveEntity = _mapper.Map<DATA.Entity.EmployeeLeave>(request);

               

                EmployeeLeaveEntity.ModifiedDate = BaseUtilities.UtcToLocal(DateTime.UtcNow);

                //Auto generateKey For Unique Identification
                EmployeeLeaveEntity.EmployeeLeaveId = BaseUtilities.GenerateUniqueNumber();
 
                // Update the new EmployeeLeave entity to the repository
                _EmployeeLeaveRepository.Update(EmployeeLeaveEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<UpdateEmployeeLeave>.Return500();
                }
                // Map the EmployeeLeave entity to UpdateEmployeeLeave and return it with a success response
                var UpdateEmployeeLeave = _mapper.Map<UpdateEmployeeLeave>(EmployeeLeaveEntity);
                return ServiceResponse<UpdateEmployeeLeave>.ReturnResultWith200(UpdateEmployeeLeave);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving EmployeeLeave: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<UpdateEmployeeLeave>.Return500(e);
            }
        }
    }

}
