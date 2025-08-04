using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.MEDIATR.Customer.Command;
using CBS.CUSTOMER.REPOSITORY.CustomerRepo;
using Microsoft.EntityFrameworkCore;

namespace CBS.CUSTOMER.MEDIATR.Customer.Handlers
{
    /// <summary>
    /// Handles the command to delete a Customer Age Category.
    /// </summary>
    public class DeleteCustomerAgeCategoryCommandHandler : IRequestHandler<DeleteCustomerAgeCategoryCommand, ServiceResponse<bool>>
    {
        private readonly ICustomerAgeCategoryRepository _repository; // Repository for accessing CustomerAgeCategory data.
        private readonly IUnitOfWork<POSContext> _uow; // Unit of Work for handling transactions.
        private readonly ILogger<DeleteCustomerAgeCategoryCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _UserInfoToken; // User token information for logging.

        /// <summary>
        /// Constructor for initializing the DeleteCustomerAgeCategoryCommandHandler.
        /// </summary>
        public DeleteCustomerAgeCategoryCommandHandler(
            ICustomerAgeCategoryRepository repository,
            IUnitOfWork<POSContext> uow,
            ILogger<DeleteCustomerAgeCategoryCommandHandler> logger,
            UserInfoToken userInfoToken)
        {
            _repository = repository;
            _uow = uow;
            _logger = logger;
            _UserInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the DeleteCustomerAgeCategoryCommand to delete a Customer Age Category.
        /// </summary>
        /// <param name="request">The DeleteCustomerAgeCategoryCommand containing the ID of the category to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteCustomerAgeCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the category to be deleted
                var category = await _repository.FindBy(x => x.Id == request.Id && !x.IsDeleted).FirstOrDefaultAsync();
                if (category == null)
                {
                    return ServiceResponse<bool>.Return404("Category not found.");
                }

                // Mark the category as deleted
                category.IsDeleted = true;
                _repository.Update(category);

                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }

                // Log the successful deletion
                await APICallHelper.AuditLogger(
                    _UserInfoToken.Email,
                    LogAction.Delete.ToString(),
                    category,
                    $"Deleting CustomerAgeCategory with ID '{request.Id}' Successful.",
                    LogLevelInfo.Information.ToString(),
                    200,
                    _UserInfoToken.Token
                );

                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                // Log the error
                _logger.LogError(e, "Error deleting CustomerAgeCategory");

                await APICallHelper.AuditLogger(
                    _UserInfoToken.Email,
                    LogAction.Delete.ToString(),
                    e.Message,
                    $"Internal Server Error occurred while deleting CustomerAgeCategory with ID '{request.Id}'.",
                    LogLevelInfo.Error.ToString(),
                    500,
                    _UserInfoToken.Token
                );

                return ServiceResponse<bool>.Return500(e);
            }
        }
    }


}
