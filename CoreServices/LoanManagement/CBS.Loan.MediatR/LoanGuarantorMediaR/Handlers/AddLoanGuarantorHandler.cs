using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.CustomerP;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Data.Enums;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.CustomerP.Command;
using CBS.NLoan.MediatR.CustomerP.Query;
using CBS.NLoan.MediatR.LoanGuarantorMediaR.Commands;
using CBS.NLoan.Repository.LoanApplicationP;
using CBS.NLoan.Repository.LoanApplicationP.LoanGuarantorP;
using CBS.NLoan.Repository.LoanProductP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanGuarantorMediaR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Loan.
    /// </summary>
    public class AddLoanGuarantorHandler : IRequestHandler<AddLoanGuarantorCommand, ServiceResponse<LoanGuarantorDto>>
    {
        private readonly ILoanGuarantorRepository _LoanGuarantorRepository; // Repository for accessing LoanGuarantor data.
        private readonly ILoanApplicationRepository _loanApplicationRepository;
        private readonly IMediator _mediator;

        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddLoanGuarantorHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<LoanContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddLoanGuarantorCommandHandler.
        /// </summary>
        /// <param name="LoanGuarantorRepository">Repository for LoanGuarantor data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddLoanGuarantorHandler(
            ILoanGuarantorRepository LoanGuarantorRepository,
            IMapper mapper,
            ILogger<AddLoanGuarantorHandler> logger,
            IUnitOfWork<LoanContext> uow,
            ILoanApplicationRepository loanApplicationRepository = null,
            IMediator mediatR = null)
        {
            _LoanGuarantorRepository = LoanGuarantorRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _loanApplicationRepository = loanApplicationRepository;
            _mediator = mediatR;
        }

        public async Task<ServiceResponse<LoanGuarantorDto>> Handle(AddLoanGuarantorCommand request, CancellationToken cancellationToken)
        {
            string logReference = BaseUtilities.GenerateUniqueNumber(); // Generate a unique reference for logging.

            try
            {
                // Step 1: Check if the loan is already guaranteed.
                var existingLoanGuarantor = await CheckIfLoanIsAlreadyGuaranteed(request.LoanApplicationId);
                if (existingLoanGuarantor != null)
                {
                    string message = $"A guarantor already exists for loan application ID: {request.LoanApplicationId}.";
                    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Conflict, LogAction.LoanGuarantor, LogLevelInfo.Warning, logReference);
                    return ServiceResponse<LoanGuarantorDto>.Return409(message);
                }

                // Step 2: Validate the guarantee amount.
                if (!IsValidGuaranteeAmount(request.GuaranteeAmount))
                {
                    string message = "Invalid or missing guarantee amount.";
                    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Forbidden, LogAction.LoanGuarantor, LogLevelInfo.Warning, logReference);
                    return ServiceResponse<LoanGuarantorDto>.Return403(message);
                }

                // Step 3: Check if the loan application exists.
                var loanApplication = await GetLoanApplication(request.LoanApplicationId);
                if (loanApplication == null)
                {
                    string message = $"Loan application with ID {request.LoanApplicationId} not found.";
                    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Conflict, LogAction.LoanGuarantor, LogLevelInfo.Warning, logReference);
                    return ServiceResponse<LoanGuarantorDto>.Return409(message);
                }

                // Step 4: Retrieve customer information for the loan application.
                var customer = await GetCustomerDto(loanApplication.CustomerId);
                if (customer == null)
                {
                    string message = "Failed to retrieve the member's information for the loan guarantor.";
                    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Forbidden, LogAction.LoanGuarantor, LogLevelInfo.Warning, logReference);
                    return ServiceResponse<LoanGuarantorDto>.Return403(message);
                }

                // Step 5: Map the command to a LoanGuarantor entity.
                var loanGuarantorEntity = MapToLoanGuarantorEntity(request);

                // Step 6: If the guarantor is a co-obligor or shotee, validate and block the guarantor's account.
                if (request.GuarantorType == GurantorsTypes.Co_Obligor.ToString() || request.GuarantorType == GurantorsTypes.Shotee.ToString())
                {
                   
                   
                    // Handle missing or invalid account numbers.
                    if (string.IsNullOrWhiteSpace(request.AccountNumber))
                    {
                        string message = "Account number must be provided.";
                        await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Forbidden, LogAction.LoanGuarantor, LogLevelInfo.Warning, logReference);
                        return ServiceResponse<LoanGuarantorDto>.Return403(message);
                    }
                    else if (!long.TryParse(request.AccountNumber, out _))
                    {
                        string message = "Invalid account number format. Please enter a valid long integer.";
                        await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Forbidden, LogAction.LoanGuarantor, LogLevelInfo.Warning, logReference);
                        return ServiceResponse<LoanGuarantorDto>.Return403(message);
                    }

                    // Block the account associated with the guarantor.
                    var blockedAccountResult = await BlockAccount(request.AccountNumber, request.GuaranteeAmount, request.LoanApplicationId);
                    if (blockedAccountResult.StatusCode != 200)
                    {
                        string message = $"Failed to block account {request.AccountNumber}: {blockedAccountResult.Message}";
                        await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Forbidden, LogAction.LoanGuarantor, LogLevelInfo.Warning, logReference);
                        return ServiceResponse<LoanGuarantorDto>.Return403(message);
                    }
                }
                else
                {
                    // Handle missing or invalid account numbers.
                    if (string.IsNullOrEmpty(request.AccountNumber))
                    {
                        request.AccountNumber = "N/A";  // Set default if no account number is provided.
                    }
                }
                // Step 7: Add the new guarantor to the repository and save changes.
                _LoanGuarantorRepository.Add(loanGuarantorEntity);
                await _uow.SaveAsync();

                // Step 8: Map the LoanGuarantor entity to a DTO.
                var loanGuarantorDto = _mapper.Map<LoanGuarantorDto>(loanGuarantorEntity);

                // Step 9: Log success and return the result.
                string successMessage = "Loan guarantor added successfully.";
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.LoanGuarantor, LogLevelInfo.Information, logReference);
                return ServiceResponse<LoanGuarantorDto>.ReturnResultWith200(loanGuarantorDto, successMessage);
            }
            catch (Exception e)
            {
                // Step 10: Log any unexpected errors.
                string errorMessage = $"An error occurred while adding the loan guarantor: {e.Message}";
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.LoanGuarantor, LogLevelInfo.Error, logReference);
                return ServiceResponse<LoanGuarantorDto>.Return500(errorMessage);
            }
        }

        private async Task<LoanGuarantor> CheckIfLoanIsAlreadyGuaranteed(string loanApplicationId)
        {
            return await _LoanGuarantorRepository.FindBy(c => c.LoanApplicationId == loanApplicationId).FirstOrDefaultAsync();
        }

        private bool IsValidGuaranteeAmount(decimal guaranteeAmount)
        {
            return guaranteeAmount > 0;
        }

        private async Task<ServiceResponse<bool>> BlockAccount(string accountNumber, decimal amount, string LoanApplicationId)
        {
            var blockedAccount = new BlockOrUnblockAccountCommand
            {
                AccountNumber = accountNumber,
                Amount = amount,
                Reason = "Guarantee repayment",
                Status = "Blocked",
                LoanApplicationId = LoanApplicationId
            };
            return await _mediator.Send(blockedAccount);
        }

        private async Task<LoanApplication> GetLoanApplication(string loanApplicationId)
        {
            return await _loanApplicationRepository.FindAsync(loanApplicationId);
        }

        private async Task<CustomerDto> GetCustomerDto(string customerId)
        {
            var customerPICallCommand = new GetCustomerCallCommand { CustomerId = customerId };
            var customerPICallCommandResult = await _mediator.Send(customerPICallCommand);
            return customerPICallCommandResult.StatusCode == 200 ? customerPICallCommandResult.Data : null;
        }

        private bool IsSameCustomerAsApplicant(CustomerDto customer, string loanCustomerId)
        {
            return customer.customerId == loanCustomerId;
        }

        private LoanGuarantor MapToLoanGuarantorEntity(AddLoanGuarantorCommand request)
        {
            var loanGuarantorEntity = _mapper.Map<LoanGuarantor>(request);
            // Convert UTC to local time and set it in the entity
            loanGuarantorEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
            loanGuarantorEntity.Id = BaseUtilities.GenerateUniqueNumber();
            return loanGuarantorEntity;
        }
    }

}
