
using MediatR;
using CBS.CUSTOMER.HELPER.Helper;
using AutoMapper;

using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.REPOSITORY;

using BusinessServiceLayer.Objects.SmsObject;


using CBS.APICaller.Helper.LoginModel.Authenthication;
using BusinessServiceLayer.Objects.SmsObject;
using Newtonsoft.Json;
using System.Text;

using Microsoft.EntityFrameworkCore;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.COMMON.UnitOfWork;

using CBS.APICaller.Helper;

using CBS.CUSTOMER.MEDIATR.CustomerMediatR;
using Microsoft.IdentityModel.Tokens;

using CBS.CUSTOMER.REPOSITORY.GroupRepo;

using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.REPOSITORY.GroupRepo;
using System.Text.RegularExpressions;
using CBS.Customer.MEDIATR.CustomerMediatR;
using System.Reflection;
using System.Linq;



namespace CBS.CUSTOMER.MEDIATR
{
    /// <summary>
    /// Handles the command to add a new Customer.
    /// </summary>
    public class AddCustomerCommandHandler : IRequestHandler<AddCustomerCommand, ServiceResponse<CreateCustomer>>
    {

        private readonly ICustomerRepository _CustomerRepository; // Repository for accessing Customer data.
        private readonly ICustomerCategoryRepository _CustomerCategoryRepository; // Repository for accessing Category data.
        private readonly IGroupCustomerRepository _GroupCustomerRepository; // Repository for accessing GroupCustomer data.
        private readonly IGroupRepository _GroupRepository; // Repository for accessing Group data.
        private readonly ICustomerSmsConfigurationRepository _CustomerSmsConfigurationRepository; // Repository for accessing CustomerSmsConfiguration data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddCustomerCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _UserInfoToken;
        private readonly PathHelper _PathHelper;
        private readonly SmsHelper _SmsHelper;
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for initializing the AddCustomerCommandHandler.
        /// </summary>
        /// <param name="CustomerRepository">Repository for Customer data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddCustomerCommandHandler(
            ICustomerRepository CustomerRepository,
            ICustomerSmsConfigurationRepository CustomerSmsConfigurationRepository,
            ICustomerCategoryRepository CustomerCategoryRepository,
            IMapper mapper,
            ILogger<AddCustomerCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            PathHelper pathHelper,
            UserInfoToken userInfoToken,
            IGroupRepository groupRepository,
            IGroupCustomerRepository groupCustomerRepository,
            IMediator mediator)
        {
            _CustomerRepository = CustomerRepository;
            _CustomerSmsConfigurationRepository = CustomerSmsConfigurationRepository;
            _mapper = mapper;
            _logger = logger;

            _CustomerCategoryRepository = CustomerCategoryRepository;
            _uow = uow;
            _PathHelper = pathHelper;
            _UserInfoToken = userInfoToken;
            _SmsHelper = new SmsHelper(_PathHelper);
            _GroupRepository = groupRepository;
            _GroupCustomerRepository = groupCustomerRepository;
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the command to add a new customer to the system.
        /// </summary>
        /// <param name="request">The AddCustomerCommand containing customer data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A service response containing the created customer details.</returns>
        public async Task<ServiceResponse<CreateCustomer>> Handle(AddCustomerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Normalize phone number to ensure it has the correct country code.
                request.Phone = BaseUtilities.Add237Prefix(request.Phone);



                if (string.IsNullOrEmpty(request.BankCode))
                {
                    var errorMessage = "[ERROR] Customer registration failed: Bank Code is missing. Please provide a valid Bank Code.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.BadRequest, LogAction.MemberCreation, LogLevelInfo.Error);
                    return ServiceResponse<CreateCustomer>.Return409(errorMessage);
                }

                // Step 3: Define restricted customer types that must be unique per branch.
                var restrictedCustomerTypes = new[] { "DailyCollection", "MobileMoneyMTN", "MobileMoneyOrange", "Remittance", "SalaryCollection", "UnknownSalaryMemberAccount", "AffiliateMemberSalaryAccount" };

                // Step 4: Map request data to customer entity.
                var customerEntity = _mapper.Map<DATA.Entity.Customer>(request);

                // Step 5: Validate Customer ID length.
                if (request.CustomerId.Length > 7)
                {
                    var errorMessage = $"[ERROR] The provided Member Reference {request.CustomerId} exceeds the maximum limit of 7 characters. Please provide a valid Member Reference.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.BadRequest, LogAction.MemberCreation, LogLevelInfo.Error);
                    return ServiceResponse<CreateCustomer>.Return409(errorMessage);
                }

                // Step 6: Generate or validate customer ID.
                string customerid = request.CustomerId.All(c => c == '0')
                    ? await GenerateCustomerCodeAsyncxxxx(request, restrictedCustomerTypes)/*await GenerateCustomerCodeAsync(request, restrictedCustomerTypes)*/
                    : request.CustomerId.PadLeft(7, '0');

                if (customerid.Length > 7)
                {
                    var errorMessage = $"[ERROR] System-generated Member Reference {customerid} exceeds 7 characters. Please contact support for further assistance.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.BadRequest, LogAction.MemberCreation, LogLevelInfo.Error);
                    return ServiceResponse<CreateCustomer>.Return409(errorMessage);
                }

                // Step 7: Assign necessary customer details.
                customerEntity.CreatedDate = BaseUtilities.UtcToLocal(DateTime.UtcNow);
                customerEntity.CustomerId = $"{request.BranchCode}{customerid}";
                customerEntity.IsDailyCollector = request.IsDailyCollector;
                customerEntity.Active = !request.IsFileData;
                customerEntity.MembershipApprovalStatus = request.IsFileData
                    ? MembershipApprovalStatus.Approved.ToString()
                    : MembershipApprovalStatus.Awaits_Validation.ToString();
                customerEntity.Email = request.Email ?? $"{customerEntity.Phone}@gmail.com";
                customerEntity.CustomerCode = customerid;
                customerEntity.PhotoUrl = _PathHelper.DefaultPhotoURL;


                var customer = await _CustomerRepository.FindAsync(customerEntity.CustomerId);
                if (customer!=null)
                {
                    var errorMessage = $"[ERROR] The Member Reference '{customerEntity.CustomerId}' is already assigned to another member [{customer.FirstName} {customer.LastName}] in Branch '{_UserInfoToken.BranchName}'. " +
                    "Each member must have a unique Member Reference. " +
                    "Please verify the reference number or contact the system administrator if this is an error.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Conflict, LogAction.MemberCreation, LogLevelInfo.Error);
                    return ServiceResponse<CreateCustomer>.Return409(errorMessage);

                }


                // Step 8: Generate and encrypt a temporary PIN.
                string temporalPin = request.IsFileData ? "1234" : BaseUtilities.GenerateUniqueNumber(4);
                customerEntity.Pin = BCrypt.Net.BCrypt.HashPassword(temporalPin);
                //SalaryCollection
                // Step 9: Check if the customer type requires unique assignment per branch.
                if (restrictedCustomerTypes.Contains(request.CustomerType) && request.CustomerType!= "UnknownSalaryMemberAccount" && request.CustomerType != "AffiliateMemberSalaryAccount" && request.CustomerType != "SalaryCollection"&& request.CustomerType != "Remittance")
                {
                    var existingCustomer = await _CustomerRepository
                        .FindBy(c => c.CustomerType == request.CustomerType && !c.IsDeleted && c.BranchId == request.BranchId)
                        .FirstOrDefaultAsync();

                    if (existingCustomer != null)
                    {
                        var errorMessage = $"[ERROR] Customer type '{request.CustomerType}' is already assigned to Branch '{_UserInfoToken.BranchName}'. Only one instance is allowed per branch.";
                        _logger.LogError(errorMessage);
                        await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Conflict, LogAction.MemberCreation, LogLevelInfo.Error);
                        return ServiceResponse<CreateCustomer>.Return409(errorMessage);
                    }
                }

                // Step 10: Save the new customer record.
                _CustomerRepository.Add(customerEntity);
                await _uow.SaveAsync();

                // Step 11: Send customer creation SMS & create an account (if applicable).
                if (!request.IsNotDirectRequest && !request.IsFileData)
                {
                    await SendCustomerCreationSms(customerEntity);
                    var memberOrdinaryAccount = new CreateMemberOrdinaryAccount
                    {
                        CustomerId = customerEntity.CustomerId,
                        CustomerName = $"{customerEntity.FirstName} {customerEntity.LastName}"
                    };

                    var accountCreationResponse = await APICallHelper.CreateAccount<ServiceResponse<bool>>(
                        _PathHelper.BaseMemberURL,
                        _PathHelper.CreateMemberUrl,
                        memberOrdinaryAccount,
                        _UserInfoToken.Token
                    );

                    if (accountCreationResponse.StatusCode != 200)
                    {
                        string warningMessage = $"[WARNING] Customer '{customerEntity.CustomerId}' was created successfully, but account creation encountered an issue.";
                        _logger.LogWarning(warningMessage);
                        await BaseUtilities.LogAndAuditAsync(warningMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.MemberCreation, LogLevelInfo.Warning);
                    }
                }

                // Step 12: Log and audit the successful customer creation.
                var createCustomer = _mapper.Map<CreateCustomer>(customerEntity);
                string successMessage = $"[SUCCESS] Customer registration successful! Member Reference: {customerEntity.CustomerId}, Name: {customerEntity.FirstName} {customerEntity.LastName}.";
                _logger.LogInformation(successMessage);
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.MemberCreation, LogLevelInfo.Information);

