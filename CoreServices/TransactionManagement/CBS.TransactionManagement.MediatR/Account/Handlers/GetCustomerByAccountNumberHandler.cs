using AutoMapper;
using MediatR;

using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Account based on its unique identifier.
    /// </summary>
    public class GetCustomerByAccountNumberHandler : IRequestHandler<GetCustomerByAccountNumber, ServiceResponse<CustomerKYCDto>>
    {
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Account data.
        private readonly ILogger<GetCustomerByAccountNumberHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        private readonly IMediator _mediator;
        /// <summary>
        /// Constructor for initializing the GetTransactionQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetCustomerByAccountNumberHandler(
            IAccountRepository AccountRepository,
            UserInfoToken userInfoToken,
            ILogger<GetCustomerByAccountNumberHandler> logger,
            IMediator mediator = null)
        {
            _AccountRepository = AccountRepository;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the GetCustomerByAccountNumber to retrieve a specific Account.
        /// </summary>
        /// <param name="request">The GetCustomerByAccountNumber containing Account ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CustomerKYCDto>> Handle(GetCustomerByAccountNumber request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the Account entity with the specified ID from the repository
                var account = await _AccountRepository.FindBy(x=>x.AccountNumber==request.AccountNumber).FirstOrDefaultAsync();
                //var entity = await _AccountRepository.FindAsync(request.Id);
                if (account != null)
                {
                    var customerCommandQuery = new GetCustomerCommandQuery { CustomerID = account.CustomerId };
                    var customerResponse = await _mediator.Send(customerCommandQuery);
                    if (customerResponse.StatusCode==200)
                    {
                        var customer = customerResponse.Data;
                        var andriodCustomer = new CustomerKYCDto { CustomerId = customer.CustomerId,
                         FirstName=customer.FirstName, LastName=customer.LastName, Phone=customer.Phone};
                        return ServiceResponse<CustomerKYCDto>.ReturnResultWith200(andriodCustomer);

                    }
                }
                // If the Account entity was not found, log the error and return a 404 Not Found response
                _logger.LogError("Account not found.");
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "Account not found.", LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                return ServiceResponse<CustomerKYCDto>.Return404("Invalida account number");

            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Account: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.Log(LogLevel.Error, e, errorMessage, null);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<CustomerKYCDto>.Return500(e, errorMessage);
            }
        }
    }

}
