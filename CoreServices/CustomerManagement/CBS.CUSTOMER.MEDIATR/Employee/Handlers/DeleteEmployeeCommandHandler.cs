using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands;

namespace CBS.Employee.MEDIATR.EmployeeMediatR.Handlers
{
    /// <summary>
    /// Handles the command to delete a Employee based on DeleteEmployeeCommand.
    /// </summary>
    public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, ServiceResponse<bool>>
    {
        private readonly IEmployeeRepository _EmployeeRepository; // Repository for accessing Employee data.
        private readonly ILogger<DeleteEmployeeCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteEmployeeCommandHandler.
        /// </summary>
        /// <param name="EmployeeRepository">Repository for Employee data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteEmployeeCommandHandler(
            IEmployeeRepository EmployeeRepository, IMapper mapper,
            ILogger<DeleteEmployeeCommandHandler> logger
, IUnitOfWork<POSContext> uow)
        {
            _EmployeeRepository = EmployeeRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteEmployeeCommand to delete a Employee.
        /// </summary>
        /// <param name="request">The DeleteEmployeeCommand containing Employee ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the Employee entity with the specified ID exists
                var existingEmployee = await _EmployeeRepository.FindAsync(request.Id);
                if (existingEmployee == null)
                {
                    errorMessage = $"Employee with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingEmployee.IsDeleted = true;
                existingEmployee.DeletedBy = request.UserId;
                _EmployeeRepository.Update(existingEmployee);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting Employee: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
