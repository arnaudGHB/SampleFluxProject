using CBS.CUSTOMER.DATA.Entity.Customers;
using CBS.CUSTOMER.MEDIATR.Customer.Queries;
using CBS.CUSTOMER.REPOSITORY.CustomerRepo;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.DATA.Dto.Customers;
using CBS.CUSTOMER.DATA.Dto;
using AutoMapper;

namespace CBS.CUSTOMER.MEDIATR.Customer.Handlers
{
    /// <summary>
    /// Handles the query to retrieve a Customer Age Category by ID.
    /// </summary>
    public class GetCustomerAgeCategoryQueryHandler : IRequestHandler<GetCustomerAgeCategoryQuery, ServiceResponse<CustomerAgeCategoryDto>>
    {
        private readonly ICustomerAgeCategoryRepository _repository; // Repository for accessing CustomerAgeCategory data.
        private readonly ILogger<GetCustomerAgeCategoryQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly UserInfoToken _UserInfoToken; // User token information for logging.

        /// <summary>
        /// Constructor for initializing the GetCustomerAgeCategoryQueryHandler.
        /// </summary>
        public GetCustomerAgeCategoryQueryHandler(
            ICustomerAgeCategoryRepository repository,
            IMapper mapper,
            ILogger<GetCustomerAgeCategoryQueryHandler> logger,
            UserInfoToken userInfoToken)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _UserInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the GetCustomerAgeCategoryQuery to retrieve a Customer Age Category.
        /// </summary>
        /// <param name="request">The GetCustomerAgeCategoryQuery containing the ID.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CustomerAgeCategoryDto>> Handle(GetCustomerAgeCategoryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the Customer Age Category by ID
                var category = await _repository.FindAsync(request.Id);
                if (category == null)
                {
                    return ServiceResponse<CustomerAgeCategoryDto>.Return404("Category not found.");
                }

                var mappedCategory = _mapper.Map<CustomerAgeCategoryDto>(category);

                // Log the successful retrieval
                await APICallHelper.AuditLogger(
                    _UserInfoToken.Email,
                    LogAction.Read.ToString(),
                    mappedCategory,
                    $"Retrieving CustomerAgeCategory with ID '{request.Id}' Successful.",
                    LogLevelInfo.Information.ToString(),
                    200,
                    _UserInfoToken.Token
                );

                return ServiceResponse<CustomerAgeCategoryDto>.ReturnResultWith200(mappedCategory);
            }
            catch (Exception e)
            {
                // Log the error
                var errorMessage = $"Error occurred while retrieving CustomerAgeCategory: {e.Message}";
                _logger.LogError(errorMessage);

                await APICallHelper.AuditLogger(
                    _UserInfoToken.Email,
                    LogAction.Read.ToString(),
                    e.Message,
                    $"Internal Server Error occurred while retrieving CustomerAgeCategory with ID '{request.Id}'.",
                    LogLevelInfo.Error.ToString(),
                    500,
                    _UserInfoToken.Token
                );

                return ServiceResponse<CustomerAgeCategoryDto>.Return500(e);
            }
        }
    }

}
