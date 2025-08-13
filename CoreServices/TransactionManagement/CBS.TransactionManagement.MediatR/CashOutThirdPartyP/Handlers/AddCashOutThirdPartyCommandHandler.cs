using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Helper.Helper;
using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Repository.AccountServices;
using CBS.TransactionManagement.CashOutThirdPartyP.Commands;
using CBS.TransactionManagement.Data.Dto.CashOutThirdPartyP;
using CBS.TransactionManagement.Data.Entity.CashOutThirdPartyP;
using CBS.TransactionManagement.Repository.CashOutThirdPartyP;
using CBS.TransactionManagement.MediatR.User.Command;
using CBS.TransactionManagement.Data.Dto.User;
using AutoMapper;

namespace CBS.TransactionManagement.CashOutThirdPartyP.Handlers
{
    public class AddCashOutThirdPartyCommandHandler : IRequestHandler<AddCashOutThirdPartyCommand, ServiceResponse<CashOutThirdPartyDto>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ICashOutThirdPartyRepository _cashOutThirdPartyRepository;
        private readonly IDepositServices _depositServices;
        private readonly ITellerRepository _tellerRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IMapper _mapper;  // AutoMapper for object mapping.

        private readonly IMediator _mediator;
        private readonly IUnitOfWork<TransactionContext> _unitOfWork;
        private readonly ILogger<AddCashOutThirdPartyCommandHandler> _logger;
        private readonly IConfigRepository _configRepository;
        public AddCashOutThirdPartyCommandHandler(
            IAccountRepository accountRepository,
            ITellerRepository tellerRepository,
            IMediator mediator,
            IUnitOfWork<TransactionContext> unitOfWork,
            ILogger<AddCashOutThirdPartyCommandHandler> logger,
            UserInfoToken userInfoToken = null,
            IDepositServices depositServices = null,
            IConfigRepository configRepository = null,
            ICashOutThirdPartyRepository cashOutThirdPartyRepository = null,
            IMapper mapper = null)
        {
            // Dependency injection
            _accountRepository = accountRepository;
            _tellerRepository = tellerRepository;
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _depositServices = depositServices;
            _configRepository = configRepository;
            _cashOutThirdPartyRepository = cashOutThirdPartyRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<CashOutThirdPartyDto>> Handle(AddCashOutThirdPartyCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if system configuration is set.
                var config = await _configRepository.GetConfigAsync(OperationSourceType.TTP.ToString());
                // Generate transaction reference based on branch type
                string reference = CurrencyNotesMapper.GenerateTransactionReference(_userInfoToken, TransactionType.TTP_WITHDRAWAL.ToString(),false);

                // Get customer account information
                var customerAccount = await _accountRepository.GetAccount(request.AccountNumber, OperationType.Withdrawal.ToString());

                // Retrieve customer information
                var customer = await GetCustomerInfo(customerAccount.CustomerId);

                // Validate customer PIN
                await ValidateCustomerPin(customer.Phone, request.Pin, request.SourceType);

                // Retrieve branch information
                var branch = await GetBranchInfo(customer.BranchId);


                // Retrieve teller information
                var teller = await GetTellerInfo(request.ApplicationCode, branch);

                // Create CashOutThirdParty entity
                var cashOutThirdParty = new CashOutThirdParty
                {
                    AccountId = customerAccount.Id,
                    Id = BaseUtilities.GenerateUniqueNumber(),
                    ExternalTransactionReference = request.ExternalTransactionReference,
                    AccountNumber = request.AccountNumber,
                    Amount = request.Amount,
                    BankId = teller.BankId,
                    BranchId = teller.BranchId,
                    CallBackURL = "N/A",
                    CustomerId = customer.CustomerId,
                    Credit = 0,
                    Debit = request.Amount,
                    DateOfInitiation = BaseUtilities.UtcNowToDoualaTime(),
                    DateOfConfirmation = DateTime.MinValue,
                    PhoneNumber = customer.Phone,
                    SourceType = request.SourceType,
                    Status = Status.Pending.ToString(),
                    TellerId = teller.Id,
                    TransactionReference = reference,
                };

                // Generate OTP
                var otpDto = await GenerateOTP(reference);

                // Add the CashOutThirdParty entity to the repository
                _cashOutThirdPartyRepository.Add(cashOutThirdParty);

                // Save changes to the database
                await _unitOfWork.SaveAsync();

                // Send SMS notification with OTP to customer
                await SendOTPToCustomer(customer, branch, otpDto, request.Amount);
                // Return success response with a message
                return ServiceResponse<CashOutThirdPartyDto>.ReturnResultWith200(_mapper.Map<CashOutThirdPartyDto>(cashOutThirdParty), "Cashout initiated successfully.");
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Transaction: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<CashOutThirdPartyDto>.Return500(e);
            }
        }




