using AutoMapper;
using CBS.NLoan.Repository.FeePolicyP;
using CBS.TransactionManagement.Data.Entity.MemberAccountConfiguration;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MemberAccountConfiguration.Commands;
using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.Repository.FeeP;
using CBS.TransactionManagement.Repository.MemberAccountConfiguration;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.TransactionManagement.Handler
{
    /// <summary>
    /// Handles the command to activate a membership.
    /// </summary>
    public class ActivateMembershipCommandHandler : IRequestHandler<ActivateMembershipCommand, ServiceResponse<List<MemberActivationResponse>>>
    {
        private readonly UserInfoToken _userInfoToken;
        private readonly PathHelper _pathHelper;
        private readonly IMediator _mediator;
        private readonly ILogger<ActivateMembershipCommandHandler> _logger; // Logger for logging actions and errors.

        public ActivateMembershipCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<ActivateMembershipCommandHandler> logger,
            PathHelper pathHelper = null,
            IMediator mediator = null)
        {
            _userInfoToken = userInfoToken;
            _logger = logger;
            _pathHelper = pathHelper;
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the ActivateMembershipCommand to activate a customer membership.
        /// </summary>
        /// <param name="request">The command containing membership activation data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<MemberActivationResponse>>> Handle(ActivateMembershipCommand request, CancellationToken cancellationToken)
        {
            var memberActivationResponses = new List<MemberActivationResponse>();

            try
            {
                // Activate member using API
                var activateMember = new ActivateMember
                {
                    CustomerId = request.CustomerId,
                    MembershipApprovalStatus = Status.Approved.ToString(),
                    BranchCode = request.BranchCode,
                    IsAutomatic = true
                };
                string requestData = JsonConvert.SerializeObject(activateMember);
                var customerData = await APICallHelper.ActivateMemberShip<ServiceResponse<CustomerDto>>(_pathHelper, _userInfoToken.Token, requestData, _pathHelper.ActivateMemberShip);

                if (customerData.StatusCode != 200)
                {
                    string errorMessage = "Failed to activate membership via API.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.MemberActivation, LogLevelInfo.Error);
                    return ServiceResponse<List<MemberActivationResponse>>.Return500(errorMessage);
                }

                var customer = customerData.Data;

                // Process member account activations and fees
                var memberAccounts = ProcessMemberAccountActivations(request, memberActivationResponses);

                // Save member account activations
                var addMemberAccountActivationCommand = new AddMemberAccountActivationCommand
                {
                    BankId = customer.BankId,
                    BranchId = customer.BranchId,
                    CustomerId = customer.CustomerId,
                    IsNew = false,
                    MemberAccountActivations = memberAccounts
                };

                var serviceResponse = await _mediator.Send(addMemberAccountActivationCommand);
                if (serviceResponse.StatusCode != 200)
                {
                    string errorMessage = "Failed to save member account activations.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.MemberActivation, LogLevelInfo.Error);
                    return ServiceResponse<List<MemberActivationResponse>>.Return500(serviceResponse.Message);
                }

                // Log success
                string successMessage = "Membership activated successfully.";
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.MemberActivation, LogLevelInfo.Information);
                return ServiceResponse<List<MemberActivationResponse>>.ReturnResultWith200(memberActivationResponses, successMessage);
            }
            catch (Exception e)
            {
                // Log and handle errors
                string errorMessage = $"Error occurred while activating customer membership: {e.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.MemberActivation, LogLevelInfo.Error);
                return ServiceResponse<List<MemberActivationResponse>>.Return500(errorMessage);
            }
        }

        /// <summary>
        /// Processes the member account activations and updates the response list.
        /// </summary>
        /// <param name="request">The activation command.</param>
        /// <param name="memberActivationResponses">The list to populate with activation responses.</param>
        private List<MemberAccountActivation> ProcessMemberAccountActivations(ActivateMembershipCommand request, List<MemberActivationResponse> memberActivationResponses)
        {
            var memberAccounts = new List<MemberAccountActivation>();
            var feePolicies = request.MemberFeePolicy.Policies;

            foreach (var memberAccount in request.MemberFeePolicy.MemberAccountActivations)
            {
                memberAccount.AmountPaid = memberAccount.CustomeAmount;
                memberAccount.DatePaid = BaseUtilities.UtcNowToDoualaTime();
                memberAccount.Balance -= memberAccount.CustomeAmount;

                if (memberAccount.Balance == 0)
                {
                    memberAccount.IsPaid = true;
                }

                memberAccounts.Add(memberAccount);

                string eventCode = feePolicies
                    .FirstOrDefault(x => x.Fee.OperationFeeType == OperationFeeType.MemberShip.ToString() && x.FeeId == memberAccount.FeeId && x.EventCode != null)
                    ?.EventCode;

                var memberActivationResponse = new MemberActivationResponse
                {
                    Balance = memberAccount.Balance,
                    EventCode = eventCode,
                    Fee = memberAccount.CustomeAmount,
                    Paid = memberAccount.AmountPaid,
                    ServiceName = memberAccount.FeeName,
                };

                memberActivationResponses.Add(memberActivationResponse);
            }

            return memberAccounts;
        }
    }
}

