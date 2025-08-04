using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Command;
using Newtonsoft.Json;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Commands;
using AutoMapper.Internal;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the command to add a new AccountingEvent.
    /// </summary>
    public class AddOpenAndCloseOfDayPostingCommandHandler : IRequestHandler<AddOpenAndCloseOfDayPostingCommand, ServiceResponse<bool>>
    {
        private readonly ILogger<AddOpenAndCloseOfDayPostingCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly PathHelper _pathHelper;
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Constructor for initializing the AddAccountingEventCommandHandler.
        /// </summary>
        /// <param name="AccountingEventRepository">Repository for AccountingEvent data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddOpenAndCloseOfDayPostingCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<AddOpenAndCloseOfDayPostingCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            PathHelper pathHelper,
            IMediator mediator)
        {

            _userInfoToken = userInfoToken;
            _logger = logger;
            _uow = uow;
            _pathHelper = pathHelper;
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the AddAccountingEventCommand to add a new AccountingEvent.
        /// </summary>
        /// <param name="request">The AddAccountingEventCommand containing AccountingEvent data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(AddOpenAndCloseOfDayPostingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                string stringifyData = JsonConvert.SerializeObject(request);
                var serviceResponse = await APICallHelper.AccountingAPICalls<ServiceResponse<bool>>(_pathHelper, _userInfoToken.Token, stringifyData, _pathHelper.OpenAndCloseOfTheDayURL);
                return ServiceResponse<bool>.ReturnResultWith200(serviceResponse.Data);
            }
            catch (Exception e)
            {
                string data = JsonConvert.SerializeObject(request);
                string errorMessage = $"Error occurred while calling the accounting API for Open and Close of the day: {data} {e.Message}";
                _logger.LogError(errorMessage);
                await LogAuditError(request, errorMessage);
                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }

        private async Task LogAuditError(AddOpenAndCloseOfDayPostingCommand request, string errorMessage)
        {
            await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);
        }


    }

}
