using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.GAV.Queries;
using DocumentFormat.OpenXml.Spreadsheet;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Accounts based on the GetCustomerKYCByTelephoneQuery.
    /// </summary>
    public class GetCustomerKYCByTelephoneQueryHandler : IRequestHandler<GetCustomerKYCByTelephoneQuery, ServiceResponse<CustomerKYCDto>>
    {
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Accounts data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetCustomerKYCByTelephoneQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for initializing the GetCustomerKYCByTelephoneQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Accounts data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetCustomerKYCByTelephoneQueryHandler(
            IAccountRepository AccountRepository,
            UserInfoToken UserInfoToken,
            IMapper mapper, ILogger<GetCustomerKYCByTelephoneQueryHandler> logger, IMediator mediator = null)
        {
            // Assign provided dependencies to local variables.
            _AccountRepository = AccountRepository;
            _userInfoToken = UserInfoToken;
            _mapper = mapper;
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the GetCustomerKYCByTelephoneQuery to retrieve all Accounts.
        /// </summary>
        /// <param name="request">The GetCustomerKYCByTelephoneQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CustomerKYCDto>> Handle(GetCustomerKYCByTelephoneQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var customerRequest = await _mediator.Send(new GetCustomerByTelephoneQuery { TelephoneNumber = request.TelephoneNumber });
                if (customerRequest!=null)
                {
                    if (customerRequest.StatusCode == 200)
                    {
                        var customer = customerRequest.Data;
                        var customerDto = new CustomerKYCDto { CustomerId = customer.CustomerId, FirstName = customer.FirstName, LastName = customer.LastName, Phone = customer.Phone };
                        await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, $"Customer KYC gottened by using {request.TelephoneNumber}", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                        return ServiceResponse<CustomerKYCDto>.ReturnResultWith200(customerDto);

                    }
                }
          
                _logger.LogError($"Failed to getting customer KYC");
                return ServiceResponse<CustomerKYCDto>.Return403($"Failed to getting customer KYC.");

            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to getting customer KYC due to: {e.Message}");
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, $"Failed to getting customer KYC due to: {e.Message}", LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<CustomerKYCDto>.Return500($"Failed to getting customer KYC due to: {e.Message}");
            }
        }
    }
}
