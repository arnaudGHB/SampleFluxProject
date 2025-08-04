using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.MediatR.TellerP.Queries;

namespace CBS.TransactionManagement.MediatR.TellerP.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Teller based on the GetAllTellerQuery.
    /// </summary>
    public class GetAllTellerQueryHandler : IRequestHandler<GetAllTellerQuery, ServiceResponse<List<TellerDto>>>
    {
        private readonly ITellerRepository _TellerRepository; // Repository for accessing Teller data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllTellerQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllTellerQueryHandler.
        /// </summary>
        /// <param name="TellerRepository">Repository for Teller data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllTellerQueryHandler(
            ITellerRepository TellerRepository,
            UserInfoToken UserInfoToken,
            IMapper mapper, ILogger<GetAllTellerQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _TellerRepository = TellerRepository;
            _userInfoToken = UserInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllTellerQuery to retrieve all Teller.
        /// </summary>
        /// <param name="request">The GetAllTellerQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<TellerDto>>> Handle(GetAllTellerQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all Teller entities from the repository
                var entities = await _TellerRepository.All.Where(x => !x.IsDeleted).ToListAsync();
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "System Tellers returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                return ServiceResponse<List<TellerDto>>.ReturnResultWith200(_mapper.Map<List<TellerDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Teller: {e.Message}");
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, $"Failed to get all Teller: {e.Message}", LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<List<TellerDto>>.Return500(e, "Failed to get all Teller");
            }
        }

    }
}
