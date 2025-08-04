using MediatR;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.MediatR.Queries;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a paginated list of member accounts based on specific criteria.
    /// </summary>
    public class GetAllMembersAccountPegginationQueryHandler : IRequestHandler<GetAllMembersAccountPegginationQuery, ServiceResponse<MemberAccountSituationListing>>
    {
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Account data.
        private readonly ILogger<GetAllMembersAccountPegginationQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken; // Contains user information and authentication token.

        /// <summary>
        /// Constructor for initializing the GetAllMembersAccountPegginationQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="userInfoToken">User information and authentication token.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllMembersAccountPegginationQueryHandler(
            IAccountRepository AccountRepository,
            UserInfoToken userInfoToken,
            ILogger<GetAllMembersAccountPegginationQueryHandler> logger)
        {
            _AccountRepository = AccountRepository; // Assign the repository instance.
            _userInfoToken = userInfoToken; // Assign the user info token.
            _logger = logger; // Assign the logger instance.
        }

        /// <summary>
        /// Handles the GetAllMembersAccountPegginationQuery to retrieve a paginated list of member accounts.
        /// </summary>
        /// <param name="request">The GetAllMembersAccountPegginationQuery containing pagination and filter criteria.</param>
        /// <param name="cancellationToken">A cancellation token to monitor for cancellation requests.</param>
        /// <returns>A ServiceResponse containing the paginated member account summaries.</returns>
        public async Task<ServiceResponse<MemberAccountSituationListing>> Handle(GetAllMembersAccountPegginationQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null; // Variable to store error messages.

            try
            {
                // Fetch paginated customer account summaries from the repository based on the query parameters.
                MemberAccountSituationListing membersAccountSummaries = await _AccountRepository.GetMembersAccountSummaries(request.AccountResource);

                // Return a successful response with the paginated customer account summaries.
                return ServiceResponse<MemberAccountSituationListing>.ReturnResultWith200(membersAccountSummaries);
            }
            catch (Exception e)
            {
                // Capture the error message if an exception occurs.
                errorMessage = $"Failed to get paginated members account: {e.Message}";

                // Log the error using the logger.
                _logger.LogError(errorMessage);

                // Perform audit logging for the error.
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                // Return a 500 Internal Server Error response with the error message.
                return ServiceResponse<MemberAccountSituationListing>.Return500(errorMessage);
            }
        }
    }

}
