using CBS.CUSTOMER.DATA.Entity.Customers;
using CBS.CUSTOMER.MEDIATR.Customer.Queries;
using CBS.CUSTOMER.REPOSITORY.CustomerRepo;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.DATA.Dto.Customers;
using CBS.CUSTOMER.DATA.Dto;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DOMAIN.Context;

namespace CBS.CUSTOMER.MEDIATR.Customer.Handlers
{
    /// <summary>
    /// Handles the query to retrieve all Customer Age Categories.
    /// </summary>
    public class GetAllCustomerAgeCategoriesQueryHandler : IRequestHandler<GetAllCustomerAgeCategoriesQuery, ServiceResponse<List<CustomerAgeCategoryDto>>>
    {
        private readonly ICustomerAgeCategoryRepository _repository; // Repository for accessing CustomerAgeCategory data.
        private readonly ILogger<GetAllCustomerAgeCategoriesQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly UserInfoToken _UserInfoToken; // User token information for logging.
        private readonly IUnitOfWork<POSContext> _uow; // Unit of Work for handling transactions.

        /// <summary>
        /// Constructor for initializing the GetAllCustomerAgeCategoriesQueryHandler.
        /// </summary>
        public GetAllCustomerAgeCategoriesQueryHandler(
            ICustomerAgeCategoryRepository repository,
            IMapper mapper,
            ILogger<GetAllCustomerAgeCategoriesQueryHandler> logger,
            UserInfoToken userInfoToken,
            IUnitOfWork<POSContext> uow = null)
        {
            _mapper = mapper;
            _repository = repository;
            _logger = logger;
            _UserInfoToken = userInfoToken;
            _uow=uow;
        }

        /// <summary>
        /// Handles the retrieval of all Customer Age Categories. 
        /// If no records are found in the database, a default category is created and returned.
        /// </summary>
        /// <param name="request">The query request to retrieve all customer age categories.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
        /// <returns>
        /// A service response containing a list of customer age category DTOs, or an appropriate error response.
        /// </returns>
        public async Task<ServiceResponse<List<CustomerAgeCategoryDto>>> Handle(GetAllCustomerAgeCategoriesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve all non-deleted customer age categories from the repository.
                var categories = await _repository.All.Where(x => !x.IsDeleted).ToListAsync();

                // Step 2: Check if the category list is empty.
                if (!categories.Any())
                {
                    string logReference = BaseUtilities.GenerateUniqueNumber();  // Create a unique log reference.

                    _logger.LogWarning("No customer age categories found. Creating a default category.");

                    // Step 3: Create a default "Major" category covering ages 0 to 150.
                    var defaultCategory = new CustomerAgeCategory
                    {
                        Id = BaseUtilities.GenerateUniqueNumber(),  // Generate a unique identifier.
                        Name = AgeCategories.Major.ToString(),  // Default category name: "Major".
                        From = 0,  // Age range: starting from 0.
                        To = 150,  // Age range: up to 150.
                        CreatedDate = BaseUtilities.UtcNowToDoualaTime(),  // Current date in the Douala time zone.
                        CreatedBy = _UserInfoToken.Id  // Record the user who created this default category.
                    };

                    // Step 4: Save the default category to the database.
                    _repository.Add(defaultCategory);
                    await _uow.SaveAsync();

                    // Step 5: Log and audit the creation of the default category.
                    await BaseUtilities.LogAndAuditAsync(
                        "Default customer age category created successfully.",
                        defaultCategory,
                        HttpStatusCodeEnum.OK,
                        LogAction.CreateCustomerAgeCategory,
                        LogLevelInfo.Information
                        
                    );

                    // Step 6: Map the newly created category to a DTO for response.
                    var createdCategoryDto = _mapper.Map<List<CustomerAgeCategoryDto>>(new List<CustomerAgeCategory> { defaultCategory });

                    // Step 7: Return the newly created default category in the service response.
                    return ServiceResponse<List<CustomerAgeCategoryDto>>.ReturnResultWith200(createdCategoryDto);
                }

                // Step 8: Map the existing categories to DTOs for response.
                var categoryDtos = _mapper.Map<List<CustomerAgeCategoryDto>>(categories);

                _logger.LogInformation("Retrieved {CategoryCount} customer age categories from the database.", categories.Count);

                // Step 9: Log and audit the retrieval of existing categories.
                await BaseUtilities.LogAndAuditAsync(
                    $"Successfully retrieved {categories.Count} customer age categories.",
                    categories,
                    HttpStatusCodeEnum.OK,
                    LogAction.GetCustomerAgeCategories,
                    LogLevelInfo.Information
                );

                // Step 10: Return the list of customer age category DTOs.
                return ServiceResponse<List<CustomerAgeCategoryDto>>.ReturnResultWith200(categoryDtos);
            }
            catch (Exception e)
            {
                // Step 11: Log any exceptions that occur during the operation.
                var errorMessage = $"Error retrieving customer age categories: {e.Message}";
                _logger.LogError(e, errorMessage);

                // Step 12: Log and audit the error for tracking purposes.
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    null,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.ErrorCustomerAgeCategory,
                    LogLevelInfo.Error
                );

                // Step 13: Return a 500 Internal Server Error response with the exception details.
                return ServiceResponse<List<CustomerAgeCategoryDto>>.Return500(e);
            }
        }
    }

}

