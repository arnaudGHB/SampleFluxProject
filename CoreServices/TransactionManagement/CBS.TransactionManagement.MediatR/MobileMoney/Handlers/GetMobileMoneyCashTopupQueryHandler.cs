using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.MediatR.MobileMoney.Queries;
using CBS.TransactionManagement.Data.Dto.MobileMoney;
using CBS.TransactionManagement.Repository.MobileMoney;

namespace CBS.TransactionManagement.MediatR.MobileMoney.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific MobileMoneyCashTopup based on its unique identifier.
    /// </summary>
    public class GetMobileMoneyCashTopupQueryHandler : IRequestHandler<GetMobileMoneyCashTopupQuery, ServiceResponse<MobileMoneyCashTopupDto>>
    {
        private readonly IMobileMoneyCashTopupRepository _MobileMoneyCashTopupRepository; // Repository for accessing MobileMoneyCashTopup data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetMobileMoneyCashTopupQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetMobileMoneyCashTopupQueryHandler.
        /// </summary>
        /// <param name="MobileMoneyCashTopupRepository">Repository for MobileMoneyCashTopup data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetMobileMoneyCashTopupQueryHandler(
            IMobileMoneyCashTopupRepository MobileMoneyCashTopupRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetMobileMoneyCashTopupQueryHandler> logger)
        {
            _MobileMoneyCashTopupRepository = MobileMoneyCashTopupRepository;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetMobileMoneyCashTopupQuery to retrieve a specific MobileMoneyCashTopup.
        /// </summary>
        /// <param name="request">The GetMobileMoneyCashTopupQuery containing MobileMoneyCashTopup ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<MobileMoneyCashTopupDto>> Handle(GetMobileMoneyCashTopupQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the MobileMoneyCashTopup entity with the specified ID from the repository
                var entity = await _MobileMoneyCashTopupRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    // Map the MobileMoneyCashTopup entity to MobileMoneyCashTopupDto and return it with a success response
                    var MobileMoneyCashTopupDto = _mapper.Map<MobileMoneyCashTopupDto>(entity);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                    return ServiceResponse<MobileMoneyCashTopupDto>.ReturnResultWith200(MobileMoneyCashTopupDto);
                }
                else
                {
                    errorMessage = $"Resource not found with request id: {request.Id}";
                    // If the MobileMoneyCashTopup entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<MobileMoneyCashTopupDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting MobileMoneyCashTopup: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<MobileMoneyCashTopupDto>.Return500(errorMessage);
            }
        }

    }

}
