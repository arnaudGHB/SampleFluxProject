using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Command;
using Newtonsoft.Json;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.LoanRepayment.Command;
using CBS.TransactionManagement.Data.Dto.LoanRepayment;
using CBS.TransactionManagement.MediatR.LoanRepayment.Command;

namespace CBS.TransactionManagement.LoanRepayment.Handlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    public class AddLoanSSOBulkRepaymentCommandHandler : IRequestHandler<AddLoanSSOBulkRepaymentCommand, ServiceResponse<bool>>
    {
        private readonly ILogger<AddLoanSSOBulkRepaymentCommandHandler> _logger; // Logger for error tracking and auditing.
        private readonly UserInfoToken _userInfoToken; // User authentication token.
        private readonly PathHelper _pathHelper; // Path helper for API call references.

        /// <summary>
        /// Constructor for initializing the Salary Standing Order (SSO) Bulk Repayment Handler.
        /// </summary>
        /// <param name="userInfoToken">User information token for authorization.</param>
        /// <param name="logger">Logger instance for monitoring operations.</param>
        /// <param name="pathHelper">Helper class for API endpoints.</param>
        public AddLoanSSOBulkRepaymentCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<AddLoanSSOBulkRepaymentCommandHandler> logger,
            PathHelper pathHelper)
        {
            _userInfoToken = userInfoToken;
            _logger = logger;
            _pathHelper = pathHelper;
        }

        /// <summary>
        /// Handles the bulk salary standing order loan refund process by calling the appropriate API.
        /// </summary>
        /// <param name="request">SSO bulk repayment request containing the salary code.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>Service response indicating success or failure.</returns>
        public async Task<ServiceResponse<bool>> Handle(AddLoanSSOBulkRepaymentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Convert request data to JSON format for logging and API transmission.
                string requestData = JsonConvert.SerializeObject(request);
                string msg = $"[INFO] Initiating Salary Standing Order (SSO) Bulk Loan Repayment. Request Data: {requestData}";
                _logger.LogInformation(msg);
                await BaseUtilities.LogAndAuditAsync(msg, request, HttpStatusCodeEnum.OK, LogAction.SalaryStandingOrderLoanRefund, LogLevelInfo.Information,request.UserInfoToken.FullName,request.UserInfoToken.Token);

                // Step 2: Call the Salary Standing Order Loan Refund API
                var apiResponse = await APICallHelper.PostData<ServiceResponse<bool>>(
                    _pathHelper.LoanBaseURL,
                    _pathHelper.LoanSOBulkRepayment,
                    requestData,
                      request.UserInfoToken.Token
                );

                // Step 3: Check API response status
                if (apiResponse.StatusCode == 200)
                {
                    string successMessage = $"[SUCCESS] SSO Bulk Loan Repayment processed successfully for Salary Code: {request.SalaryCode}.";
                    _logger.LogInformation(successMessage);
                    await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.SalaryStandingOrderLoanRefund, LogLevelInfo.Information, request.UserInfoToken.FullName, request.UserInfoToken.Token);

                    return ServiceResponse<bool>.ReturnResultWith200(true, successMessage);
                }
                else
                {
                    string failureMessage = $"[ERROR] SSO Bulk Loan Repayment API call failed. Status Code: {apiResponse.StatusCode}.";
                    _logger.LogError(failureMessage);
                    await BaseUtilities.LogAndAuditAsync(failureMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.SalaryStandingOrderLoanRefund, LogLevelInfo.Error, request.UserInfoToken.FullName, request.UserInfoToken.Token);

                    return ServiceResponse<bool>.Return500(failureMessage);
                }
            }
            catch (Exception e)
            {
                // Step 4: Log and audit API failure
                string errorMessage = $"[CRITICAL ERROR] SSO Bulk Loan Repayment process failed for Salary Code: {request.SalaryCode}. Exception: {e.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.SalaryStandingOrderLoanRefund, LogLevelInfo.Error, request.UserInfoToken.FullName, request.UserInfoToken.Token);

                throw;
            }
        }
    }

}
