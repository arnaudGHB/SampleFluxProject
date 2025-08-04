
using MediatR;
using CBS.CUSTOMER.HELPER.Helper;
using AutoMapper;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.REPOSITORY;
using BusinessServiceLayer.Objects.SmsObject;
using Microsoft.EntityFrameworkCore;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR;

namespace CBS.CUSTOMER.MEDIATR
{


    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using NPOI.HPSF;

    /// <summary>
    /// Handles the command to change membership status and update account confirmation numbers.
    /// Includes functionality for sending SMS notifications and ensuring unique confirmation numbers.
    /// </summary>
    public class ChangeMembershipStatusCommandHandler : IRequestHandler<ChangeMembershipStatusCommand, ServiceResponse<CreateCustomer>>
    {
        private readonly ICustomerRepository _CustomerRepository; // Repository for accessing Customer data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<ChangeMembershipStatusCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow; // Unit of Work for managing transactions.
        private readonly PathHelper _PathHelper; // Helper for managing paths.
        private readonly ICustomerSmsConfigurationRepository _CustomerSmsConfigurationRepository; // Repository for accessing CustomerSmsConfiguration data.
        private readonly SmsHelper _SmsHelper; // Helper for sending SMS.
        private readonly UserInfoToken _UserInfoToken; // Token containing user information.
        private readonly IMediator _mediator; // Mediator for publishing events.

        /// <summary>
        /// Constructor for initializing dependencies.
        /// </summary>
        public ChangeMembershipStatusCommandHandler(
            ICustomerRepository CustomerRepository,
            IMapper mapper,
            ILogger<ChangeMembershipStatusCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            ICustomerSmsConfigurationRepository CustomerSmsConfigurationRepository,
            PathHelper pathHelper,
            UserInfoToken userInfoToken,
            IMediator mediator)
        {
            _CustomerRepository = CustomerRepository;
            _CustomerSmsConfigurationRepository = CustomerSmsConfigurationRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _PathHelper = pathHelper;
            _SmsHelper = new SmsHelper(_PathHelper);
            _UserInfoToken = userInfoToken;
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the command to change the membership status and generate unique account confirmation numbers.
        /// </summary>
        public async Task<ServiceResponse<CreateCustomer>> Handle(ChangeMembershipStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Find the customer by ID
                var existingCustomer = await _CustomerRepository.FindAsync(request.CustomerId);

                // Return 404 if the customer is not found
                if (existingCustomer == null)
                {
                    return ServiceResponse<CreateCustomer>.Return404("Customer Not Found");
                }

                // Generate unique confirmation number if the customer type is MemberAccount
                if (existingCustomer.CustomerType == "MemberAccount")
                {
                    existingCustomer.AccountConfirmationNumber = await GenerateUniqueConfirmationNumber(existingCustomer.BranchId, request.BranchCode);
                }

                // Update membership approval status
                existingCustomer.MembershipApprovalStatus = request.MembershipApprovalStatus;
                existingCustomer.MembershipApprovalBy = _UserInfoToken.FullName;
                existingCustomer.MembershipApprovedDate = BaseUtilities.UtcNowToDoualaTime().ToString();
                // Update the customer record in the repository
                _CustomerRepository.Update(existingCustomer);

                // Save changes to the database
                await _uow.SaveAsync();

                // Log the update operation
                _logger.LogInformation($"Updated membership status for customer ID: {request.CustomerId}");

                // Send SMS notification to the customer
                await SendSms(existingCustomer, request.MembershipApprovalStatus);

                // Map the updated customer entity to DTO and return it
                var GetCustomer = _mapper.Map<CreateCustomer>(existingCustomer);
                return ServiceResponse<CreateCustomer>.ReturnResultWith200(GetCustomer);
            }
            catch (Exception e)
            {
                // Log error details
                var errorMessage = $"Error occurred while saving Customer: {e.Message}";
                _logger.LogError(errorMessage);

                // Return error response
                return ServiceResponse<CreateCustomer>.Return500(e);
            }
        }

        /// <summary>
        /// Generates a unique account confirmation number for a branch.
        /// Ensures that the number does not conflict with existing numbers.
        /// </summary>
        private async Task<string> GenerateUniqueConfirmationNumber(string branchId, string branchCode)
        {
            // Retrieve existing confirmation numbers for the branch and process in memory
            var existingNumbers = await _CustomerRepository
                .FindBy(x => x.BranchId == branchId && !x.IsDeleted && x.CustomerType == "MemberAccount" && !string.IsNullOrEmpty(x.AccountConfirmationNumber))
                .Select(x => x.AccountConfirmationNumber)
                .ToListAsync();

            // Extract the maximum serial number on the client side
            int maxAccountNumber = existingNumbers
                .Select(num => int.Parse(num.Substring(3))) // Extract serial part
                .DefaultIfEmpty(0)
                .Max();

            // Calculate the next account number
            int nextAccountNumber = maxAccountNumber + 1;

            // Generate the 7-digit serial number padded with zeros
            string serialNumber = nextAccountNumber.ToString("D7");

            // Return the full confirmation number
            return branchCode + serialNumber;
        }


        /// <summary>
        /// Sends an SMS notification to the customer about their membership status.
        /// </summary>
        private async Task SendSms(DATA.Entity.Customer CustomerEntity, string status)
        {
            // Initialize the message content
            string msg = null;

            // Construct the message based on the customer's language preference
            if (CustomerEntity != null && CustomerEntity.Language != null && CustomerEntity.Language.ToLower() == "english")
            {
                msg = $"Greeting {CustomerEntity.FirstName} {CustomerEntity.LastName}, your membership status with {CustomerEntity.BankName} have been {status}.";
            }
            else
            {
                msg = $"Bonjour {CustomerEntity.FirstName} {CustomerEntity.LastName}, votre statut d'adhésion avec {CustomerEntity.BankName} a été {status}";
            }

            // Create the SMS request DTO
            var smsRequest = new SubSmsRequestDto()
            {
                Message = msg,
                Msisdn = CustomerEntity.Phone,
                Token = _UserInfoToken.Token
            };

            // Log the SMS sending operation
            _logger.LogInformation($"Sending SMS to customer ID: {CustomerEntity.CustomerId}");

            // Optionally send SMS using SmsHelper
            // await _SmsHelper.SendSms(smsRequest);
        }
    }

}
