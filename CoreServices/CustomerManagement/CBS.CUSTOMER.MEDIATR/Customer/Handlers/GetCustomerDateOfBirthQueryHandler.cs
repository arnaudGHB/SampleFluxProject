using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.MEDIATR.Customer.Queries;
using CBS.CUSTOMER.DATA.Dto.Customers;

namespace CBS.CUSTOMER.MEDIATR
{
    /// <summary>
    /// Handles the request to retrieve a specific Customer based on its unique identifier.
    /// </summary>
    public class GetCustomerDateOfBirthQueryHandler : IRequestHandler<GetCustomerDateOfBirthQuery, ServiceResponse<GetCustomerDateOfBirthDto>>
    {
        private readonly ICustomerRepository _CustomerRepository; // Repository for accessing Customer data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IDocumentBaseUrlRepository _DocumentBaseUrlRepository;
        private readonly UserInfoToken _UserInfoToken;
        private readonly ILogger<GetCustomerDateOfBirthQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetCustomerDateOfBirthQueryHandler.
        /// </summary>
        /// <param name="CustomerRepository">Repository for Customer data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetCustomerDateOfBirthQueryHandler(
            ICustomerRepository CustomerRepository,
            IMapper mapper,
            ILogger<GetCustomerDateOfBirthQueryHandler> logger,
            UserInfoToken userInfoToken,
            IDocumentBaseUrlRepository DocumentBaseUrlRepository)
        {
            _CustomerRepository = CustomerRepository;
            _mapper = mapper;
            _logger = logger;
            _UserInfoToken = userInfoToken;
            _DocumentBaseUrlRepository = DocumentBaseUrlRepository;
        }

        /// <summary>
        /// Handles the GetCustomerQuery to retrieve a specific Customer.
        /// </summary>
        /// <param name="request">The GetCustomerQuery containing Customer ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<GetCustomerDateOfBirthDto>> Handle(GetCustomerDateOfBirthQuery request, CancellationToken cancellationToken)
        {
          
            string errorMessage = null;
            try
            {
                // Retrieve the Customer entity with the specified ID from the repository
                var entity =  _CustomerRepository.FindBy(  s => s.CustomerId == request.Id &&  s.IsDeleted == false);
                if (entity != null)
                {
                    

                    var Customer = _mapper.Map<GetCustomerDateOfBirthDto>(entity);

                    await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Read.ToString(), request, $"Get Customer with CustomerId : {request.Id}", LogLevelInfo.Information.ToString(), 200,_UserInfoToken.Token);
                    return ServiceResponse<GetCustomerDateOfBirthDto>.ReturnResultWith200(Customer);
                }
                else
                {
                    // If the Customer entity was not found, log the error and return a 404 Not Found response
                    errorMessage = $"Customer with CustomerId :  {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _UserInfoToken.Token);
                    return ServiceResponse<GetCustomerDateOfBirthDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Customer: {e.Message}";
                await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 500, _UserInfoToken.Token);

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<GetCustomerDateOfBirthDto>.Return500(e);
            }
        }
    }

}
