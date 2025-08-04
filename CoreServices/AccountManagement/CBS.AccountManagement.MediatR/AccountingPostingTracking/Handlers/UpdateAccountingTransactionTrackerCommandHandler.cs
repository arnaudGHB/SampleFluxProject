using CBS.AccountManagement.Common;
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
    public class UpdateAccountingTransactionTrackerCommandHandler : IRequestHandler<UpdateAccountingTransactionTrackerCommand, ServiceResponse<bool>>
    {
        private readonly ILogger<UpdateAccountingTransactionTrackerCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly PathHelper _pathHelper;
        /// <summary>
        /// Constructor for initializing the AddAccountingEventCommandHandler.
        /// </summary>
        /// <param name="AccountingEventRepository">Repository for AccountingEvent data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public UpdateAccountingTransactionTrackerCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<UpdateAccountingTransactionTrackerCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            PathHelper pathHelper)
        {

            _userInfoToken = userInfoToken;
            _logger = logger;
            _uow = uow;
            _pathHelper = pathHelper;

        }

        /// <summary>
        /// Handles the AddAccountingEventCommand to add a new AccountingEvent.
        /// </summary>
        /// <param name="request">The AddAccountingEventCommand containing AccountingEvent data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(UpdateAccountingTransactionTrackerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Filter out any 0 amount from the request
         
                string strigifyData = JsonConvert.SerializeObject(request);
                var customerData = await APICallHelper.TransactionTrackerAccountingAPICalls<ServiceResponse<bool>>(_pathHelper, _userInfoToken.Token, strigifyData,request.Id, _pathHelper.TransactionTrackerAccounting);
                await BaseUtilities.LogAndAuditAsync($"UpdateAccountingTransactionTrackerCommand", request, HttpStatusCodeEnum.OK, LogAction.AccountingPosting, LogLevelInfo.Information);
                return ServiceResponse<bool>.ReturnResultWith200(customerData.Data, "Accounting posting was successfull");
            }
            catch (Exception e)
            {
               
                string strigifyData = JsonConvert.SerializeObject(request);
                var errorMessage = $"Error occurred while posting accounting entries for UpdateAccountingTransactionTrackerCommand: {e.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.AccountingPosting, LogLevelInfo.Error);
                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }
    }
}
