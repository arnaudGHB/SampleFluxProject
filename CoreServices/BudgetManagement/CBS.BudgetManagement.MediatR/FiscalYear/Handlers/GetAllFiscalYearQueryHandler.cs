using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BudgetManagement.MediatR.Queries;
using CBS.BudgetManagement.Repository;
using CBS.BudgetManagement.Helper;
using CBS.BudgetManagement.Data;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CBS.BudgetManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all FiscalYear based on the GetAllFiscalYearQuery.
    /// </summary>
    public class GetAllFiscalYearQueryHandler : IRequestHandler<GetAllFiscalYearQuery, ServiceResponse<List<FiscalYearDto>>>
    {
        private readonly IFiscalYearRepository _FiscalYearRepository; // Repository for accessing FiscalYear data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllFiscalYearQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllFiscalYearQueryHandler.
        /// </summary>
        /// <param name="FiscalYearRepository">Repository for FiscalYear data access.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllFiscalYearQueryHandler(
            IFiscalYearRepository FiscalYearRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetAllFiscalYearQueryHandler> logger)
        {
            _FiscalYearRepository = FiscalYearRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllFiscalYearQuery to retrieve all FiscalYear.
        /// </summary>
        /// <param name="request">The GetAllFiscalYearQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<FiscalYearDto>>> Handle(GetAllFiscalYearQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve all FiscalYear entities from the repository
                var entities = await _FiscalYearRepository.All.ToListAsync(cancellationToken);

                // Step 2: Log the successful retrieval of FiscalYear
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "FiscalYear returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Step 3: Return the result with a successful response
                return ServiceResponse<List<FiscalYearDto>>.ReturnResultWith200(_mapper.Map<List<FiscalYearDto>>(entities));
            }
            catch (Exception e)
            {
                // Step 4: Log error and return a 500 Internal Server Error response
                string errorMessage = $"Failed to retrieve FiscalYear: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<List<FiscalYearDto>>.Return500(e, "Failed to retrieve FiscalYear");
            }
        }
    }
}
