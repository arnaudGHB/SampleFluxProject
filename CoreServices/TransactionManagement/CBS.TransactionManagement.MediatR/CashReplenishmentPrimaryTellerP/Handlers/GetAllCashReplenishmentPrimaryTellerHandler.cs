using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.MediatR.TellerP.Queries;
using CBS.TransactionManagement.MediatR.PrimaryTellerProvisioningP.Queries;
using CBS.TransactionManagement.MediatR.CashReplenishmentPrimaryTellerP.Queries;

namespace CBS.TransactionManagement.MediatR.PrimaryTellerProvisioningP.Handlers
{
    /// <summary>
    /// Handles the retrieval of all CashReplenishmentPrimaryTeller based on the GetAllRemittanceQuery.
    /// </summary>
    public class GetAllCashReplenishmentPrimaryTellerHandler : IRequestHandler<GetAllCashReplenishmentPrimaryTellerQuery, ServiceResponse<List<CashReplenishmentPrimaryTellerDto>>>
    {
        private readonly ICashReplenishmentPrimaryTellerRepository _CashReplenishmentPrimaryTellerRepository; // Repository for accessing CashReplenishmentPrimaryTeller data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllCashReplenishmentPrimaryTellerHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllCashReplenishmentPrimaryTellerQueryHandler.
        /// </summary>
        /// <param name="CashReplenishmentPrimaryTellerRepository">Repository for CashReplenishmentPrimaryTeller data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllCashReplenishmentPrimaryTellerHandler(
            ICashReplenishmentPrimaryTellerRepository CashReplenishmentPrimaryTellerRepository,
            UserInfoToken UserInfoToken,
            IMapper mapper, ILogger<GetAllCashReplenishmentPrimaryTellerHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _CashReplenishmentPrimaryTellerRepository = CashReplenishmentPrimaryTellerRepository;
            _userInfoToken = UserInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllRemittanceQuery to retrieve all CashReplenishmentPrimaryTeller.
        /// </summary>
        /// <param name="request">The GetAllRemittanceQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<CashReplenishmentPrimaryTellerDto>>> Handle(GetAllCashReplenishmentPrimaryTellerQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all CashReplenishmentPrimaryTeller entities from the repository
                var entities = await _CashReplenishmentPrimaryTellerRepository.FindBy(x=>x.IsDeleted==false && x.CreatedDate.Date >= request.DateFrom.Date && x.CreatedDate.Date <= request.DateTo.Date).ToListAsync();
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "System CashReplenishmentPrimaryTellers returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                return ServiceResponse<List<CashReplenishmentPrimaryTellerDto>>.ReturnResultWith200(_mapper.Map<List<CashReplenishmentPrimaryTellerDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all CashReplenishmentPrimaryTeller: {e.Message}");
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, $"Failed to get all CashReplenishmentPrimaryTeller: {e.Message}", LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<List<CashReplenishmentPrimaryTellerDto>>.Return500(e, "Failed to get all CashReplenishmentPrimaryTeller");
            }
        }

    }
}
