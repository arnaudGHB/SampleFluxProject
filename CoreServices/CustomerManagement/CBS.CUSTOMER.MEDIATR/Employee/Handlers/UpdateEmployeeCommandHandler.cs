
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

namespace CBS.Employee.MEDIATR.EmployeeMediatR.Handlers
{
    /// <summary>
    /// Handles the command to Update a new Employee.
    /// </summary>
    public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, ServiceResponse<UpdateEmployee>>
    {
        private readonly IEmployeeRepository _EmployeeRepository; // Repository for accessing Employee data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<UpdateEmployeeCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        /// <summary>
        /// Constructor for initializing the UpdateEmployeeCommandHandler.
        /// </summary>
        /// <param name="EmployeeRepository">Repository for Employee data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public UpdateEmployeeCommandHandler(
            IEmployeeRepository EmployeeRepository,
            IMapper mapper,
            ILogger<UpdateEmployeeCommandHandler> logger,
            IUnitOfWork<POSContext> uow)
        {
            _EmployeeRepository = EmployeeRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateEmployeeCommand to Update a new Employee.
        /// </summary>
        /// <param name="request">The UpdateEmployeeCommand containing Employee data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<UpdateEmployee>> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                /*// Check if a Employee with the same name already exists (case-insensitive)
                var existingEmployee = await _EmployeeRepository.FindBy(c => c.E == (request.EmployeeName)).FirstOrDefaultAsync();

                // If a Employee with the same name already exists, return a conflict response
                if (existingEmployee != null)
                {
                    var errorMessage = $"Employee {(request.EmployeeName)} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<UpdateEmployee>.Return409(errorMessage);
                }*/

                // Map the UpdateEmployeeCommand to a Employee entity
                var EmployeeEntity = _mapper.Map<CUSTOMER.DATA.Entity.Employee>(request);

               

                EmployeeEntity.ModifiedDate = BaseUtilities.UtcToLocal(DateTime.UtcNow);

                //Auto generateKey For Unique Identification
                EmployeeEntity.EmployeeId = BaseUtilities.GenerateUniqueNumber();
 
                // Update the new Employee entity to the repository
                _EmployeeRepository.Update(EmployeeEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<UpdateEmployee>.Return500();
                }
                // Map the Employee entity to UpdateEmployee and return it with a success response
                var UpdateEmployee = _mapper.Map<UpdateEmployee>(EmployeeEntity);
                return ServiceResponse<UpdateEmployee>.ReturnResultWith200(UpdateEmployee);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Employee: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<UpdateEmployee>.Return500(e);
            }
        }
    }

}
