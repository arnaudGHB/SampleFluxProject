using CBS.NLoan.Data.Dto;
using CBS.NLoan.Data.Dto.BankP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.BankP.Command;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.NLoan.MediatR.BankP.Handler
{
    public class BranchAPICallCommandHandler : IRequestHandler<BranchPICallCommand, ServiceResponse<BranchDto>>
    {
        private readonly ILogger<BranchAPICallCommandHandler> _logger; // Logger for logging handler actions and errors.
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
        public BranchAPICallCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<BranchAPICallCommandHandler> logger,
            PathHelper pathHelper,
            IMediator mediator)
        {

            _userInfoToken = userInfoToken;
            _logger = logger;
            _pathHelper = pathHelper;
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the AddAccountingEventCommand to add a new AccountingEvent.
        /// </summary>
        /// <param name="request">The AddAccountingEventCommand containing AccountingEvent data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<BranchDto>> Handle(BranchPICallCommand request, CancellationToken cancellationToken)
        {
            try
            {
                
                string serializedRequest = JsonConvert.SerializeObject(request);

                // Call the Branch API
                var serviceResponse = await APICallHelper.GetBranch<ServiceResponse<BranchDto>>(_pathHelper, request.BranchId, _userInfoToken.Token);
                if (serviceResponse.StatusCode != 200)
                {
                    return ServiceResponse<BranchDto>.Return403(serviceResponse.Data);
                }
                return serviceResponse;

                // Return the response

            }
            catch (Exception e)
            {
                // Log the error
                string errorMessage = $"Error occurred while sending Branch via Branch API: {e.Message}";
                _logger.LogError(errorMessage);

                // Log the audit error
                await LogAuditError(request, errorMessage);

                // Return a 500 Internal Server Error response with the error message
                return ServiceResponse<BranchDto>.Return500(e, errorMessage);
            }
        }

        private async Task LogAuditError(BranchPICallCommand request, string errorMessage)
        {
            // Serialize the request data
            string serializedRequest = JsonConvert.SerializeObject(request);

            // Log the audit error
            await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), serializedRequest, errorMessage, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);
        }

    }
}