                return ServiceResponse<CreateCustomer>.ReturnResultWith200(createCustomer);
            }
            catch (Exception e)
            {
                // Step 13: Log and audit unexpected errors.
                string errorMessage = $"[ERROR] An unexpected error occurred while creating Customer '{request.FirstName} {request.LastName}'. Error Details: {e.Message}. Please contact support if the issue persists.";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.MemberCreation, LogLevelInfo.Error);

                return ServiceResponse<CreateCustomer>.Return500(e);
            }
        }

        /// <summary>
        /// Generates a unique customer code based on the provided request and exclusion criteria.
        /// Ensures the customer code is exactly 10 characters long by truncating or padding as needed.
        /// </summary>
        /// <param name="request">The command containing customer and branch details.</param>
        /// <param name="excludedCustomerTypes">Array of customer types to exclude from processing.</param>
        /// <returns>A task that resolves to a unique 10-character customer code.</returns>
        private async Task<string> GenerateCustomerCodeAsyncxxxx(AddCustomerCommand request, string[] excludedCustomerTypes)
        {
            // Check if the request is based on file data
            if (request.IsFileData)
            {
                // Assign a default customer code if none is provided
                if (string.IsNullOrEmpty(request.CustomerCode))
                {
                    request.CustomerCode = "0000000";
                }

                // Ensure the customer code is exactly 10 characters long
                request.CustomerCode = EnsureCustomerCodeSize(request.CustomerCode);

                // Return the processed customer code
                return request.CustomerCode;
            }

            // Assign the branch code from the user token if not provided
            if (string.IsNullOrEmpty(request.BranchCode))
            {
                request.BranchCode = _UserInfoToken.BranchCode;
            }

            string customerId = string.Empty; // Placeholder for the numeric part of the customer ID
            string customerIdToCheck = string.Empty; // Full customer ID including branch code

            // Regular expression to extract the numeric part of the customer code
            var regex = new Regex(@"\d+$");

            // Apply special regex rules if the customer type requires a specific format
            if (excludedCustomerTypes.Contains(request.CustomerType))
            {
                switch (request.CustomerType)
                {
                    case "MemberAccount":
                        // No specific pattern required for Member Accounts
                        break;
                    case "DailyCollection":
                        // Extracts a numeric ID prefixed with "DSA"
                        regex = new Regex(@"^DSA(\d+)$");
                        break;
                    case "MobileMoneyMTN":
                        // Extracts a numeric ID prefixed with "MTCA"
                        regex = new Regex(@"^MTCA(\d+)$");
                        break;
                    case "MobileMoneyOrange":
                        // Extracts a numeric ID prefixed with "OMCA"
                        regex = new Regex(@"^OMCA(\d+)$");
                        break;
                    case "Remittance":
                        // Extracts a numeric ID prefixed with "RMCA"
                        regex = new Regex(@"^RMCA(\d+)$");
                        break;
                    case "SalaryCollection":
                        // Extracts a numeric ID prefixed with "SCA"
                        regex = new Regex(@"^SCA(\d+)$");
                        break;
                    case "UnknownSalaryMemberAccount":
                        // Extracts a numeric ID prefixed with "TMSA"
                        regex = new Regex(@"^TMSA(\d+)$");
                        break;
                    case "AffiliateMemberSalaryAccount":
                        // Extracts a numeric ID prefixed with "TMAA"
                        regex = new Regex(@"^TMAA(\d+)$");
                        break;
                    default:
                        // Default case: Extracts just the numeric part from any format
                        regex = new Regex(@"\d+$");
                        break;
                }
            }

            // Retrieve all customers in the branch, excluding deleted and invalid entries
            var customers = await _CustomerRepository
                .FindBy(x => x.BranchId == request.BranchId &&
                             !x.IsDeleted &&
                             x.CustomerCode != null)
                .AsNoTracking() // Improve query performance by preventing entity tracking
                .Select(x => new { x.CustomerId, x.CustomerCode }) // Fetch only required fields
                .ToListAsync();

            // Determine the starting Customer ID from existing customer codes
            int lastCustomerId = customers.Any()
                ? customers
                    .Select(x => excludedCustomerTypes.Contains(request.CustomerType) ? regex.Match(x.CustomerCode).Groups[1].Value : regex.Match(x.CustomerCode).Value) // Extract numeric part
                    .Where(x => !string.IsNullOrEmpty(x)) // Filter out empty results
                    .Select(x => int.TryParse(x, out int number) ? number : (int?)null) // Parse to integer
                    .Where(x => x.HasValue) // Filter out null results
                    .DefaultIfEmpty(0)
                    .Max().Value + 1 // Get the maximum existing number and increment by 1
                : 1; // Default to 1 if no customers exist

            // Ensure the generated customer ID does not exceed 7 digits
            string lastCustomerIdString = lastCustomerId.ToString();
            if (lastCustomerIdString.Length > 7)
            {
                // If ID is too long, trim the first two characters
                lastCustomerIdString = lastCustomerIdString.Substring(2);
                lastCustomerId = int.Parse(lastCustomerIdString);
            }

            // Generate a unique customer ID
            bool isUnique;
            do
            {
                // Format the numeric part of the customer ID to be exactly 7 digits
                customerId = lastCustomerId.ToString("D7");

                // Combine the branch code and numeric ID to form the full customer code
                customerIdToCheck = $"{customerId}";
                customerIdToCheck = EnsureCustomerCodeSize(customerIdToCheck);

                // Apply specific prefixes based on customer type
                switch (request.CustomerType)
                {
                    case "DailyCollection":
                        // Prefix with "DSA" and retain the last 5 digits
                        customerIdToCheck = $"DSA{customerIdToCheck.Substring(3)}";
                        break;
                    case "MobileMoneyMTN":
                        // Prefix with "MTCA" and retain the last 5 digits
                        customerIdToCheck = $"MTCA{customerIdToCheck.Substring(4)}";
                        break;
                    case "MobileMoneyOrange":
                        // Prefix with "OMCA" and retain the last 5 digits
                        customerIdToCheck = $"OMCA{customerIdToCheck.Substring(4)}";
                        break;
                    case "Remittance":
                        // Prefix with "RMCA" and retain the last 5 digits
                        customerIdToCheck = $"RMCA{customerIdToCheck.Substring(4)}";
                        break;
                    case "SalaryCollection":
                        // Prefix with "SCA" and retain the last 5 digits
                        customerIdToCheck = $"SCA{customerIdToCheck.Substring(3)}";
                        break;
                    case "UnknownSalaryMemberAccount":
                        // Prefix with "TMSA" and retain the last 3 digits
                        customerIdToCheck = $"TMSA{customerIdToCheck.Substring(4)}";
                        break;
                    case "AffiliateMemberSalaryAccount":
                        // Prefix with "TMAA" and retain the last 3 digits
                        customerIdToCheck = $"TMAA{customerIdToCheck.Substring(4)}";
                        break;
                    default:
                        // No specific prefix needed for other customer types
                        break;
                }

                // Check if the generated customer ID is unique within the branch
                isUnique = !await _CustomerRepository.AnyAsync(x =>
                    x.CustomerCode == customerIdToCheck &&
                    x.BranchId == request.BranchId &&
                    !x.IsDeleted);

                // If not unique, increment and try again
                if (!isUnique)
                {
                    lastCustomerId++;
                }

            } while (!isUnique); // Repeat until a unique ID is found

            // Return the finalized unique customer code
            return customerIdToCheck;
        }

        /// <summary>
        /// Ensures a customer code is exactly 10 characters long by truncating or padding as needed.
        /// </summary>
        /// <param name="customerCode">The customer code to process.</param>
        /// <returns>The processed customer code, exactly 10 characters long.</returns>
        private string EnsureCustomerCodeSize(string customerCode)
        {
            // If the customer code exceeds 10 characters, truncate it
            if (customerCode.Length > 7)
            {
                // Skip the first 3 characters and remove positions 4 and 5
                string truncated = customerCode.Substring(3); // Skip first 3
                truncated = truncated.Remove(0, 2); // Remove 4th and 5th positions
                return truncated.PadRight(7, '0'); // Pad to ensure 10 characters
            }

            // If the customer code is less than 10 characters, pad it with leading zeros
            if (customerCode.Length < 7)
            {
                return customerCode.PadLeft(7, '0');
            }

            // Return the code as-is if it's already 10 characters
            return customerCode;
        }




        private async Task SendCustomerCreationSms(DATA.Entity.Customer CustomerEntity)
        {
            DATA.Entity.CustomerSmsConfigurations getSmsTemplate = new();
            if (CustomerEntity != null && CustomerEntity.Language != null && CustomerEntity.Language.ToLower() == "english")
            {
                getSmsTemplate = await _CustomerSmsConfigurationRepository.FindAsync("CUSTOMER_CREATION_NOTIFICATION_ENGLISH");
            }
            else
            {
                getSmsTemplate = await _CustomerSmsConfigurationRepository.FindAsync("CUSTOMER_CREATION_NOTIFICATION_FRENCH");
            }

            if (getSmsTemplate != null && getSmsTemplate.SmsTemplate != null)
            {
                var smsRequest = new SubSmsRequestDto()
                {
                    Message = getSmsTemplate.SmsTemplate.Replace("{FirstName}", CustomerEntity.FirstName).Replace("{LastName}", $"{CustomerEntity.LastName}").Replace("{BankName}", $"{CustomerEntity.BankName}").Replace("{CustomerId}", $"{CustomerEntity.CustomerId}"),
                    Msisdn = CustomerEntity.Phone,
                    Token = _UserInfoToken.Token
                };
                await _SmsHelper.SendSms(smsRequest);
            }
            else
            {
                if (CustomerEntity != null && CustomerEntity.Language != null && CustomerEntity.Language.ToLower() == "english")
                {
                    // English message
                    string msg = $"Dear {CustomerEntity.FirstName}, your membership has been successfully initiated. To complete your membership with {CustomerEntity.BankName}, please pay the membership fee at the cash desk. Your Member Reference Code is: {CustomerEntity.CustomerId}. For assistance, visit any of our branches.";

                    var smsRequest = new SubSmsRequestDto()
                    {
                        Message = msg,
                        Msisdn = CustomerEntity.Phone,
                        Token = _UserInfoToken.Token
                    };
                    await _SmsHelper.SendSms(smsRequest);
                }

                else
                {
                    // French message
                    string msg = $"Cher(e) {CustomerEntity.FirstName}, votre adhésion a été initiée avec succès. Pour compléter votre adhésion avec {CustomerEntity.BankName}, veuillez régler les frais d'adhésion au guichet. Votre code de référence membre est : {CustomerEntity.CustomerId}. Pour toute assistance, rendez-vous dans une de nos succursales.";

                    var smsRequest = new SubSmsRequestDto()
                    {
                        Message = msg,
                        Msisdn = CustomerEntity.Phone,
                        Token = _UserInfoToken.Token
                    };
                    await _SmsHelper.SendSms(smsRequest);
                }



            }
        }



    }

}
