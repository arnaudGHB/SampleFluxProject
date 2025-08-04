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
    /// Handles the retrieval of all Expenditure based on the GetAllExpenditureQuery.
    /// </summary>
    public class GetAllExpenditureQueryHandler : IRequestHandler<GetAllExpenditureQuery, ServiceResponse<List<ExpenditureDto>>>
    {
        private readonly IExpenditureRepository _ExpenditureRepository; // Repository for accessing Expenditure data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllExpenditureQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllExpenditureQueryHandler.
        /// </summary>
        /// <param name="ExpenditureRepository">Repository for Expenditure data access.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllExpenditureQueryHandler(
            IExpenditureRepository ExpenditureRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetAllExpenditureQueryHandler> logger)
        {
            _ExpenditureRepository = ExpenditureRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllExpenditureQuery to retrieve all Expenditure.
        /// </summary>
        /// <param name="request">The GetAllExpenditureQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<ExpenditureDto>>> Handle(GetAllExpenditureQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve all Expenditure entities from the repository
                var entities = await _ExpenditureRepository.All.ToListAsync(cancellationToken);

                // Step 2: Log the successful retrieval of Expenditure
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "Expenditure returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Step 3: Return the result with a successful response
                return ServiceResponse<List<ExpenditureDto>>.ReturnResultWith200(_mapper.Map<List<ExpenditureDto>>(entities));
            }
            catch (Exception e)
            {
                // Step 4: Log error and return a 500 Internal Server Error response
                string errorMessage = $"Failed to retrieve Expenditure: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<List<ExpenditureDto>>.Return500(e, "Failed to retrieve Expenditure");
            }
        }
    }
}
