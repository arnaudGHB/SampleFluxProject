
using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command for adding a new Account, including data validation, repository operations,
    /// logging, and response management.
    /// </summary>
    public class AddCommandTrialBalanceFileHandler : IRequestHandler<AddCommandTrialBalanceFile, ServiceResponse<bool>>
    {
        // Declare dependencies for handling command operations
        private readonly ITrialBalanceFileRepository _trialBalanceFileRepository;
 
        private readonly IMapper _mapper;
        private readonly ILogger<AddCommandTrialBalanceFileHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly PathHelper _pathHelper;

        /// <summary>
        /// Initializes the handler with required dependencies for adding Accounts.
        /// </summary>
        /// <param name="TrialBalanceFileRepository">Repository for managing Account data.</param>
        /// <param name="mapper">Mapper for transforming objects.</param>
        /// <param name="logger">Logger for recording actions and errors.</param>
        /// <param name="pathHelper">Helper for path-related information.</param>
        /// <param name="uow">Unit of work for managing database transactions.</param>
        /// <param name="userInfoToken">User token for auditing purposes.</param>

        public AddCommandTrialBalanceFileHandler(
            ITrialBalanceFileRepository AccountRepository,
 
            IMapper mapper,
            ILogger<AddCommandTrialBalanceFileHandler> logger,
            PathHelper pathHelper,
            IUnitOfWork<POSContext> uow,
            UserInfoToken userInfoToken )
        {
            // Assign constructor parameters to local variables
            _trialBalanceFileRepository = AccountRepository;
            _mapper = mapper;
 
            _logger = logger;
            _uow = uow;
            _pathHelper = pathHelper;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Processes the command to add a new TrialBalanceFile and returns a service response indicating success or failure.
        /// </summary>
        /// <param name="request">Command containing TrialBalanceFile details for creation.</param>
        /// <param name="cancellationToken">Token to cancel the operation if needed.</param>
        public async Task<ServiceResponse<bool>> Handle(AddCommandTrialBalanceFile request, CancellationToken cancellationToken)
        {
       

            try
            {

                var ModelEntity = _mapper.Map<Data.TrialBalanceFile>(request);
                ModelEntity.Id = BaseUtilities.GenerateInsuranceUniqueNumber(12, "TB");
                _trialBalanceFileRepository.Add(ModelEntity);
                await _uow.SaveAsync();

                
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                // Catch any exception, log the error, and return a 500 Internal Server Error response
                var errorMessage = $"Error occurred while saving Account: {BaseUtilities.GetInnerExceptionMessages(e)}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(),
                    request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }
}
 