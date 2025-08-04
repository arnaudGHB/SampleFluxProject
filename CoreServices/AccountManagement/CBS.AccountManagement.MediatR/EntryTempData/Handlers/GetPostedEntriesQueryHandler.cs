using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;

using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all AccountFeatures based on the GetAllAccountFeatureQuery.
    /// </summary>
    public class GetPostedEntriesQueryHandler : IRequestHandler<GetPostedEntriesQuery, ServiceResponse<PostedEntryDto>>
    {

        private readonly IPostedEntryRepository _postedEntryRepository; // Repository for accessing AccountFeature data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetPostedEntriesQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        /// <summary>
        /// Constructor for initializing the AddAccountFeatureCommandHandler.
        /// </summary>
        /// <param name="AccountFeatureRepository">Repository for AccountFeature data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetPostedEntriesQueryHandler(
            IPostedEntryRepository AccountFeatureRepository,
            IMapper mapper,
            ILogger<GetPostedEntriesQueryHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken)
        {
            _postedEntryRepository = AccountFeatureRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }
        /// <summary>
        /// Handles the GetAllPostedEntriesQuery to retrieve all PostedEntry.
        /// </summary>
        /// <param name="request">The GetAllPostedEntriesQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<PostedEntryDto>> Handle(GetPostedEntriesQuery request, CancellationToken cancellationToken)
        {
            try
            {
               
                var entities = await _postedEntryRepository.FindAsync(request.ReferenceId);
                string errorMessage = $"Retrieve PostedEntry entities from the repository successfully with refereceId:{request.ReferenceId}";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAllPostedEntriesQuery",
                 request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                return ServiceResponse<PostedEntryDto>.ReturnResultWith200(_mapper.Map<PostedEntryDto>(entities));
            }
            catch (Exception e)
            {
                var errorMessage = $"Failed to get all PostedEntries:{e.Message}";
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all PostedEntries: {e.Message}");
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAllPostedEntriesQuery",
                request, errorMessage, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<PostedEntryDto>.Return500(e, "Failed to get all PostedEntries");
            }
        }
    }
}