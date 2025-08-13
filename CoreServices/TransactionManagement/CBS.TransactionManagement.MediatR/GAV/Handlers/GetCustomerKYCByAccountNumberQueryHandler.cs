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
    /// Handles the retrieval of all Accounts based on the GetCustomerKYCByAccountNumberQuery.
    /// </summary>
    public class GetCustomerKYCByAccountNumberQueryHandler : IRequestHandler<GetCustomerKYCByAccountNumberQuery, ServiceResponse<CustomerKYCDto>>
    {
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Accounts data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetCustomerKYCByAccountNumberQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for initializing the GetCustomerKYCByAccountNumberQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Accounts data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetCustomerKYCByAccountNumberQueryHandler(
            IAccountRepository AccountRepository,
            UserInfoToken UserInfoToken,
            IMapper mapper, ILogger<GetCustomerKYCByAccountNumberQueryHandler> logger, IMediator mediator = null)
        {
            // Assign provided dependencies to local variables.
            _AccountRepository = AccountRepository;
            _userInfoToken = UserInfoToken;
            _mapper = mapper;
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the GetCustomerKYCByAccountNumberQuery to retrieve all Accounts.
        /// </summary>
        /// <param name="request">The GetCustomerKYCByAccountNumberQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CustomerKYCDto>> Handle(GetCustomerKYCByAccountNumberQuery request, CancellationToken cancellationToken)
        {
            try
            {
                string message = string.Empty;
                var account = await _AccountRepository.FindBy(x => x.AccountNumber == request.AccountNumber).FirstOrDefaultAsync();
                if (account==null)
                {
                    message = $"Account with number {request.AccountNumber} does not exist.";
                    _logger.LogError(message);
                    await APICallHelper.AuditLogger(_userInfoToken.FullName, LogAction.Read.ToString(), request, message, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<CustomerKYCDto>.Return404(message);

                }
                var customerRequest = new GetCustomerCommandQuery { CustomerID = account.CustomerId };
                var customerResponse = await _mediator.Send(customerRequest);
                if (customerResponse != null)
                {
                    if (customerResponse.StatusCode == 200)
                    {
                        var customer = customerResponse.Data;
                        var customerDto = new CustomerKYCDto { CustomerId = customer.CustomerId, FirstName = customer.FirstName, LastName = customer.LastName, Phone = customer.Phone };
                        await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, $"Customer KYC gottened by using {request.AccountNumber}", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
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
