using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
//using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Account based on its unique identifier.
    /// </summary>
    public class GetAccountingRuleEntryQueryHandler : IRequestHandler<GetAccountingRuleEntryQuery, ServiceResponse<AccountingRuleEntryDto>>
    {
        private readonly IAccountingRuleEntryRepository _AccountingRuleEntryRepository; // Repository for accessing Account data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAccountingRuleEntryQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        private readonly IOperationEventAttributeRepository _operationEventAttributeRepository;

        /// <summary>
        /// Constructor for initializing the GetAccountingRuleEntryQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAccountingRuleEntryQueryHandler(
            IAccountingRuleEntryRepository AccountingRuleEntryRepository,
            IMapper mapper,
           IOperationEventAttributeRepository operationEventAttributeRepository,
            ILogger<GetAccountingRuleEntryQueryHandler> logger, UserInfoToken userInfoToken)
        {
            _AccountingRuleEntryRepository = AccountingRuleEntryRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _operationEventAttributeRepository = operationEventAttributeRepository;
        }

        /// <summary>
        /// Handles the GetAccountingRuleEntryQuery to retrieve a specific Account.
        /// </summary>
        /// <param name="request">The GetAccountingRuleEntryQuery containing Account ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AccountingRuleEntryDto>> Handle(GetAccountingRuleEntryQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the AccountingRuleEntry entity with the specified ID from the repository
                var entity = await _AccountingRuleEntryRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    if (entity.IsDeleted)
                    {
                        errorMessage = $"AccountingRule instance /**/ has been retrieved.";
                        // Map the AccountingRuleEntry entity to AccountingRuleEntryDto and return it with a success response
                        var AccountingRuleEntryDto = _mapper.Map<AccountingRuleEntryDto>(entity);
                        await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAccountingRuleEntryQuery",
             JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                        return ServiceResponse<AccountingRuleEntryDto>.ReturnResultWith200(AccountingRuleEntryDto);

                    }
                    else
                    {
                        AccountingRuleEntryDto mappedObject = _mapper.Map<AccountingRuleEntryDto>(entity);
                          mappedObject = _mapper.Map(entity, mappedObject);
                        var model = _operationEventAttributeRepository.Find(entity.OperationEventAttributeId);
                        mappedObject.OperationEventId = model.OperationEventId;
                        return ServiceResponse<AccountingRuleEntryDto>.ReturnResultWith200(mappedObject);
                    }
                }
                else
                {
                    // If the AccountingRuleEntry entity was not found, log the error and return a 404 Not Found response

                    errorMessage = $"AccountingRuleEntry {request.Id} record not found.";
                    _logger.LogInformation(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAccountingRuleEntryQuery",
                        JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);

                    return ServiceResponse<AccountingRuleEntryDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Account: {e.Message}";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAccountingRuleEntryQuery",
                    JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Error.ToString(), 404, _userInfoToken.Token);

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<AccountingRuleEntryDto>.Return500(e);
            }
        }
    }
}