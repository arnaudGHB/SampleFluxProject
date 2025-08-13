using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.MediatR.CashReplenishmentSubTellerP.Queries;

namespace CBS.TransactionManagement.MediatR.CashReplenishmentSubTellerP.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific CashReplenishment based on its unique identifier.
    /// </summary>
    public class GetCashReplenishmentQueryHandler : IRequestHandler<GetCashReplenishmentQuery, ServiceResponse<SubTellerCashReplenishmentDto>>
    {
        private readonly ICashReplenishmentRepository _CashReplenishmentRepository; // Repository for accessing CashReplenishment data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetCashReplenishmentQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetCashReplenishmentQueryHandler.
        /// </summary>
        /// <param name="CashReplenishmentRepository">Repository for CashReplenishment data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetCashReplenishmentQueryHandler(
            ICashReplenishmentRepository CashReplenishmentRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetCashReplenishmentQueryHandler> logger)
        {
            _CashReplenishmentRepository = CashReplenishmentRepository;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetMemberNoneCashOperationByIdQuery to retrieve a specific CashReplenishment.
        /// </summary>
        /// <param name="request">The GetMemberNoneCashOperationByIdQuery containing CashReplenishment ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<SubTellerCashReplenishmentDto>> Handle(GetCashReplenishmentQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the CashReplenishment entity with the specified ID from the repository
                var entity = await _CashReplenishmentRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    // Map the CashReplenishment entity to CashReplenishmentDto and return it with a success response
                    var CashReplenishmentDto = _mapper.Map<SubTellerCashReplenishmentDto>(entity);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
                    return ServiceResponse<SubTellerCashReplenishmentDto>.ReturnResultWith200(CashReplenishmentDto);
                }
                else
                {
                    // If the CashReplenishment entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("CashReplenishment not found.");
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "CashReplenishment not found.", LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<SubTellerCashReplenishmentDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting CashReplenishment: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<SubTellerCashReplenishmentDto>.Return500(e);
            }
        }

    }

}
