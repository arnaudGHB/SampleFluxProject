using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.MediatR.TellerP.Queries;
using CBS.TransactionManagement.MediatR.CashReplenishmentSubTellerP.Queries;
using CBS.TransactionManagement.MediatR.PrimaryTellerProvisioningP.Queries;

namespace CBS.TransactionManagement.MediatR.PrimaryTellerProvisioningP.Handlers
{
    /// <summary>
    /// Handles the retrieval of all CashReplenishment based on the GetAllCashReplenishmentPrimaryTellerByBranchQuery.
    /// </summary>
    public class GetAllCashReplenishmentPrimaryTellerByBranchQueryHandler : IRequestHandler<GetAllCashReplenishmentPrimaryTellerByBranchQuery, ServiceResponse<List<CashReplenishmentPrimaryTellerDto>>>
    {
        private readonly ICashReplenishmentPrimaryTellerRepository _CashReplenishmentRepository; // Repository for accessing CashReplenishment data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllCashReplenishmentPrimaryTellerByBranchQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllRemittanceByBranchQueryHandler.
        /// </summary>
        /// <param name="CashReplenishmentRepository">Repository for CashReplenishment data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllCashReplenishmentPrimaryTellerByBranchQueryHandler(
            ICashReplenishmentPrimaryTellerRepository CashReplenishmentRepository,
            UserInfoToken UserInfoToken,
            IMapper mapper, ILogger<GetAllCashReplenishmentPrimaryTellerByBranchQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _CashReplenishmentRepository = CashReplenishmentRepository;
            _userInfoToken = UserInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllCashReplenishmentPrimaryTellerByBranchQuery to retrieve all CashReplenishment.
        /// </summary>
        /// <param name="request">The GetAllCashReplenishmentPrimaryTellerByBranchQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<CashReplenishmentPrimaryTellerDto>>> Handle(GetAllCashReplenishmentPrimaryTellerByBranchQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all CashReplenishment entities from the repository by Branch ID
                var entities = await _CashReplenishmentRepository.FindBy(x=>x.IsDeleted==false && x.BranchId==request.BranchId && x.CreatedDate.Date>=request.DateFrom.Date &&x.CreatedDate.Date<=request.DateTo.Date).ToListAsync();
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "System CashReplenishments returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                return ServiceResponse<List<CashReplenishmentPrimaryTellerDto>>.ReturnResultWith200(_mapper.Map<List<CashReplenishmentPrimaryTellerDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all CashReplenishment: {e.Message}");
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, $"Failed to get all CashReplenishment: {e.Message}", LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<List<CashReplenishmentPrimaryTellerDto>>.Return500(e, "Failed to get all CashReplenishment");
            }
        }

    }
}
