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
using CBS.TransactionManagement.MediatR.CashReplenishmentSubTellerP.Queries;

namespace CBS.TransactionManagement.MediatR.CashReplenishmentSubTellerP.Handlers
{
    /// <summary>
    /// Handles the retrieval of all CashReplenishmentSubTeller based on the GetAllCashReplenishmentSubTellerWithoutDatesQuery.
    /// </summary>
    public class GetAllCashReplenishmentSubTellerPendingQueryHandler : IRequestHandler<GetAllCashReplenishmentSubTellerPendingQuery, ServiceResponse<List<SubTellerCashReplenishmentDto>>>
    {
        private readonly ICashReplenishmentRepository _CashReplenishmentSubTellerRepository; // Repository for accessing CashReplenishmentSubTeller data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllCashReplenishmentSubTellerPendingQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllCashReplenishmentSubTellerWithoutDatesQueryHandler.
        /// </summary>
        /// <param name="CashReplenishmentSubTellerRepository">Repository for CashReplenishmentSubTeller data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllCashReplenishmentSubTellerPendingQueryHandler(
            ICashReplenishmentRepository CashReplenishmentSubTellerRepository,
            UserInfoToken UserInfoToken,
            IMapper mapper, ILogger<GetAllCashReplenishmentSubTellerPendingQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _CashReplenishmentSubTellerRepository = CashReplenishmentSubTellerRepository;
            _userInfoToken = UserInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllCashReplenishmentSubTellerWithoutDatesQuery to retrieve all CashReplenishmentSubTeller.
        /// </summary>
        /// <param name="request">The GetAllCashReplenishmentSubTellerWithoutDatesQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<SubTellerCashReplenishmentDto>>> Handle(GetAllCashReplenishmentSubTellerPendingQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Determine if the user is from the head office or a branch
                bool isHeadOffice = _userInfoToken.IsHeadOffice;

                // Retrieve CashReplenishmentSubTeller entities based on user's role
                IQueryable<CashReplenishmentSubTeller> query = _CashReplenishmentSubTellerRepository
                    .FindBy(x => !x.IsDeleted && x.ApprovedStatus!=Status.Approved.ToString()); // Exclude deleted and processed records

                if (!isHeadOffice)
                {
                    // If user is not from head office, filter by branch ID
                    query = query.Where(x => x.BranchId == _userInfoToken.BranchID);
                }

                // Execute query and retrieve entities asynchronously
                List<CashReplenishmentSubTeller> entities = await query.ToListAsync();

                // Log successful retrieval
                string successMessage = "System CashReplenishmentSubTellers returned successfully";
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Map entities to DTOs and return a successful response
                List<SubTellerCashReplenishmentDto> dtos = _mapper.Map<List<SubTellerCashReplenishmentDto>>(entities);
                return ServiceResponse<List<SubTellerCashReplenishmentDto>>.ReturnResultWith200(dtos);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                string errorMessage = $"Failed to get all CashReplenishmentSubTeller: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<List<SubTellerCashReplenishmentDto>>.Return500(e, "Failed to get all CashReplenishmentSubTeller");
            }
        }

    }
}
