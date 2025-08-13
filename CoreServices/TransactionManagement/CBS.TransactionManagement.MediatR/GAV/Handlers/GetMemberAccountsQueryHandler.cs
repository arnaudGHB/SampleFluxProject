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
    /// Handles the retrieval of all Accounts based on the GetMemberAccountsQuery.
    /// </summary>
    public class GetMemberAccountsQueryHandler : IRequestHandler<GetMemberAccountsQuery, ServiceResponse<List<MemberAccountsThirdPartyDto>>>
    {
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Accounts data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetMemberAccountsQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for initializing the GetMemberAccountsQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Accounts data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetMemberAccountsQueryHandler(
            IAccountRepository AccountRepository,
            UserInfoToken UserInfoToken,
            IMapper mapper, ILogger<GetMemberAccountsQueryHandler> logger, IMediator mediator = null)
        {
            // Assign provided dependencies to local variables.
            _AccountRepository = AccountRepository;
            _userInfoToken = UserInfoToken;
            _mapper = mapper;
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the GetMemberAccountsQuery to retrieve all Accounts.
        /// </summary>
        /// <param name="request">The GetMemberAccountsQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<MemberAccountsThirdPartyDto>>> Handle(GetMemberAccountsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var customerRequest = await _mediator.Send(new GetCustomerByTelephoneQuery { TelephoneNumber = request.TelephoneNumber });
                if (customerRequest.StatusCode == 200)
                {
                    var customer = customerRequest.Data;
                    // Retrieve all Accounts entities from the repository
                    var entities = _AccountRepository.FindBy(a => a.CustomerId == customer.CustomerId && a.IsDeleted == false).Include(a => a.Product).ThenInclude(a => a.WithdrawalParameters)
                        .Select(a => new
                        {
                            Account = a,
                            FilteredWithdrawalNotifications = a.WithdrawalNotifications.Where(wn => !wn.IsDeleted).ToList()
                        })
                        .AsEnumerable() // Execute the query and bring the data to the client side
                        .Select(a =>
                        {
                            a.Account.WithdrawalNotifications = a.FilteredWithdrawalNotifications;
                            return a.Account;
                        })
                        .ToList();
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "Customer accounts returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                    return ServiceResponse<List<MemberAccountsThirdPartyDto>>.ReturnResultWith200(_mapper.Map<List<MemberAccountsThirdPartyDto>>(entities));

                }
                _logger.LogError($"Failed to getting Accounts due: {customerRequest.Message}");
                return ServiceResponse<List<MemberAccountsThirdPartyDto>>.Return403($"Failed to getting Accounts due: {customerRequest.Message}");

            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Accounts: {e.Message}");
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, $"Failed to get all Accounts: {e.Message}", LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<List<MemberAccountsThirdPartyDto>>.Return500(e, "Failed to get all Accounts");
            }
        }
    }
}
