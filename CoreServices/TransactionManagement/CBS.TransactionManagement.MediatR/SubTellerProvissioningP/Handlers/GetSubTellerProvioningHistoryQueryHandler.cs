using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.MediatR.SubTellerProvissioningP.Queries;

namespace CBS.TransactionManagement.MediatR.SubTellerProvissioningP.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific SubTellerProvioningHistory based on its unique identifier.
    /// </summary>
    public class GetSubTellerProvioningHistoryQueryHandler : IRequestHandler<GetSubTellerProvioningHistoryQuery, ServiceResponse<SubTellerProvioningHistoryDto>>
    {
        private readonly ISubTellerProvisioningHistoryRepository _SubTellerProvioningHistoryRepository; // Repository for accessing SubTellerProvioningHistory data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetSubTellerProvioningHistoryQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetSubTellerProvioningHistoryQueryHandler.
        /// </summary>
        /// <param name="SubTellerProvioningHistoryRepository">Repository for SubTellerProvioningHistory data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetSubTellerProvioningHistoryQueryHandler(
            ISubTellerProvisioningHistoryRepository SubTellerProvioningHistoryRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetSubTellerProvioningHistoryQueryHandler> logger)
        {
            _SubTellerProvioningHistoryRepository = SubTellerProvioningHistoryRepository;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetSubTellerProvioningHistoryQuery to retrieve a specific SubTellerProvioningHistory.
        /// </summary>
        /// <param name="request">The GetSubTellerProvioningHistoryQuery containing SubTellerProvioningHistory ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<SubTellerProvioningHistoryDto>> Handle(GetSubTellerProvioningHistoryQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the SubTellerProvioningHistory entity with the specified ID from the repository
                var entity = await _SubTellerProvioningHistoryRepository.FindBy(a => a.Id == request.Id).Include(x => x.Teller).Include(x => x.Teller.Transactions).FirstOrDefaultAsync();
                if (entity != null)
                {
                    // Map the SubTellerProvioningHistory entity to SubTellerProvioningHistoryDto and return it with a success response
                    var SubTellerProvioningHistoryDto = _mapper.Map<SubTellerProvioningHistoryDto>(entity);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
                    return ServiceResponse<SubTellerProvioningHistoryDto>.ReturnResultWith200(SubTellerProvioningHistoryDto);
                }
                else
                {
                    // If the SubTellerProvioningHistory entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("SubTellerProvioningHistory not found.");
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "SubTellerProvioningHistory not found.", LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<SubTellerProvioningHistoryDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting SubTellerProvioningHistory: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<SubTellerProvioningHistoryDto>.Return500(e);
            }
        }

    }

}
