using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Data.Dto.LoanRepayment;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.LoanRepayment.Command;
using CBS.TransactionManagement.MediatR.Accounting.Command;
using CBS.TransactionManagement.MediatR.CMoneyNotifications.Commands;
using CBS.TransactionManagement.MediatR.Loans.Queries;
using CBS.TransactionManagement.MediatR.Operations.Cash.CashIn.Services;
using CBS.TransactionManagement.MediatR.TransactionTrackerAccountingData.Commands;
using CBS.TransactionManagement.Queries;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.MediatR.UtilityServices
{
    public class APIUtilityServicesRepository : IAPIUtilityServicesRepository
    {
        private readonly IMediator _mediator;
        private readonly ILogger<NormalDepositServices> _logger; // Logger for logging handler actions and errors.
        private readonly IServiceScopeFactory _scopeFactory;
        public APIUtilityServicesRepository(IMediator mediator, ILogger<NormalDepositServices> logger, IServiceScopeFactory scopeFactory)
        {
            _mediator=mediator;
            _logger=logger;
            _scopeFactory=scopeFactory;
        }

        /// <summary>
        /// Initiates loan repayment for salary standing orders using an asynchronous background task.
        /// </summary>
        /// <param name="salaryCode">The salary code used to identify the salary batch for processing.</param>
        public void SalaryStandingOrderLoanRepayment(string salaryCode, UserInfoToken userInfoToken)
        {
            ProcessSalaryStandingOrderLoanRepayment(salaryCode, userInfoToken).FireAndForget(ex =>
            {
                _logger.LogError(ex, $"[ERROR] Salary Standing Order Loan Repayment failed for Salary Code: {salaryCode}");
            });
        }

        /// <summary>
        /// Background task to process salary standing order loan repayment asynchronously.
        /// </summary>
        private  async Task ProcessSalaryStandingOrderLoanRepayment(string salaryCode, UserInfoToken userInfoToken)
        {
            try
            {
                // Step 1: Log the initiation of the salary standing order loan repayment.
                string initiationMessage = $"[INFO] Initiating Salary Standing Order Loan Repayment for Salary Code: {salaryCode} in the background.";
                _logger.LogInformation(initiationMessage);
                await BaseUtilities.LogAndAuditAsync(initiationMessage, salaryCode, HttpStatusCodeEnum.OK, LogAction.SalaryStandingOrderLoanRefund, LogLevelInfo.Information, userInfoToken.FullName, userInfoToken.Token);

                using (var scope = _scopeFactory.CreateScope())
                {
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    var command = new AddLoanSSOBulkRepaymentCommand { SalaryCode = salaryCode, UserInfoToken=userInfoToken };
                    await mediator.Send(command);
                }
               
                string successMessage = $"[SUCCESS] Background Salary Standing Order Loan Repayment successfully processed for Salary Code: {salaryCode}.";
                _logger.LogInformation(successMessage);
                await BaseUtilities.LogAndAuditAsync(successMessage, salaryCode, HttpStatusCodeEnum.OK, LogAction.SalaryStandingOrderLoanRefund, LogLevelInfo.Information, userInfoToken.FullName, userInfoToken.Token);
            }
            catch (Exception ex)
            {
                // Step 4: Log and audit errors to ensure failures are traceable.
                string errorMessage = $"[ERROR] Salary Standing Order Loan Repayment failed for Salary Code: {salaryCode}. Exception: {ex.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, salaryCode, HttpStatusCodeEnum.InternalServerError, LogAction.SalaryStandingOrderLoanRefund, LogLevelInfo.Error, userInfoToken.FullName, userInfoToken.Token);
            }
        }


        public async Task<bool> CreateLoanAccount(string memberId, UserInfoToken userInfoToken)
        {
            var addLoan = new AddLoanAccountCommand
            {
                CustomerId = memberId,
                BranchId = userInfoToken.BranchID,
                BankId=userInfoToken.BankID, IsBGS=true, UserInfoToken=userInfoToken
            };

           var respons= await _mediator.Send(addLoan);
            return respons.Data;
        }
        //Method to retrieve customer information
        public async Task<CustomerDto> GetCustomer(string customerId)
        {
            var customerCommandQuery = new GetCustomerCommandQuery { CustomerID = customerId }; // Create command to get customer.
            var customerResponse = await _mediator.Send(customerCommandQuery); // Send command to _mediator.

            if (customerResponse == null)
            {
                var errorMessage = "Error; Null";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            // Check if customer information retrieval was successful
            if (customerResponse.StatusCode != 200)
            {
                var errorMessage = "Failed getting member's information";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }

            var customer = customerResponse.Data; // Get customer data from response.
            customer.Name = $"{customer.FirstName} {customer.LastName}";
            //EnsureCustomerMembershipApproved(customer); // Ensure customer membership is approved.

            return customer; // Return customer data.
        }

        //Method to retrieve Loan repayment order information
        public async Task<LoanRepaymentOrderDto> GetSalaryLoanRepaymentOrder()
        {
            var getLoanRepaymentOrderQuery = new GetLoanRepaymentOrderQuery { LoanRepaymentOrderType = LoanProductRepaymentOrderType.SalaryRefundOrder.ToString() }; // Create command to get customer.
            var getLoanRepaymentOrderResponse = await _mediator.Send(getLoanRepaymentOrderQuery); // Send command to _mediator.

            if (getLoanRepaymentOrderResponse == null)
            {
                var errorMessage = "Error; Null";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            // Check if customer information retrieval was successful
            if (getLoanRepaymentOrderResponse.StatusCode != 200)
            {
                var errorMessage = "Failed getting member's information";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }

            var loanRepaymentOrderDto = getLoanRepaymentOrderResponse.Data.LoanProductRepaymentOrderType != null ? getLoanRepaymentOrderResponse.Data : new()
            {
                InterestOrder = 1,
                InterestRate = 50,
                CapitalOrder = 2,
                CapitalRate = 50,
                FineOrder = 3,
                FineRate = 0,
                LoanProductRepaymentOrderType = LoanProductRepaymentOrderType.SalaryRefundOrder.ToString()
            }; // Get Loan Repayment Order from response.



            return loanRepaymentOrderDto; // Return customer data.
        }
        public void SalaryAccountToAccountTransfer(string transactionReference, DateTime accountingDate, UserInfoToken userInfoToken)
        {
            var addSalaryAccountToAccountTransfer = new AddSalaryAccountToAccountTransferCommand
            {
                ReferenceCode = transactionReference,
                AccountingDate = accountingDate,
                UserInfoToken=userInfoToken
            };
            using (var scope = _scopeFactory.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                mediator.Send(addSalaryAccountToAccountTransfer).FireAndForget(ex => Console.WriteLine($"Error: {ex.Message}"));
            }

            
        }

        //AddSalaryAccountToAccountTransferCommand
        public async Task<List<BranchDto>> GetBranches()
        {
            var branchCommandQuery = new GetBranchesCommand(); // Create command to get branch.
            var branchResponse = await _mediator.Send(branchCommandQuery); // Send command to _mediator.

            // Check if branch information retrieval was successful
            if (branchResponse.StatusCode != 200)
            {
                var errorMessage = $"Failed to retrieve branch information: {branchResponse.Message}";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            return branchResponse.Data; // Return branch data.
        }

        public async Task<List<LightLoanDto>> GetOpenLoansByBranchQuery(string branchid)
        {
            var getOpenLoans = new GetOpenLoansByBranchQuery { BranchId=branchid }; // Create command to get branch.
            var branchResponse = await _mediator.Send(getOpenLoans); // Send command to _mediator.

            // Check if branch information retrieval was successful
            if (branchResponse.StatusCode != 200)
            {
                var errorMessage = $"Failed to retrieve branch information: {branchResponse.Message}";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            return branchResponse.Data; // Return branch data.
        }
        
        public async Task CreatAccountingRLog(string CommandJsonObject, CommandDataType commandDataType, string TransactionReferenceId, DateTime accountingDate, string destinationUrl)
        {
            var addTransactionTrackerAccountingCommand = new AddTransactionTrackerAccountingCommand { CommandDataType=commandDataType, CommandJsonObject=CommandJsonObject, TransactionDate=accountingDate, TransactionReferenceId=TransactionReferenceId, DestinationUrl= destinationUrl };
            var customerResponse = await _mediator.Send(addTransactionTrackerAccountingCommand);
        }
        public async Task CreatAccountingRLogBG(string CommandJsonObject, CommandDataType commandDataType, string TransactionReferenceId, DateTime accountingDate, string destinationUrl, UserInfoToken userInfoToken)
        {
            var addTransactionTrackerAccountingCommand = new AddTransactionTrackerAccountingCommand { CommandDataType=commandDataType, CommandJsonObject=CommandJsonObject, TransactionDate=accountingDate, TransactionReferenceId=TransactionReferenceId, DestinationUrl= destinationUrl , UserInfoToken=userInfoToken, IsBG=true,};
            var customerResponse = await _mediator.Send(addTransactionTrackerAccountingCommand);
        }
        public async Task PushNotification(string MemberReference, PushNotificationTitle pushNotificationTitle, string NotificationBody)
        {
            var cMoneyNotification = new CMoneyNotificationCommand(pushNotificationTitle.ToString(), NotificationBody, MemberReference);
            var response = await _mediator.Send(cMoneyNotification);
        }
        public async Task SendSMSNotification(string TelephoneNumber, string MessageBody)
        {
            var sMSPICallCommand = new SendSMSPICallCommand { messageBody=MessageBody, recipient=TelephoneNumber, senderService="n/a"};
            var response = await _mediator.Send(sMSPICallCommand);
        }
        public async Task<List<CustomerDto>> GetAllMembers(string branchid="All",string queryParamter= "bymatricule")
        {
            var getAllCustomerListingsQuery = new GetAllCustomerListingsQuery { BranchId=branchid, DateFrom=DateTime.Now, DateTo=DateTime.Now, LegalFormStatus="n/a", MembersStatusType="n/a", QueryParameter=queryParamter, PageSize=5000, PageNumber=1 };
            var response = await _mediator.Send(getAllCustomerListingsQuery);
            if (response.Data==null)
            {
                response.Data=new List<CustomerDto>();
            }
            return response.Data;
        }
        
        //CMoneyNotificationCommand
        public async Task<BranchDto> GetBranch(string branchid)
        {
            var branchCommandQuery = new GetBranchByIdCommand { BranchId = branchid }; // Create command to get branch.
            var branchResponse = await _mediator.Send(branchCommandQuery); // Send command to _mediator.

            // Check if branch information retrieval was successful
            if (branchResponse.StatusCode != 200)
            {
                var errorMessage = $"Failed to retrieve branch information: {branchResponse.Message}";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            return branchResponse.Data; // Return branch data.
        }
        public async Task<List<Loan>> GetMembersLoans(List<string> CustomerIds, string OperationType, string loanStatus)
        {
            var getLoansByMultiple = new GetLoansByMultipleCustomersQuery { CustomerIds=CustomerIds, OperationType=OperationType, QueryParameter=loanStatus }; // Create command to 
            var branchResponse = await _mediator.Send(getLoansByMultiple); // Send command to _mediator.
            if (branchResponse.StatusCode != 200)
            {
                var errorMessage = $"Failed to retrieve loans: {branchResponse.Message}";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }
            return branchResponse.Data;
        }
        public async Task<List<CustomerDto>> GetMultipleMembers(List<string> Matricules, string OperationType, bool isMatricule)
        {
            var getCustomersByMatricules = new GetCustomersByMatriculesQuery { Matricules=Matricules, OperationType=OperationType, IsMatricule=isMatricule }; // Create command to 
            var branchResponse = await _mediator.Send(getCustomersByMatricules); // Send command to _mediator.
            if (branchResponse.StatusCode != 200)
            {
                var errorMessage = $"Failed to retrieve loans: {branchResponse.Message}";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }
            return branchResponse.Data;
        }
        public async Task<List<CustomerDto>> GetMembersWithMatriculeByBranchQuery(string branchid)
        {
            var getMembersWithMatricule = new GetMembersWithMatriculeByBranchQuery { BranchId=branchid }; // Create command to 
            var branchResponse = await _mediator.Send(getMembersWithMatricule); // Send command to _mediator.
            if (branchResponse.StatusCode != 200)
            {
                var errorMessage = $"Failed to retrieve members for branch with matricules: {branchResponse.Message}";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }
            return branchResponse.Data;
        }


    }
    public static class TaskExtensions
    {
        public static void FireAndForget(this Task task, Action<Exception>? onError = null)
        {
            task.ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    onError?.Invoke(t.Exception);
                }
            }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }

}
