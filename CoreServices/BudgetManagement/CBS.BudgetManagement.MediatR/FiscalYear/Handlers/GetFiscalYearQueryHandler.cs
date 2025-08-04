using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BudgetManagement.MediatR.Queries;
using CBS.BudgetManagement.Repository;
using CBS.BudgetManagement.Helper;
using CBS.BudgetManagement.Data;
 
using System.Threading;
using System.Threading.Tasks;

namespace CBS.BudgetManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of FiscalYear based on the GetFiscalYearQuery.
    /// </summary>
    public class GetFiscalYearQueryHandler : IRequestHandler<GetFiscalYearQuery, ServiceResponse<FiscalYearDto>>
    {
        private readonly IFiscalYearRepository _FiscalYearRepository; // Repository for accessing FiscalYear data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetFiscalYearQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetFiscalYearQueryHandler.
        /// </summary>
        /// <param name="FiscalYearRepository">Repository for FiscalYear data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        public GetFiscalYearQueryHandler(
            IFiscalYearRepository FiscalYearRepository,
            UserInfoToken userInfoToken,
            IMapper mapper, ILogger<GetFiscalYearQueryHandler> logger)
        {
            _FiscalYearRepository = FiscalYearRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetFiscalYearQuery to retrieve an FiscalYear.
        /// </summary>
        /// <param name="request">The GetFiscalYearQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<FiscalYearDto>> Handle(GetFiscalYearQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the FiscalYear entity from the repository
                var entity = await _FiscalYearRepository.FindAsync(request.Id);

                // Step 2: Check if the entity exists
                if (entity == null)
                {
                    string notFoundMessage = $"FiscalYear with ID '{request.Id}' not found.";
                    _logger.LogWarning(notFoundMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, notFoundMessage, LogLevelInfo.Error.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<FiscalYearDto>.Return404(notFoundMessage);
                }

                // Step 3: Log success and return the mapped entity as a DTO
                string successMessage = "FiscalYear returned successfully.";
                _logger.LogInformation(successMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                var FiscalYearDto = _mapper.Map<FiscalYearDto>(entity);
                return ServiceResponse<FiscalYearDto>.ReturnResultWith200(FiscalYearDto);
            }
            catch (Exception e)
            {
                // Step 4: Log error and return a 500 Internal Server Error response
                string errorMessage = $"Failed to retrieve FiscalYear: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<FiscalYearDto>.Return500(e, "Failed to retrieve FiscalYear");
            }
        }
    }
}
