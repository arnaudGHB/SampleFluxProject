
using MediatR;
using CBS.CUSTOMER.HELPER.Helper;
using AutoMapper;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DOMAIN.Context;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.REPOSITORY;

using CBS.CUSTOMER.DATA.Dto.PinValidation;

using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.CUSTOMER.DATA.Dto.PinValidation;
using BusinessServiceLayer.Objects.SmsObject;
using Newtonsoft.Json;
using System.Text;
using CBS.CUSTOMER.DATA.Entity;


using CBS.APICaller.Helper;


using System.Data;
using Microsoft.Extensions.Configuration.UserSecrets;

using CBS.CUSTOMER.MEDIATR.CustomerMediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

namespace CBS.CUSTOMER
{
    /// <summary>
    /// Handles the command to validate a PIN for a Customer.
    /// This PinValidationCommandHandler handles the command to validate a PIN for a Customer. 
    /// It interacts with the Customer repository, performs PIN validation, and updates the Customer entity in the database. 
    /// The class includes necessary comments, logging, and error handling for clarity and robustness.
    /// </summary>
    public class PinValidationCommandHandler : IRequestHandler<PinValidationCommand, ServiceResponse<PinValidationResponse>>
    {
        private readonly ICustomerRepository _customerRepository; // Repository for accessing Customer data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<PinValidationCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _UserInfoToken;
        //private readonly ApiCallerHelper _ApiCallerHelper;
        private readonly HELPER.Helper.PathHelper _PathHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDocumentBaseUrlRepository _DocumentBaseUrlRepository;

        /// <summary>
        /// Constructor for initializing the PinValidationCommandHandler.
        /// </summary>
        /// <param name="customerRepository">Repository for Customer data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="uow">Unit of work for database operations.</param>
        public PinValidationCommandHandler(
            ICustomerRepository customerRepository,
            IMapper mapper,
            ILogger<PinValidationCommandHandler> logger,
            IUnitOfWork<POSContext> uow,

            HELPER.Helper.PathHelper pathHelper,
            UserInfoToken userInfoToken,
            IHttpContextAccessor httpContextAccessor,
            IDocumentBaseUrlRepository documentBaseUrlRepository)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;

            _PathHelper = pathHelper;
            _UserInfoToken = userInfoToken;
            _httpContextAccessor = httpContextAccessor;
            _DocumentBaseUrlRepository = documentBaseUrlRepository;
        }

