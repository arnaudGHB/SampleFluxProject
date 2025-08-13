using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.MediatR.TellerP.Queries;
using CBS.TransactionManagement.MediatR.PrimaryTellerProvisioningP.Queries;
using CBS.TransactionManagement.Data;

namespace CBS.TransactionManagement.MediatR.PrimaryTellerProvisioningP.Handlers
{
    /// <summary>
    /// Handles the retrieval of all CashReplenishmentPrimaryTeller based on the GetAllCashReplenishmentPrimaryTellerWithoutDatesQuery.
    /// </summary>
    public class GetAllCashReplenishmentPrimaryTellerPendingQueryHandler : IRequestHandler<GetAllCashReplenishmentPrimaryTellerPendingQuery, ServiceResponse<List<CashReplenishmentPrimaryTellerDto>>>
    {
        private readonly ICashReplenishmentPrimaryTellerRepository _CashReplenishmentPrimaryTellerRepository; // Repository for accessing CashReplenishmentPrimaryTeller data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllCashReplenishmentPrimaryTellerPendingQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllCashReplenishmentPrimaryTellerWithoutDatesQueryHandler.
        /// </summary>
        /// <param name="CashReplenishmentPrimaryTellerRepository">Repository for CashReplenishmentPrimaryTeller data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllCashReplenishmentPrimaryTellerPendingQueryHandler(
            ICashReplenishmentPrimaryTellerRepository CashReplenishmentPrimaryTellerRepository,
            UserInfoToken UserInfoToken,
            IMapper mapper, ILogger<GetAllCashReplenishmentPrimaryTellerPendingQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _CashReplenishmentPrimaryTellerRepository = CashReplenishmentPrimaryTellerRepository;
            _userInfoToken = UserInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllCashReplenishmentPrimaryTellerWithoutDatesQuery to retrieve all CashReplenishmentPrimaryTeller.
        /// </summary>
        /// <param name="request">The GetAllCashReplenishmentPrimaryTellerWithoutDatesQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<CashReplenishmentPrimaryTellerDto>>> Handle(GetAllCashReplenishmentPrimaryTellerPendingQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Determine if the user is from the head office or a branch
                bool isHeadOffice = _userInfoToken.IsHeadOffice;

                // Retrieve CashReplenishmentPrimaryTeller entities based on user's role
                IQueryable<CashReplenishmentPrimaryTeller> query = _CashReplenishmentPrimaryTellerRepository
                    .FindBy(x => !x.IsDeleted && !x.Status); // Exclude deleted and processed records

                if (!isHeadOffice)
                {
                    // If user is not from head office, filter by branch ID
                    query = query.Where(x => x.BranchId == _userInfoToken.BranchID);
                }

                // Execute query and retrieve entities asynchronously
                List<CashReplenishmentPrimaryTeller> entities = await query.ToListAsync();

                // Log successful retrieval
                string successMessage = "System CashReplenishmentPrimaryTellers returned successfully";
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Map entities to DTOs and return a successful response
                List<CashReplenishmentPrimaryTellerDto> dtos = _mapper.Map<List<CashReplenishmentPrimaryTellerDto>>(entities);
                return ServiceResponse<List<CashReplenishmentPrimaryTellerDto>>.ReturnResultWith200(dtos);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                string errorMessage = $"Failed to get all CashReplenishmentPrimaryTeller: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<List<CashReplenishmentPrimaryTellerDto>>.Return500(e, "Failed to get all CashReplenishmentPrimaryTeller");
            }
        }

    }
}
