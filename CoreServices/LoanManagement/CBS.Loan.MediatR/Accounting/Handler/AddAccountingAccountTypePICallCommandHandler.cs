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
    public class AddAccountingAccountTypePICallCommandHandler : IRequestHandler<AddAccountingAccountTypePICallCommand, ServiceResponse<AddAccountingAccountTypeResponse>>
    {
        private readonly ILogger<AddAccountingAccountTypePICallCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<LoanContext> _uow;
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
        public AddAccountingAccountTypePICallCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<AddAccountingAccountTypePICallCommandHandler> logger,
            IUnitOfWork<LoanContext> uow,
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
        public async Task<ServiceResponse<AddAccountingAccountTypeResponse>> Handle(AddAccountingAccountTypePICallCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Filter out any null ChartOfAccountId from the request
                var newData = request.ProductAccountBookDetails.Where(detail => detail.ChartOfAccountId != null).ToList();

                // Create a new command with filtered data
                var addAccountingAccountTypePICallCommand = new AddAccountingAccountTypePICallCommand
                {
                    ProductAccountBookDetails = newData,
                    ProductAccountBookId = request.ProductAccountBookId,
                    ProductAccountBookName = request.ProductAccountBookName,
                    ProductAccountBookType = request.ProductAccountBookType
                };

                // Serialize the new command
                string stringifyData = JsonConvert.SerializeObject(addAccountingAccountTypePICallCommand);

                // Call the accounting API
                var serviceResponse = await APICallHelper.AccountingAPICalls<ServiceResponse<AddAccountingAccountTypeResponse>>(_pathHelper, _userInfoToken.Token, stringifyData, _pathHelper.AccountingAccoutTypeCreateURL);

                // Return the response
                return ServiceResponse<AddAccountingAccountTypeResponse>.ReturnResultWith200(serviceResponse.Data);
            }
            catch (Exception e)
            {
                // Log the error
                string errorMessage = $"Error occurred while creating account type by calling accounting API: {e.Message}";
                _logger.LogError(errorMessage);
                // Log the audit error
                await LogAuditError(request, errorMessage);

                // Return a 500 Internal Server Error response with the error message
                return ServiceResponse<AddAccountingAccountTypeResponse>.Return500("Accounting returs an error.");
            }
        }

        private async Task LogAuditError(AddAccountingAccountTypePICallCommand request, string errorMessage)
        {
            // Serialize the request data
            string requestData = JsonConvert.SerializeObject(request);

            // Log the audit error
            await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);
        }

    }

}
