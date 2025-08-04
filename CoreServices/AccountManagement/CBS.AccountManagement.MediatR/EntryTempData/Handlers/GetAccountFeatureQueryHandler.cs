using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific AccountFeature based on its unique identifier.
    /// </summary>
    public class GetEntryTempDataQueryHandler : IRequestHandler<GetEntryTempDataQuery, ServiceResponse<EntryTempDataDto>>
    {
        private readonly IEntryTempDataRepository _entryTempDataRepository; // Repository for accessing AccountFeature data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetEntryTempDataQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAccountFeatureQueryHandler.
        /// </summary>
        /// <param name="AccountFeatureRepository">Repository for AccountFeature data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetEntryTempDataQueryHandler(
            IEntryTempDataRepository entryTempDataRepository,
            IMapper mapper,
            ILogger<GetEntryTempDataQueryHandler> logger, UserInfoToken userInfoToken)
        {
            _entryTempDataRepository = entryTempDataRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
        }




        /// <summary>
        /// Handles the GetAccountFeatureQuery to retrieve a specific AccountFeature.
        /// </summary>
        /// <param name="request">The GetAccountFeatureQuery containing AccountFeature ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<EntryTempDataDto>> Handle(GetEntryTempDataQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the AccountFeature entity with the specified ID from the repository
                var entity = await _entryTempDataRepository.FindAsync(request.Id);
                if (entity != null)
                {

                    if (entity.IsDeleted)
                    {
                        string message = "EntryTempDataDto has been deleted.";
                        // If the AccountCategory entity was not found, log the error and return a 404 Not Found response
                        _logger.LogError(message);
                        await APICallHelper.AuditLogger(_userInfoToken.Email, "GetEntryTempDataQuery",
                request, message, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
                        return ServiceResponse<EntryTempDataDto>.Return404(message);
                    }
                    else
                    {
                        errorMessage = $"EntryTempDataDto successfully retrieved";
                        await APICallHelper.AuditLogger(_userInfoToken.Email, "GetEntryTempDataQuery",
         request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                        var AccountFeatureDto = _mapper.Map<EntryTempDataDto>(entity);
                        return ServiceResponse<EntryTempDataDto>.ReturnResultWith200(AccountFeatureDto);
                    }
                }
                else
                {
                    errorMessage = "EntryTempDataDto not found.";
                    // If the AccountFeature entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError(errorMessage);
                   
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "GetEntryTempDataQuery",
     request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<EntryTempDataDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting EntryTempDataDto: {e.Message}";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetEntryTempDataQuery",
request, errorMessage, LogLevelInfo.Error.ToString(), 404, _userInfoToken.Token);
                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<EntryTempDataDto>.Return500(e);
            }
        }
    }
}