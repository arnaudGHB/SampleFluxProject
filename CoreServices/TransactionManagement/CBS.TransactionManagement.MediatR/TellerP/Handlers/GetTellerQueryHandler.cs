using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.MediatR.TellerP.Queries;

namespace CBS.TransactionManagement.MediatR.TellerP.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Teller based on its unique identifier.
    /// </summary>
    public class GetTellerQueryHandler : IRequestHandler<GetTellerQuery, ServiceResponse<TellerDto>>
    {
        private readonly ITellerRepository _TellerRepository; // Repository for accessing Teller data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetTellerQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetTellerQueryHandler.
        /// </summary>
        /// <param name="TellerRepository">Repository for Teller data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetTellerQueryHandler(
            ITellerRepository TellerRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetTellerQueryHandler> logger)
        {
            _TellerRepository = TellerRepository;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetTellerQuery to retrieve a specific Teller.
        /// </summary>
        /// <param name="request">The GetTellerQuery containing Teller ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<TellerDto>> Handle(GetTellerQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the Teller entity with the specified ID from the repository
                var entity = await _TellerRepository.FindBy(a => a.Id == request.Id).FirstAsync();
                if (entity != null)
                {
                    // Map the Teller entity to TellerDto and return it with a success response
                    var TellerDto = _mapper.Map<TellerDto>(entity);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
                    return ServiceResponse<TellerDto>.ReturnResultWith200(TellerDto);
                }
                else
                {
                    // If the Teller entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Teller not found.");
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "Teller not found.", LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<TellerDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Teller: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<TellerDto>.Return500(e);
            }
        }

    }

}
