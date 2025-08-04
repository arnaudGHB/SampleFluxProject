using AutoMapper;
using CBS.CUSTOMER.DATA.Entity.Customers;
using CBS.CUSTOMER.REPOSITORY.CustomerRepo;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.MEDIATR.Customer.Command;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CBS.CUSTOMER.DATA.Dto.Customers;

namespace CBS.CUSTOMER.MEDIATR.Customer.Handlers
{
    /// <summary>
    /// Handles the command to add a new Customer Age Category.
    /// </summary>
    public class AddCustomerAgeCategoryCommandHandler : IRequestHandler<AddCustomerAgeCategoryCommand, ServiceResponse<CustomerAgeCategoryDto>>
    {
        private readonly ICustomerAgeCategoryRepository _repository; // Repository for accessing CustomerAgeCategory data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow; // Unit of Work for handling transactions.
        private readonly ILogger<AddCustomerAgeCategoryCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _UserInfoToken; // User token information for logging.

        /// <summary>
        /// Constructor for initializing the AddCustomerAgeCategoryCommandHandler.
        /// </summary>
        public AddCustomerAgeCategoryCommandHandler(
            ICustomerAgeCategoryRepository repository,
            IMapper mapper,
            IUnitOfWork<POSContext> uow,
            ILogger<AddCustomerAgeCategoryCommandHandler> logger,
            UserInfoToken userInfoToken)
        {
            _repository = repository;
            _mapper = mapper;
            _uow = uow;
            _logger = logger;
            _UserInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the AddCustomerAgeCategoryCommand to add a new Customer Age Category.
        /// </summary>
        /// <param name="request">The AddCustomerAgeCategoryCommand containing Age Category data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CustomerAgeCategoryDto>> Handle(AddCustomerAgeCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a category with the provided name already exists
                var existingCategory = await _repository.FindBy(x => x.Name == request.Name && !x.IsDeleted).FirstOrDefaultAsync();
                if (existingCategory != null)
                {
                    return ServiceResponse<CustomerAgeCategoryDto>.Return409($"Category '{request.Name}' already exists.");
                }

                // Map the command to an entity
                var category = _mapper.Map<CustomerAgeCategory>(request);
                category.Id = BaseUtilities.GenerateUniqueNumber(); // Generate a unique identifier

                // Add the new category entity to the repository
                _repository.Add(category);
                await _uow.SaveAsync();

                // Log the successful creation
                await APICallHelper.AuditLogger(
                    _UserInfoToken.Email,
                    LogAction.Create.ToString(),
                    category,
                    $"Creating new CustomerAgeCategory '{request.Name}' Successful.",
                    LogLevelInfo.Information.ToString(),
                    200,
                    _UserInfoToken.Token
                );

                return ServiceResponse<CustomerAgeCategoryDto>.ReturnResultWith200(_mapper.Map<CustomerAgeCategoryDto>(request));
            }
            catch (Exception e)
            {
                // Log the error
                var errorMessage = $"Error occurred while saving CustomerAgeCategory: {e.Message}";
                _logger.LogError(errorMessage);

                await APICallHelper.AuditLogger(
                    _UserInfoToken.Email,
                    LogAction.Create.ToString(),
                    e.Message,
                    $"Internal Server Error occurred while saving new CustomerAgeCategory '{request.Name}'.",
                    LogLevelInfo.Error.ToString(),
                    500,
                    _UserInfoToken.Token
                );

                return ServiceResponse<CustomerAgeCategoryDto>.Return500(e);
            }
        }
    }

}
