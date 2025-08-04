using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to update a AccountCategory based on UpdateAccountCategoryCommand.
    /// </summary>
    public class UpdateAccountCategoryCommandHandler : IRequestHandler<UpdateAccountCartegoryCommand, ServiceResponse<AccountCartegoryDto>>
    {
        private readonly IAccountCategoryRepository _AccountCategoryRepository; // Repository for accessing AccountCategory data.
        private readonly ILogger<UpdateAccountCategoryCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateAccountCategoryCommandHandler.
        /// </summary>
        /// <param name="AccountCategoryRepository">Repository for AccountCategory data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateAccountCategoryCommandHandler(
            IAccountCategoryRepository AccountCategoryRepository,
            ILogger<UpdateAccountCategoryCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<POSContext> uow = null)
        {
            _AccountCategoryRepository = AccountCategoryRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateAccountCategoryCommand to update a AccountCategory.
        /// </summary>
        /// <param name="request">The UpdateAccountCategoryCommand containing updated AccountCategory data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AccountCartegoryDto>> Handle(UpdateAccountCartegoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the AccountCategory entity to be updated from the repository
                var existingAccountCategory = await _AccountCategoryRepository.FindAsync(request.Id);

                // Check if the AccountCategory entity exists
                if (existingAccountCategory != null)
                {
                    // Update AccountCategory entity properties with values from the request
                    existingAccountCategory.Name = request.Name;
                    existingAccountCategory.Description = request.Description;

                    // Use the repository to update the existing AccountCategory entity
                    _AccountCategoryRepository.Update(existingAccountCategory);
                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<AccountCartegoryDto>.Return500();
                    }
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<AccountCartegoryDto>.ReturnResultWith200(_mapper.Map<AccountCartegoryDto>(existingAccountCategory));
                    _logger.LogInformation($"AccountCategory {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the AccountCategory entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<AccountCartegoryDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating AccountCategory: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<AccountCartegoryDto>.Return500(e);
            }
        }
    }
}