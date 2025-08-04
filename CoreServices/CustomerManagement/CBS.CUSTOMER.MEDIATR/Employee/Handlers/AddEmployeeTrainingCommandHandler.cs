
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

namespace CBS.EmployeeTraining.MEDIATR.EmployeeTrainingMediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new EmployeeTraining.
    /// </summary>
    public class AddEmployeeTrainingCommandHandler : IRequestHandler<AddEmployeeTrainingCommand, ServiceResponse<CreateEmployeeTraining>>
    {
        private readonly IEmployeeTrainingRepository _EmployeeTrainingRepository; // Repository for accessing EmployeeTraining data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddEmployeeTrainingCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddEmployeeTrainingCommandHandler.
        /// </summary>
        /// <param name="EmployeeTrainingRepository">Repository for EmployeeTraining data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddEmployeeTrainingCommandHandler(
            IEmployeeTrainingRepository EmployeeTrainingRepository,
            IMapper mapper,
            ILogger<AddEmployeeTrainingCommandHandler> logger,
            IUnitOfWork<POSContext> uow)
        {
            _EmployeeTrainingRepository = EmployeeTrainingRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddEmployeeTrainingCommand to add a new EmployeeTraining.
        /// </summary>
        /// <param name="request">The AddEmployeeTrainingCommand containing EmployeeTraining data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CreateEmployeeTraining>> Handle(AddEmployeeTrainingCommand request, CancellationToken cancellationToken)
        {
            try
            {
               

                // Map the AddEmployeeTrainingCommand to a EmployeeTraining entity
                var EmployeeTrainingEntity = _mapper.Map<CUSTOMER.DATA.Entity.EmployeeTraining>(request);

               

                EmployeeTrainingEntity.CreatedDate = BaseUtilities.UtcToLocal(DateTime.UtcNow);

                //Auto generateKey For Unique Identification
                //EmployeeTrainingEntity.EmployeeTrainingId = BaseUtilities.GenerateUniqueNumber();
 
                // Add the new EmployeeTraining entity to the repository
                _EmployeeTrainingRepository.Add(EmployeeTrainingEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<CreateEmployeeTraining>.Return500();
                }
                // Map the EmployeeTraining entity to CreateEmployeeTraining and return it with a success response
                var CreateEmployeeTraining = _mapper.Map<CreateEmployeeTraining>(EmployeeTrainingEntity);
                return ServiceResponse<CreateEmployeeTraining>.ReturnResultWith200(CreateEmployeeTraining);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving EmployeeTraining: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<CreateEmployeeTraining>.Return500(e);
            }
        }
    }

}
