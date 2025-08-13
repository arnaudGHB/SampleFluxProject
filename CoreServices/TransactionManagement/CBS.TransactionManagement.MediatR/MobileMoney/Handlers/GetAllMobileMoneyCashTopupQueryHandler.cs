using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.MediatR.MobileMoney.Queries;
using CBS.TransactionManagement.Data.Dto.MobileMoney;
using CBS.TransactionManagement.Repository.MobileMoney;

namespace CBS.TransactionManagement.MediatR.MobileMoney.Handlers
{
    /// <summary>
    /// Handles the retrieval of all MobileMoneyCashTopup based on the GetAllMobileMoneyCashTopupQuery.
    /// </summary>
    public class GetAllMobileMoneyCashTopupQueryHandler : IRequestHandler<GetAllMobileMoneyCashTopupQuery, ServiceResponse<List<MobileMoneyCashTopupDto>>>
    {
        private readonly IMobileMoneyCashTopupRepository _MobileMoneyCashTopupRepository; // Repository for accessing MobileMoneyCashTopup data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllMobileMoneyCashTopupQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllMobileMoneyCashTopupQueryHandler.
        /// </summary>
        /// <param name="MobileMoneyCashTopupRepository">Repository for MobileMoneyCashTopup data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllMobileMoneyCashTopupQueryHandler(
            IMobileMoneyCashTopupRepository MobileMoneyCashTopupRepository,
            UserInfoToken UserInfoToken,
            IMapper mapper, ILogger<GetAllMobileMoneyCashTopupQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _MobileMoneyCashTopupRepository = MobileMoneyCashTopupRepository;
            _userInfoToken = UserInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllMobileMoneyCashTopupQuery to retrieve all MobileMoneyCashTopup.
        /// </summary>
        /// <param name="request">The GetAllMobileMoneyCashTopupQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<MobileMoneyCashTopupDto>>> Handle(GetAllMobileMoneyCashTopupQuery request, CancellationToken cancellationToken)
        {
            try
            {

                // Retrieve all MobileMoneyCashTopup entities from the repository
                var entities = await _MobileMoneyCashTopupRepository.GetMobileMoneyCash(request.QueryParameter, request.ByBranch, request.BranchId);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "System MobileMoneyCashTopups returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                return ServiceResponse<List<MobileMoneyCashTopupDto>>.ReturnResultWith200(_mapper.Map<List<MobileMoneyCashTopupDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all MobileMoneyCashTopup: {e.Message}");
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, $"Failed to get all MobileMoneyCashTopup: {e.Message}", LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<List<MobileMoneyCashTopupDto>>.Return500(e, "Failed to get all MobileMoneyCashTopup");
            }
        }

    }
}
