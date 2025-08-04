
using MediatR;
using CBS.CUSTOMER.HELPER.Helper;
using AutoMapper;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DOMAIN.Context;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.DATA.Dto.PinValidation;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR;

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
        //private readonly ApiCallerHelper _ApiCallerHelper;
        private readonly HELPER.Helper.PathHelper _PathHelper;


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

            HELPER.Helper.PathHelper pathHelper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;

            _PathHelper = pathHelper;
        }

        /// <summary>
        /// Handles the PinValidationCommand to validate a PIN for a Customer.
        /// </summary>
        /// <param name="request">The PinValidationCommand containing validation data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<PinValidationResponse>> Handle(PinValidationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                request.Telephone = BaseUtilities.Add237Prefix(msisdn: request.Telephone);
      
                PinValidationResponse pinValidation = new PinValidationResponse();
                request.Telephone =BaseUtilities.Add237Prefix(msisdn: request.Telephone);
                // Get the Customer entity from the repository based on the provided phone number
                var entity = _customerRepository.FindBy(e => e.Phone == request.Telephone && e.IsDeleted==false).FirstOrDefault();

                if (entity != null)
                {
                    if (entity.MobileOrOnLineBankingLoginFailedAttempts <= 3)
                    {
                        // Validate the PIN against the saved hash
                        var validateToken = ValidateToken(request.Pin, entity.Pin);

                        if (validateToken)
                        {
                            var Customer= _mapper.Map<GetCustomer>(entity);
                            entity.NumberOfAttemptsOfMobileOrOnLineBankingLogin = entity.NumberOfAttemptsOfMobileOrOnLineBankingLogin + 1;
                            entity.MobileOrOnLineBankingLoginFailedAttempts = 0;
                            entity.MobileOrOnLineBankingLoginState = entity.NumberOfAttemptsOfMobileOrOnLineBankingLogin == 0 ? "FIRST_TRY" : "MORETHAN_ONE_TRY";
                            // Successful PIN validation

                            var OnLineToken =await GetTokenForMobileAndWebPlatforms(entity, request.Pin, request.Channel);
                            pinValidation = new PinValidationResponse()
                            {
                                LoginStatus = entity.MobileOrOnLineBankingLoginState,
                                ValidationStatus = true,
                                NumberOfFailedTries = entity.MobileOrOnLineBankingLoginFailedAttempts,
                                Telephone = request.Telephone,
                                Customer = Customer,
                                Token = OnLineToken.BearerToken,
                                RefreshToken=OnLineToken.RefreshToken,
                            };
                        }
                        else
                        {
                            entity.MobileOrOnLineBankingLoginFailedAttempts = entity.MobileOrOnLineBankingLoginFailedAttempts + 1;
                            entity.NumberOfAttemptsOfMobileOrOnLineBankingLogin = entity.NumberOfAttemptsOfMobileOrOnLineBankingLogin + 1;
                            entity.MobileOrOnLineBankingLoginState = entity.NumberOfAttemptsOfMobileOrOnLineBankingLogin == 0 ? "FIRST_TRY" : "MORETHAN_ONE_TRY";
                            // Failed PIN validation
                            pinValidation = new PinValidationResponse()
                            {
                                LoginStatus = entity.MobileOrOnLineBankingLoginState,
                                ValidationStatus = false,
                                NumberOfFailedTries = entity.MobileOrOnLineBankingLoginFailedAttempts,
                                Telephone = request.Telephone,
                                Customer = null
                            };
                        }

                        // Update the Customer entity in the repository
                       
                    }
                    else
                    {
                        entity.MobileOrOnLineBankingLoginState = "BLOCKED";
                        pinValidation = new PinValidationResponse()
                        {
                            LoginStatus = entity.MobileOrOnLineBankingLoginState,
                            ValidationStatus = false,
                            NumberOfFailedTries = entity.MobileOrOnLineBankingLoginFailedAttempts,
                            Telephone = request.Telephone,
                            Customer = null
                        };
                        // Failed attempts exceeded thrice
                    }

                    _customerRepository.Update(entity);

                    // Save changes to the database
                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<PinValidationResponse>.Return500();
                    }
                }
                else
                {
                    // Customer not found
                    /* pinValidation = new PinValidationResponse()
                     {
                         LoginStatus = "",
                         ValidationStatus = false,
                         NumberOfFailedTries = 0,
                         Telephone = request.Telephone,
                     };*/
                    return ServiceResponse<PinValidationResponse>.Return404("Record does not exist!!!");

                }

                // Return the result with a 200 OK response
                return ServiceResponse<PinValidationResponse>.ReturnResultWith200(pinValidation);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while validating PIN: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<PinValidationResponse>.Return500(e);
            }
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

            var GetTokenResponse= await APICallHelper.AuthenthicationFromIdentityServer<TokenUserInformationOnlineResponse>(_PathHelper,_PathHelper.OnlineAuthenthicationUrl, GetTokenRequest);

            return GetTokenResponse;


        


        }
    
    }
}
