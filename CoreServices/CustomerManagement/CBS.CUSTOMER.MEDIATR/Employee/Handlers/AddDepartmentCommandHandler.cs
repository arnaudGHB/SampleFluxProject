
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

namespace CBS.CUSTOMER.MEDIATR.DepartmentMediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Department.
    /// </summary>
    public class AddDepartmentCommandHandler : IRequestHandler<AddDepartmentCommand, ServiceResponse<CreateDepartment>>
    {
        private readonly IDepartmentRepository _DepartmentRepository; // Repository for accessing Department data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddDepartmentCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddDepartmentCommandHandler.
        /// </summary>
        /// <param name="DepartmentRepository">Repository for Department data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddDepartmentCommandHandler(
            IDepartmentRepository DepartmentRepository,
            IMapper mapper,
            ILogger<AddDepartmentCommandHandler> logger,
            IUnitOfWork<POSContext> uow)
        {
            _DepartmentRepository = DepartmentRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddDepartmentCommand to add a new Department.
        /// </summary>
        /// <param name="request">The AddDepartmentCommand containing Department data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CreateDepartment>> Handle(AddDepartmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a Department with the same name already exists (case-insensitive)
                var existingDepartment = await _DepartmentRepository.FindBy(c => c.DepartmentName == (request.DepartmentName)).FirstOrDefaultAsync();

                // If a Department with the same name already exists, return a conflict response
                if (existingDepartment != null)
                {
                    var errorMessage = $"Department {(request.DepartmentName)} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<CreateDepartment>.Return409(errorMessage);
                }

                // Map the AddDepartmentCommand to a Department entity
                var DepartmentEntity = _mapper.Map<CUSTOMER.DATA.Entity.Department>(request);

               

                DepartmentEntity.CreatedDate = BaseUtilities.UtcToLocal(DateTime.UtcNow);

                //Auto generateKey For Unique Identification
                //DepartmentEntity.DepartmentId = BaseUtilities.GenerateUniqueNumber();
 
                // Add the new Department entity to the repository
                _DepartmentRepository.Add(DepartmentEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<CreateDepartment>.Return500();
                }
                // Map the Department entity to CreateDepartment and return it with a success response
                var CreateDepartment = _mapper.Map<CreateDepartment>(DepartmentEntity);
                return ServiceResponse<CreateDepartment>.ReturnResultWith200(CreateDepartment);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Department: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<CreateDepartment>.Return500(e);
            }
        }
    }

}
