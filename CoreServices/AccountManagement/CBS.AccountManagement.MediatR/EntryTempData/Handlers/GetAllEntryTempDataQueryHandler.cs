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
    public class GetAllEntryTempDataQueryHandler : IRequestHandler<GetAllEntryTempDataQuery, ServiceResponse<List<EntryTempDataDto>>>
    {

        private readonly IEntryTempDataRepository _entryTempDataRepository; // Repository for accessing AccountFeature data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllEntryTempDataQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        /// <summary>
        /// Constructor for initializing the AddAccountFeatureCommandHandler.
        /// </summary>
        /// <param name="AccountFeatureRepository">Repository for AccountFeature data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllEntryTempDataQueryHandler(
            IEntryTempDataRepository AccountFeatureRepository,
            IMapper mapper,
            ILogger<GetAllEntryTempDataQueryHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken)
        {
            _entryTempDataRepository = AccountFeatureRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }
        /// <summary>
        /// Handles the GetAllAccountFeatureQuery to retrieve all AccountFeatures.
        /// </summary>
        /// <param name="request">The GetAllAccountFeatureQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<EntryTempDataDto>>> Handle(GetAllEntryTempDataQuery request, CancellationToken cancellationToken)
        {
            try
            {
               
                var entities = await _entryTempDataRepository.All.Where(c =>   c.CreatedBy == _userInfoToken.Id && c.BranchId == _userInfoToken.BranchId && c.Reference == request.ReferenceId&&c.Status!= "Review").ToListAsync();
                string errorMessage = $"Rerieve all AccountFeatures entities from the repository successfully.Count:{entities.Count()}";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAllEntryTempDataQuery",
                 request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                return ServiceResponse<List<EntryTempDataDto>>.ReturnResultWith200(_mapper.Map<List<EntryTempDataDto>>(entities));
            }
            catch (Exception e)
            {
                var errorMessage = $"Failed to get all EntryTempData:{e.Message}";
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all EntryTempData: {e.Message}");
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAllEntryTempDataQuery",
                request, errorMessage, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<List<EntryTempDataDto>>.Return500(e, "Failed to get all AccountFeatures");
            }
        }
    }
}