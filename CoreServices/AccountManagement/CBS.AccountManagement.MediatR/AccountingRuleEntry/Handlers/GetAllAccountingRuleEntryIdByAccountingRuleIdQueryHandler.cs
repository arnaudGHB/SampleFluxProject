using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Dto.AccountingRuleEntryDto;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
//using SixLabors.ImageSharp.Processing.Processors.Transforms;
using System.Collections.Generic;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Account based on its unique identifier.
    /// </summary>
    public class GetAllAccountingRuleEntryIdByAccountingRuleIdQueryHandler : IRequestHandler<GetAllAccountingRuleEntryIdByAccountingRuleIdQuery, ServiceResponse<List<AccountingEntryRuleIdsDto>>>
    {
        private readonly IAccountingRuleEntryRepository _AccountingRuleEntryRepository; // Repository for accessing Account data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllAccountingRuleEntryIdByAccountingRuleIdQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAccountingRuleEntryQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllAccountingRuleEntryIdByAccountingRuleIdQueryHandler(
            IAccountingRuleEntryRepository AccountingRuleEntryRepository,
            IMapper mapper,
            ILogger<GetAllAccountingRuleEntryIdByAccountingRuleIdQueryHandler> logger, UserInfoToken userInfoToken)
        {
            _AccountingRuleEntryRepository = AccountingRuleEntryRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the GetAllAccountingRuleEntryIdByAccountingRuleIdQuery to retrieve a specific Account.
        /// </summary>
        /// <param name="request">The GetAllAccountingRuleEntryIdByAccountingRuleIdQuery containing Account ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<AccountingEntryRuleIdsDto>>> Handle(GetAllAccountingRuleEntryIdByAccountingRuleIdQuery request, CancellationToken cancellationToken)
        {
            List < AccountingEntryRuleIdsDto > modelEntryRuleIdsDtos = new List < AccountingEntryRuleIdsDto >();
            string errorMessage = null;
            try
            {
                // Retrieve the AccountingRuleEntry entity with the specified ID from the repository
                var entities =  _AccountingRuleEntryRepository.All.ToList();
                if (entities != null)
                {
                    foreach (var item in entities.ToList())
                    {
                          var modelResult = new AccountingEntryRuleIdsDto { PostingOrder = 0, OperationEventAttributeId = item.OperationEventAttributeId};
                          modelEntryRuleIdsDtos.Add(modelResult);
                    }
                 
                    errorMessage = $"Getting all Ids of AccountingRuleEntries with ruleId {request.AccountingRuleId} has been retrieved.";
                    // Map the AccountingEntryRuleIdsDto entity to AccountingRuleEntryDto and return it with a success response
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAllAccountingRuleEntryIdByAccountingRuleIdQuery",
                        JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                    return ServiceResponse<List<AccountingEntryRuleIdsDto>>.ReturnResultWith200(modelEntryRuleIdsDtos);
                }
                else
                {
                    // If the AccountingRuleEntry entity was not found, log the error and return a 404 Not Found response

                    errorMessage = $"AccountingRuleId {request.AccountingRuleId} record not found.";
                    _logger.LogInformation(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAllAccountingRuleEntryIdByAccountingRuleIdQuery",
                        JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<List<AccountingEntryRuleIdsDto>>.ReturnResultWith200(modelEntryRuleIdsDtos);
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting AccountingRuleId: {e.Message}";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAllAccountingRuleEntryIdByAccountingRuleIdQuery",
                    JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Error.ToString(), 404, _userInfoToken.Token);

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<List<AccountingEntryRuleIdsDto>>.Return500(e);
            }
        }
    }
}