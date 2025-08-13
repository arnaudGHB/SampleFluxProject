using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Queries.AccountingDayOpening;
using CBS.TransactionManagement.Repository.AccountingDayOpening;

namespace CBS.TransactionManagement.Handlers.AccountingDayOpening
{
    public class GetCurrentAccountingDayQueryHandler : IRequestHandler<GetCurrentAccountingDayQuery, ServiceResponse<DateTime>>
    {
        private readonly IAccountingDayRepository _accountingDayRepository; // Repository for accessing AccountingDay data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetCurrentAccountingDayQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken; // User information and token for audit logging.

        /// <summary>
        /// Constructor for initializing the GetCurrentAccountingDayQueryHandler.
        /// </summary>
        /// <param name="accountingDayRepository">Repository for AccountingDay data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="userInfoToken">User information and token for audit logging.</param>
        public GetCurrentAccountingDayQueryHandler(
            IAccountingDayRepository accountingDayRepository,
            IMapper mapper,
            ILogger<GetCurrentAccountingDayQueryHandler> logger,
            UserInfoToken userInfoToken)
        {
            _accountingDayRepository = accountingDayRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the query to retrieve the current accounting day for a given branch.
        /// </summary>
        /// <param name="request">The request containing the branch ID.</param>
        /// <param name="cancellationToken">Cancellation token for the async operation.</param>
        /// <returns>A service response containing the current accounting day or an error message.</returns>
        public async Task<ServiceResponse<DateTime>> Handle(GetCurrentAccountingDayQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the current accounting day for the specified branch.
                var currentAccountingDay = _accountingDayRepository.GetCurrentAccountingDay(request.BrnachId);

                if (currentAccountingDay != null)
                {
                    // Step 2: Log success and audit trail.
                    string successMessage = $"Successfully retrieved accounting day for branch {_userInfoToken.BranchCode} on {DateTime.UtcNow}.";
                    _logger.LogInformation(successMessage);
                    await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.AccountingDayRetrieved, LogLevelInfo.Information);

                    // Step 3: Return the current accounting day with a 200 status code.
                    return ServiceResponse<DateTime>.ReturnResultWith200(currentAccountingDay, "Accounting day is set.");
                }
                else
                {
                    // Step 4: Handle case where no accounting day is found.
                    string notFoundMessage = $"Accounting day not found for branch {_userInfoToken.BranchCode}.";
                    _logger.LogWarning(notFoundMessage);
                    await BaseUtilities.LogAndAuditAsync(notFoundMessage, request, HttpStatusCodeEnum.NotFound, LogAction.AccountingDayRetrieved, LogLevelInfo.Warning);

                    // Return a 404 Not Found response.
                    return ServiceResponse<DateTime>.Return404(notFoundMessage);
                }
            }
            catch (Exception e)
            {
                // Step 5: Handle any exceptions and log the error.
                string errorMessage = $"Error occurred while retrieving the accounting day for branch {_userInfoToken.BranchCode}: {e.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.AccountingDayRetrieved, LogLevelInfo.Error);

                // Return a 500 Internal Server Error response.
                return ServiceResponse<DateTime>.Return500(e);
            }
        }

    }

}
