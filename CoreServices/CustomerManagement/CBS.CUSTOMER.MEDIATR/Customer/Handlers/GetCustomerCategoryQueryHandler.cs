using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.DATA;
using CBS.CUSTOMER.DATA.Dto;

namespace CBS.CUSTOMER.MEDIATR
{
    public class GetCustomerCategoryQueryHandler : IRequestHandler<GetCustomerCategoryQuery, ServiceResponse<CreateCustomerCategory>>
    {
        private readonly ICustomerCategoryRepository _CustomerCategoryRepository; // Repository for accessing CustomerCategoryQuery data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly UserInfoToken _UserInfoToken; // User information token for auditing purposes.
        private readonly ILogger<GetCustomerCategoryQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetCustomerCategoryQueryHandler.
        /// </summary>
        /// <param name="CustomerCategoryQueryRepository">Repository for CustomerCategoryQuery data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="userInfoToken">User information token for auditing purposes.</param>
        public GetCustomerCategoryQueryHandler(
            ICustomerCategoryRepository CustomerCategoryQueryRepository,
            IMapper mapper,
            ILogger<GetCustomerCategoryQueryHandler> logger,
            UserInfoToken userInfoToken)
        {
            _CustomerCategoryRepository = CustomerCategoryQueryRepository;
            _mapper = mapper;
            _logger = logger;
            _UserInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the GetCustomerCategoryQuery to retrieve a specific CustomerCategoryQuery.
        /// </summary>
        /// <param name="request">The GetCustomerCategoryQuery containing CustomerCategoryQuery ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CreateCustomerCategory>> Handle(GetCustomerCategoryQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null; // Variable to hold error message, initialized to null.
            try
            {
                // Retrieve the CustomerCategoryQuery entity with the specified ID from the repository
                var entity = await _CustomerCategoryRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    // Map the entity to a CreateCustomerCategory object using AutoMapper
                    var CustomerCategoryQuery = _mapper.Map<CreateCustomerCategory>(entity);

                    // Log the successful retrieval and audit the action
                    await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Read.ToString(), CustomerCategoryQuery, $"Get CustomerCategoryQuery with CustomerId : {request.Id}", LogLevelInfo.Information.ToString(), 200, _UserInfoToken.Token);

                    // Return a successful response with the mapped CreateCustomerCategory object
                    return ServiceResponse<CreateCustomerCategory>.ReturnResultWith200(CustomerCategoryQuery);
                }
                else
                {
                    // If the CustomerCategoryQuery entity was not found, log the error, return a 404 Not Found response, and audit the action
                    errorMessage = $"CustomerCategory with CustomerId :  {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _UserInfoToken.Token);
                    return ServiceResponse<CreateCustomerCategory>.Return404();
                }
            }
            catch (Exception e)
            {
                // If an exception occurs during the process, log the error, return a 500 Internal Server Error response with the error message, and audit the action
                errorMessage = $"Error occurred while getting CustomerCategoryQuery: {e.Message}";
                await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 500, _UserInfoToken.Token);
                _logger.LogError(errorMessage);
                return ServiceResponse<CreateCustomerCategory>.Return500(e);
            }
        }
    }

}
