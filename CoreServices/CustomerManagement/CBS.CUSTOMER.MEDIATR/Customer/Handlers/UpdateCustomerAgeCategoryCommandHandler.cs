using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.MEDIATR.Customer.Command;
using CBS.CUSTOMER.DATA.Entity.Customers;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.REPOSITORY.CustomerRepo;
using Microsoft.EntityFrameworkCore;
using CBS.CUSTOMER.DATA.Dto.Customers;

namespace CBS.CUSTOMER.MEDIATR.Customer.Handlers
{
    /// <summary>
    /// Handles the command to update an existing Customer Age Category.
    /// </summary>
    public class UpdateCustomerAgeCategoryCommandHandler : IRequestHandler<UpdateCustomerAgeCategoryCommand, ServiceResponse<CustomerAgeCategoryDto>>
    {
        private readonly ICustomerAgeCategoryRepository _repository; // Repository for accessing CustomerAgeCategory data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow; // Unit of Work for handling transactions.
        private readonly ILogger<UpdateCustomerAgeCategoryCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _UserInfoToken; // User token information for logging.

        /// <summary>
        /// Constructor for initializing the UpdateCustomerAgeCategoryCommandHandler.
        /// </summary>
        public UpdateCustomerAgeCategoryCommandHandler(
            ICustomerAgeCategoryRepository repository,
            IMapper mapper,
            IUnitOfWork<POSContext> uow,
            ILogger<UpdateCustomerAgeCategoryCommandHandler> logger,
            UserInfoToken userInfoToken)
        {
            _repository = repository;
            _mapper = mapper;
            _uow = uow;
            _logger = logger;
            _UserInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the UpdateCustomerAgeCategoryCommand to update an existing Customer Age Category.
        /// </summary>
        /// <param name="request">The UpdateCustomerAgeCategoryCommand containing updated data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CustomerAgeCategoryDto>> Handle(UpdateCustomerAgeCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Find the existing category by name
                var category = await _repository.FindBy(x => x.Name == request.Name && !x.IsDeleted).FirstOrDefaultAsync();
                if (category == null)
                {
                    return ServiceResponse<CustomerAgeCategoryDto>.Return404("Category not found.");
                }

                // Map the updated values onto the existing category entity
                _mapper.Map(request, category);

                // Update the entity in the repository
                _repository.Update(category);
                await _uow.SaveAsync();

                // Log the successful update
                await APICallHelper.AuditLogger(
                    _UserInfoToken.Email,
                    LogAction.Update.ToString(),
                    category,
                    $"Updating CustomerAgeCategory '{request.Name}' Successful.",
                    LogLevelInfo.Information.ToString(),
                    200,
                    _UserInfoToken.Token
                );

                return ServiceResponse<CustomerAgeCategoryDto>.ReturnResultWith200(_mapper.Map<CustomerAgeCategoryDto>(category));
            }
            catch (Exception e)
            {
                // Log the error
                var errorMessage = $"Error occurred while updating CustomerAgeCategory: {e.Message}";
                _logger.LogError(errorMessage);

                await APICallHelper.AuditLogger(
                    _UserInfoToken.Email,
                    LogAction.Update.ToString(),
                    e.Message,
                    $"Internal Server Error occurred while updating CustomerAgeCategory '{request.Name}'.",
                    LogLevelInfo.Error.ToString(),
                    500,
                    _UserInfoToken.Token
                );

                return ServiceResponse<CustomerAgeCategoryDto>.Return500(e);
            }
        }
    }

}
