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
    /// Handles the retrieval of Expenditure based on the GetExpenditureQuery.
    /// </summary>
    public class GetExpenditureQueryHandler : IRequestHandler<GetExpenditureQuery, ServiceResponse<ExpenditureDto>>
    {
        private readonly IExpenditureRepository _ExpenditureRepository; // Repository for accessing Expenditure data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetExpenditureQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetExpenditureQueryHandler.
        /// </summary>
        /// <param name="ExpenditureRepository">Repository for Expenditure data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        public GetExpenditureQueryHandler(
            IExpenditureRepository ExpenditureRepository,
            UserInfoToken userInfoToken,
            IMapper mapper, ILogger<GetExpenditureQueryHandler> logger)
        {
            _ExpenditureRepository = ExpenditureRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetExpenditureQuery to retrieve an Expenditure.
        /// </summary>
        /// <param name="request">The GetExpenditureQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<ExpenditureDto>> Handle(GetExpenditureQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the Expenditure entity from the repository
                var entity = await _ExpenditureRepository.FindAsync(request.Id);

                // Step 2: Check if the entity exists
                if (entity == null)
                {
                    string notFoundMessage = $"Expenditure with ID '{request.Id}' not found.";
                    _logger.LogWarning(notFoundMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, notFoundMessage, LogLevelInfo.Error.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<ExpenditureDto>.Return404(notFoundMessage);
                }

                // Step 3: Log success and return the mapped entity as a DTO
                string successMessage = "Expenditure returned successfully.";
                _logger.LogInformation(successMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                var ExpenditureDto = _mapper.Map<ExpenditureDto>(entity);
                return ServiceResponse<ExpenditureDto>.ReturnResultWith200(ExpenditureDto);
            }
            catch (Exception e)
            {
                // Step 4: Log error and return a 500 Internal Server Error response
                string errorMessage = $"Failed to retrieve Expenditure: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<ExpenditureDto>.Return500(e, "Failed to retrieve Expenditure");
            }
        }
    }
}
