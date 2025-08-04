using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.Command;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.NLoan.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new AccountingEvent.
    /// </summary>
    public class AddAccountingPostingCommandHandler : IRequestHandler<AddAccountingPostingCommand, ServiceResponse<bool>>
    {
        private readonly ILogger<AddAccountingPostingCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<LoanContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly PathHelper _pathHelper;
        /// <summary>
        /// Constructor for initializing the AddAccountingEventCommandHandler.
        /// </summary>
        /// <param name="AccountingEventRepository">Repository for AccountingEvent data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddAccountingPostingCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<AddAccountingPostingCommandHandler> logger,
            IUnitOfWork<LoanContext> uow,
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
        public async Task<ServiceResponse<bool>> Handle(AddAccountingPostingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                string strigifyData = JsonConvert.SerializeObject(request);
                var customerData = await APICallHelper.AccountingAPICalls<ServiceResponse<bool>>(_pathHelper, _userInfoToken.Token, strigifyData, _pathHelper.LoanApprovalPostingURL);
                return ServiceResponse<bool>.ReturnResultWith200(customerData.Data,"Accounting posting was successfull");
            }
            catch (Exception e)
            {
                string data = JsonConvert.SerializeObject(request);
                var errorMessage = $"Error occurred while posting accounting entries";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<bool>.Return500("Accounting returs an error.");
            }
        }
    }

}
