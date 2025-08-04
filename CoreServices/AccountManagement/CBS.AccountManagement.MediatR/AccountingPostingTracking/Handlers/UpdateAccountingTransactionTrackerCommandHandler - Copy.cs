using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Common.Repository.Uow;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Command;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Handlers
{
    public class AddAccountingTransactionTrackerCommandHandler : IRequestHandler<AddAccountingTransactionTrackerCommand, ServiceResponse<bool>>
    {
        private readonly ILogger<AddAccountingTransactionTrackerCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly PathHelper _pathHelper;
        private readonly IMongoUnitOfWork _mongoUnitOfWork; // MongoDB Unit of Work.
        private readonly IMapper _mapper;
        /// <summary>
        /// Constructor for initializing the AddAccountingEventCommandHandler.
        /// </summary>
        /// <param name="AccountingEventRepository">Repository for AccountingEvent data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddAccountingTransactionTrackerCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<AddAccountingTransactionTrackerCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            PathHelper pathHelper,
            IMongoUnitOfWork? mongoUnitOfWork,
            IMapper? mapper)
        {

            _userInfoToken = userInfoToken;
            _logger = logger;
            _uow = uow;
            _pathHelper = pathHelper;
            _mongoUnitOfWork = mongoUnitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Handles the AddAccountingEventCommand to add a new AccountingEvent.
        /// </summary>
        /// <param name="request">The AddAccountingEventCommand containing AccountingEvent data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(AddAccountingTransactionTrackerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Map the AddAuditTrailCommand to an AuditTrail entity
                var auditTrailEntity = _mapper.Map<TransactionTracker>(request);
                auditTrailEntity.CreatedDate = BaseUtilities.UtcToLocal(DateTime.Now);

                // Set BankID and BranchID, handling potential null values in _userInfoToken
                auditTrailEntity.BankId = _userInfoToken?.BankId ?? "Nan";
                auditTrailEntity.BranchId = _userInfoToken?.BranchId ?? "Nan";

                // Set FullName and UserID, defaulting to request.UserName and "Nan" respectively
                auditTrailEntity.CreatedBy = _userInfoToken?.FullName ?? "nan";
                auditTrailEntity.Id = _userInfoToken?.Id ?? "Nan";

              
                auditTrailEntity.Id = BaseUtilities.GenerateUniqueNumber(15);
                // Get the MongoDB repository for AuditTrail
                var auditTrailRepository = _mongoUnitOfWork.GetRepository<TransactionTracker>();


                // Add the TransactionTracker entity to the MongoDB repository
                await auditTrailRepository.InsertAsync(auditTrailEntity);

         
                return ServiceResponse<bool>.ReturnResultWith200(true); 
            }
            catch (Exception e)
            {
               
                string strigifyData = JsonConvert.SerializeObject(request);
                var errorMessage = $"Error occurred while posting accounting entries for AddAccountingTransactionTrackerCommand: {e.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.AccountingPosting, LogLevelInfo.Error);
                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }
    }
}