        // Retrieves customer information based on the provided customer ID
        private async Task<CustomerDto> GetCustomerInfo(string customerId)
        {
            var customerCommandQuery = new GetCustomerCommandQuery { CustomerID = customerId };
            var customerResponse = await _mediator.Send(customerCommandQuery);
            // Check if retrieving customer information was successful
            if (customerResponse.StatusCode != 200)
                throw new InvalidOperationException("Failed getting member's information");
            // Check if the customer's membership is approved
            if (customerResponse.Data.MembershipApprovalStatus.ToLower() != AccountStatus.approved.ToString().ToLower())
                throw new InvalidOperationException($"Customer membership is not approved. Current Status: {customerResponse.Data.MembershipApprovalStatus}");
            return customerResponse.Data;
        }

        // Retrieves customer information based on the provided customer ID
        private async Task<CustomerPinValidationDto> ValidateCustomerPin(string phoneNumber, string pin, string channel)
        {
            string camCode = "237";
            if (phoneNumber.StartsWith(camCode))
            {
                phoneNumber = phoneNumber.Substring(camCode.Length);
            }
            var customerPinValidation = new CustomerPinValidationCommand { Pin = pin, Telephone = phoneNumber, Channel = channel };
            var customerResponse = await _mediator.Send(customerPinValidation);
            // Check if retrieving customer information was successful
            if (customerResponse.StatusCode != 200)
            {
                var errorMessage = $"Failed validating pin, {customerResponse.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.FullName, LogAction.Update.ToString(), phoneNumber, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                throw new InvalidOperationException(errorMessage);
            }
            if (!customerResponse.Data.ValidationStatus)
            {
                var errorMessage = $"Invalid pin";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.FullName, LogAction.Update.ToString(), phoneNumber, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                throw new InvalidOperationException(errorMessage);

            }
            return customerResponse.Data;
        }
        // Generate OPT
        private async Task<OTPDto> GenerateOTP(string Reference)
        {
            var generateOTPCommand = new GenerateOTPCommand { Id= Reference };
            var customerResponse = await _mediator.Send(generateOTPCommand);
            // Check if retrieving customer information was successful
            if (customerResponse.StatusCode != 200)
            {
                var errorMessage = $"Failed generating otp, {customerResponse.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.FullName, LogAction.Update.ToString(), Reference, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                throw new InvalidOperationException(errorMessage);
            }
            return customerResponse.Data;
        }

        // Retrieves branch information based on the provided branch ID
        private async Task<BranchDto> GetBranchInfo(string branchId)
        {
            var branchCommandQuery = new GetBranchByIdCommand { BranchId = branchId };
            var branchResponse = await _mediator.Send(branchCommandQuery);
            // Check if retrieving branch information was successful
            if (branchResponse.StatusCode != 200)
                throw new InvalidOperationException("Failed getting branch information.");
            return branchResponse.Data;
        }


        // Retrieves teller information based on the provided application code and branch
        private async Task<Teller> GetTellerInfo(string applicationCode, BranchDto branch)
        {
            string tppCode = $"{applicationCode}-{branch.branchCode}";
            var teller = await _tellerRepository.FindBy(t => t.Code == tppCode).FirstOrDefaultAsync();
            // Check if the teller exists
            if (teller == null)
                throw new InvalidOperationException($"3PP with application code: {tppCode} does not exist.");
            return teller;
        }

        // Sends SMS notification to the customer
        private async Task SendOTPToCustomer(CustomerDto customer, BranchDto branch, OTPDto oTPDto, decimal amount)
        {
            string bankName = branch.name;
            string englishMessage = $"{customer.FirstName} {customer.LastName}, use the OTP {oTPDto.OTP} to complete your cashout request of {BaseUtilities.FormatCurrency(amount)}. Your OTP code expires in the next {oTPDto.ExpireDate.Subtract(DateTime.Now).Minutes} minutes.\nDate and Time: {BaseUtilities.UtcToDoualaTime(DateTime.Now)}.\nThank you for banking with us.\n{bankName}.";

            string frenchMessage = $"{customer.FirstName} {customer.LastName}, utilisez le code OTP {oTPDto.OTP} pour compléter votre demande de retrait de {BaseUtilities.FormatCurrency(amount)}. Votre code OTP expire dans les prochaines {oTPDto.ExpireDate.Subtract(DateTime.Now).Minutes} minutes.\nDate et Heure: {BaseUtilities.UtcToDoualaTime(DateTime.Now)}.\nMerci de faire affaire avec nous.\n{bankName}.";

            string msg = customer.Language.ToLower() == "french" ? frenchMessage : englishMessage;

            var sMSPICallCommand = new SendSMSPICallCommand
            {
                messageBody = msg,
                recipient = customer.Phone
            };

            await _mediator.Send(sMSPICallCommand);
        }




    }
}