        /// <summary>
        /// Handles the PinValidationCommand to validate a PIN for a Customer.
        /// </summary>
        /// <param name="request">The PinValidationCommand containing validation data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<PinValidationResponse>> Handle(PinValidationCommand request, CancellationToken cancellationToken)
        {                    // 6. Access HttpContext from IHttpContextAccessor to store customer details in the session.

            // 1. Initialize the response message string to hold any log or audit messages.
            string message = string.Empty;
            var context = _httpContextAccessor.HttpContext;

            try
            {
                // 2. Format the phone number to include the required "237" prefix if not already present.
                request.Telephone = BaseUtilities.Add237Prefix(msisdn: request.Telephone);

                // 3. Create an empty PinValidationResponse object to hold the validation response data.
                PinValidationResponse pinValidation = new PinValidationResponse();

                // 4. Retrieve the Customer entity based on the provided phone number and ensure the customer is not deleted.
                var entity = await _customerRepository.FindBy(e => e.Phone == request.Telephone && e.IsDeleted == false).Include(x=>x.CustomerDocuments).FirstOrDefaultAsync();
               

                // 5. Check if the Customer entity was found in the database.
                if (entity != null)
                {
                    
                    // 8. Store customer's full name and phone in the session.
                    context.Session.SetString("FullName", $"[{entity.FirstName} {entity.LastName}] [{entity.Phone}]");
                    //// 7. Ensure context and session are available.
                    //if (context != null && context.Session != null)
                    //{
                    //    // 8. Store customer's full name and phone in the session.
                    //    context.Session.SetString("FullName", $"[{entity.FirstName} {entity.LastName}] [{entity.Phone}]");
                    //}

                    // 9. Verify that the customer’s failed login attempts are less than or equal to 3.
                    if (entity.MobileOrOnLineBankingLoginFailedAttempts <= 3)
                    {
                        // 10. Check if the provided PIN matches the saved hashed PIN in the database.
                        var validateToken = ValidateToken(request.Pin, entity.Pin);

                        // 11. If the PIN validation is successful:
                        if (validateToken)
                        {
                            // 12. Increment the successful login attempt counter.
                            entity.NumberOfAttemptsOfMobileOrOnLineBankingLogin++;
                            // 13. Update the customer's login state to indicate success.
                            entity.MobileOrOnLineBankingLoginState = "OK";
                            // 14. Reset the failed login attempts counter to zero since validation was successful.
                            entity.MobileOrOnLineBankingLoginFailedAttempts = 0;

                            // 15. Update the login state based on the number of login attempts (initial or multiple).
                            entity.MobileOrOnLineBankingLoginState = entity.NumberOfAttemptsOfMobileOrOnLineBankingLogin == 0 ? "FIRST_TRY" : "MORETHAN_ONE_TRY";

                            // 16. Map the Customer entity to a response model, CustomerDto, for further use.
                            var customer = _mapper.Map<CustomerDto>(entity);

                            // 17. Populate the successful PinValidationResponse object with relevant details.
                            pinValidation = MapGetCustomerToData(customer);

                            // 18. Update the Customer entity in the repository to reflect any changes made during the validation process.
                            _customerRepository.Update(entity);

                            // 19. Commit all changes to the database, saving the updated entity.
                            await _uow.SaveAsync();

                            var documentBaseUrl = "";
                            var baseDocumentUrl = _DocumentBaseUrlRepository.Find("0");

                            if (baseDocumentUrl != null)
                            {
                                documentBaseUrl = baseDocumentUrl.baseURL;
                            }

                            if (!entity.PhotoUrl.IsNullOrEmpty())
                            {
                                pinValidation.PhotoUrl = $"{documentBaseUrl}/{entity.PhotoUrl}";
                            }

                            // 20. Prepare a success message for logging.
                            message = $"PIN validation successful for customer with MSISDN: {request.Telephone}. Customer login status updated to '{entity.MobileOrOnLineBankingLoginState}' for this session.";
                            await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.OK, LogAction.MobileUserLogin, LogLevelInfo.Information);

                            // 21. Return a 200 OK response with the populated PinValidationResponse object.
                            return ServiceResponse<PinValidationResponse>.ReturnResultWith200(pinValidation, message);
                        }
                        else
                        {
                            // 22. Increment the failed login attempts counter if validation fails.
                            entity.MobileOrOnLineBankingLoginFailedAttempts++;

                            // 23. Increment the total login attempts counter, regardless of failure.
                            entity.NumberOfAttemptsOfMobileOrOnLineBankingLogin++;

                            // 24. Update the login state based on the number of attempts.
                            entity.MobileOrOnLineBankingLoginState = entity.NumberOfAttemptsOfMobileOrOnLineBankingLogin == 0 ? "FIRST_TRY" : "MORETHAN_ONE_TRY";

                            // 25. Populate the failed PinValidationResponse object with relevant details.
                            var customer = _mapper.Map<CustomerDto>(entity);
                            pinValidation = MapGetCustomerToData(customer);

                            // 26. Update the Customer entity in the repository to reflect any changes made during the validation process.
                            _customerRepository.Update(entity);

                            // 27. Commit all changes to the database, saving the updated entity.
                            await _uow.SaveAsync();

                            // 28. Prepare a failure message for logging.
                            message = $"PIN validation failed for customer with MSISDN: {request.Telephone}. Failed attempt count is now {entity.MobileOrOnLineBankingLoginFailedAttempts}.";
                            await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Unauthorized, LogAction.MobileUserLogin, LogLevelInfo.Warning);

                            // 29. Return a 401 Unauthorized response with the populated PinValidationResponse object.
                            return ServiceResponse<PinValidationResponse>.Return401(pinValidation, message);
                        }
                    }
                    else
                    {
                        // 30. If failed attempts exceed the allowed threshold (3), set the user’s state to "BLOCKED".
                        entity.MobileOrOnLineBankingLoginState = "BLOCKED";

                        // 31. Populate the PinValidationResponse object to indicate the account is blocked.
                        var customer = _mapper.Map<CustomerDto>(entity);
                        pinValidation = MapGetCustomerToData(customer);
                        pinValidation.IsBlocked = true;

                        // 32. Update the Customer entity in the repository to reflect any changes made during the validation process.
                        _customerRepository.Update(entity);

                        // 33. Commit all changes to the database, saving the updated entity.
                        await _uow.SaveAsync();

                        // 34. Prepare a message indicating the account has been blocked due to too many failed attempts.
                        message = $"Mobile access blocked for customer with MSISDN: {request.Telephone} after exceeding the maximum allowed login attempts of 3.";
                        await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Forbidden, LogAction.MobileUserLogin, LogLevelInfo.Warning);

                        // 35. Return a 403 Forbidden response with the populated PinValidationResponse object.
                        return ServiceResponse<PinValidationResponse>.Return403(pinValidation, message);
                    }
                }
                else
                {

                    // 36. If the Customer entity was not found, prepare a message indicating this.
                    message = $"Customer with MSISDN: {request.Telephone} not found in the system.";
                    context.Session.SetString("FullName", $"[AllowAnonymous] [{request.Telephone}]");
                    // 37. Log the message for auditing, and return a 404 response indicating the customer was not found.
                    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.NotFound, LogAction.MobileUserLogin, LogLevelInfo.Warning);
                    return ServiceResponse<PinValidationResponse>.Return404(message);
                }
            }
            catch (Exception e)
            {
                // 38. Prepare an error message capturing the exception details.
                var errorMessage = $"An error occurred during PIN validation for customer with MSISDN: {request.Telephone}. Error details: {e.Message}";

                // 39. Log the error message for debugging purposes.
                _logger.LogError(errorMessage);

                // 40. Log the error message for auditing and return a 500 Internal Server Error response.
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.MobileUserLogin, LogLevelInfo.Error);

                // 41. Return a 500 response with the error message.
                return ServiceResponse<PinValidationResponse>.Return500(errorMessage);
            }
        }

        public PinValidationResponse MapGetCustomerToData(CustomerDto getCustomer)
        {
            return new PinValidationResponse
            {
                CustomerId = getCustomer.CustomerId,
                Name = $"{getCustomer.FirstName} {getCustomer.LastName}".Trim(),
                DateOfBirth = getCustomer.DateOfBirth.ToString(),
                PlaceOfBirth = getCustomer.PlaceOfBirth,
                Occupation = getCustomer.Occupation,
                Address = getCustomer.Address,
                IdNumber = getCustomer.IDNumber,
                IdNumberIssueDate = getCustomer.IDNumberIssueDate,
                IdNumberIssueAt = getCustomer.IDNumberIssueAt,
                MembershipApprovalStatus = getCustomer.MembershipApprovalStatus,
                Gender = getCustomer.Gender,
                Email = getCustomer.Email,
                CustomerCode = getCustomer.CustomerCode,
                BankName = getCustomer.BankName,
                PhotoUrl = getCustomer.PhotoUrl,
                IsUseOnLineMobileBanking = getCustomer.IsUseOnLineMobileBanking,
                Language = getCustomer.Language,
                Active = getCustomer.Active,
                Telephone = getCustomer.Phone,
                IsBlocked = false  // Assuming a default value as this field does not map directly
            };
        }


        /// <summary>
        /// Validates a PIN against a hashed token.
        /// </summary>
        /// <param name="pincode">The PIN to be validated.</param>
        /// <param name="savedHashToken">The hashed token saved in the database.</param>
        /// <returns>True if the PIN is valid, false otherwise.</returns>
        public bool ValidateToken(string pincode, string savedHashToken)
        {
            try
            {
                // Use BCrypt.NET to verify the PIN against the saved hash
                bool verified = BCrypt.Net.BCrypt.Verify(pincode, savedHashToken);
                return verified;
            }
            catch (Exception ex)
            {
                // Log and rethrow any exceptions during validation
                throw ex;
            }
        }


        public async Task<TokenUserInformationOnlineResponse> GetTokenForMobileAndWebPlatforms(DATA.Entity.Customer Customer,string Pin,string Channel)
        {

            var GetTokenRequest = new TokenUserInformationOnlineRequest()
            {
                Email = Customer.Email,
                BranchId = Customer.BranchId,
                Channel = Channel,
                IpAddress = null,
                Msisdn = Customer.Phone,
                Name = (Customer.FirstName + " " + Customer.LastName),
                Pin = Pin

            };

           // var GetTokenResponse= await APICallHelper.WithOutAuthenthicationFromOtherServer<TokenUserInformationOnlineResponse>(_PathHelper.OnlineAuthenthicationUrl, GetTokenRequest);
            var GetTokenResponse = await APICallHelper.AuthenthicationFromIdentityServer<TokenUserInformationOnlineResponse>(_PathHelper, _PathHelper.OnlineAuthenthicationUrl, GetTokenRequest,_UserInfoToken.Token);

            return GetTokenResponse;


        


        }
    
    }
}
