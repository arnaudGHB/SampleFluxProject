using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Entity;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to update a Account based on UpdateAccountCommand.
    /// </summary>
    public class UpdateAccountingEntryRulePositionCommandHandler : IRequestHandler<UpdateAccountingEntryRulePositionCommand ,ServiceResponse<AccountingEntryRulePositionDto>>
    {
        private readonly IAccountingRuleEntryRepository _AccountingRuleEntryRepository; // Repository for accessing Account data.
        private readonly ILogger<UpdateAccountingEntryRulePositionCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the UpdateAccountCommandHandler.
        /// </summary>
        /// <param name="AccountingRuleEntryRepository">Repository for Account data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateAccountingEntryRulePositionCommandHandler(
            IAccountingRuleEntryRepository AccountingRuleEntryRepository,
            ILogger<UpdateAccountingEntryRulePositionCommandHandler> logger,
            IMapper mapper,
            UserInfoToken userInfoToken, IUnitOfWork<POSContext> uow = null)
        {
            _AccountingRuleEntryRepository = AccountingRuleEntryRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

    /// <summary>
    /// Handles the UpdateAccountingEntryRulePositionCommand Command to update a AccountingRuleEntry.
    /// </summary>
    /// <param name="request">The UpdateAccountingEntryRulePositionCommand containing updated AccountingRuleEntry data.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public async Task<ServiceResponse<AccountingEntryRulePositionDto>> Handle(UpdateAccountingEntryRulePositionCommand request, CancellationToken cancellationToken)
    {
          string errorMessage = "";
            try
            {
                // Retrieve the Account entity to be updated from the repository
                var existing_AccountingRuleEntries =  _AccountingRuleEntryRepository.All.Where(x=>x.OperationEventAttributeId.Equals(request.AccountingRuleId)).ToList();

                // Check if the Account entity exists
                if (existing_AccountingRuleEntries != null)
                {
                    // Update Account entity properties with values from the request
                    var entries = UpdateAccountingEntries(existing_AccountingRuleEntries, request);
                    // Use the repository to update the existing Account entity
                   
                    if (await _uow.SaveAsync() <= 0)
                    {
                        errorMessage = $"An Exception occured when saving the accountingRuleEntries";
                        await APICallHelper.AuditLogger(_userInfoToken.Email, "UpdateAccountingEntryRulePositionCommand",
                            JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                        return ServiceResponse<AccountingEntryRulePositionDto>.Return500();
                    }
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<AccountingEntryRulePositionDto>.ReturnResultWith200(_mapper.Map<AccountingEntryRulePositionDto>(entries));
                    errorMessage=$"AccountingRuleId : {request.AccountingRuleId} was successfully updated.";
                    _logger.LogInformation(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "UpdateAccountingEntryRulePositionCommand",
                        JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                    return response;
                }
                else
                {
                    // If the Account entity was not found, return 404 Not Found response with an error message
                     errorMessage = $"AccountingRuleId:{request.AccountingRuleId} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "UpdateAccountingEntryRulePositionCommand",
                            JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Error.ToString(), 200, _userInfoToken.Token);

                    return ServiceResponse<AccountingEntryRulePositionDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                 errorMessage = $"Error occurred while updating Account: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "UpdateAccountingEntryRulePositionCommand",
                    JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Error.ToString(), 200, _userInfoToken.Token);

                return ServiceResponse<AccountingEntryRulePositionDto>.Return500(e);
            }
        }

        private List<AccountingRuleEntryDto> UpdateAccountingEntries(List<Data.AccountingRuleEntry> existing_AccountingRuleEntries, UpdateAccountingEntryRulePositionCommand request)
        {
            List<AccountingRuleEntryDto> existingEntries = new List<AccountingRuleEntryDto>();
            try
            {
               

                var ArrayOfExistingEntries = existing_AccountingRuleEntries.ToArray();
                int count = existing_AccountingRuleEntries.Count - 1;
                int i = 0;
                while (i<=count)
                {
                   Data. AccountingRuleEntry model = existing_AccountingRuleEntries[i];
                    var requestData = request.AccountingRuleEntries.Find(x => x.Id.Equals(model.Id));
                    model.ModifiedBy = _userInfoToken.Id;
                    model.ModifiedDate= DateTime.Now;
                    //model.PostingOrder = requestData.PostingOrder;
                    _AccountingRuleEntryRepository.Update(model);
                    var Dto = _mapper.Map<AccountingRuleEntryDto>(model);
                    existingEntries.Add(Dto);
                    i++;
                }
               
              
            }
            catch (Exception ex)
            {
                string errorMessage = $"Error occurred while updating AccountingEntryPosition: {ex.Message}";
                _logger.LogError(errorMessage);

            }
            return existingEntries;
        } 
    }
}