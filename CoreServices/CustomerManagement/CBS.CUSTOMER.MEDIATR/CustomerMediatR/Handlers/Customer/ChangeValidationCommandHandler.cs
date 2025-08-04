
using MediatR;
using CBS.CUSTOMER.HELPER.Helper;
using AutoMapper;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DOMAIN.Context;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.DATA.Dto.PinValidation;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR;

namespace CBS.CUSTOMER.MEDIATR
{
    /// <summary>
    /// Handles the command to validate a PIN for a Customer.
    /// This ChangeValidationCommandHandler handles the command to validate a PIN for a Customer. 
    /// It interacts with the Customer repository, performs PIN validation, and updates the Customer entity in the database. 
    /// The class includes necessary comments, logging, and error handling for clarity and robustness.
    /// </summary>
    public class ChangeValidationCommandHandler : IRequestHandler<ChangePinCommand, ServiceResponse<ChangePinResponse>>
    {
        private readonly ICustomerRepository _customerRepository; // Repository for accessing Customer data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<ChangeValidationCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow; // Unit of work for database operations.

        /// <summary>
        /// Constructor for initializing the ChangeValidationCommandHandler.
        /// </summary>
        /// <param name="customerRepository">Repository for Customer data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="uow">Unit of work for database operations.</param>
        public ChangeValidationCommandHandler(
            ICustomerRepository customerRepository,
            IMapper mapper,
            ILogger<ChangeValidationCommandHandler> logger,
            IUnitOfWork<POSContext> uow
        )
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the ChangeValidationCommand to validate a PIN for a Customer.
        /// </summary>
        /// <param name="request">The ChangeValidationCommand containing validation data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<ChangePinResponse>> Handle(ChangePinCommand request, CancellationToken cancellationToken)
        {
            try
            {
                ChangePinResponse changeValidation = new ChangePinResponse();
                request.Telephone = "237" + request.Telephone;
                // Get the Customer entity from the repository based on the provided phone number
                var entity = _customerRepository.FindBy(e => e.Phone == request.Telephone).FirstOrDefault();

                if (entity != null)
                {
                    if (entity.MobileOrOnLineBankingLoginFailedAttempts <= 3)
                    {
                        // Validate the PIN against the saved hash
                        var validateToken = ValidateToken(request.Pin, entity.Pin);

                        if (validateToken)
                        {
                            var customer = _mapper.Map<GetCustomer>(entity);
                            // Successful PIN validation

                            entity.Pin = BCrypt.Net.BCrypt.HashPassword(request.NewPin);
                            entity.NumberOfAttemptsOfMobileOrOnLineBankingLogin = 0;
                            entity.MobileOrOnLineBankingLoginFailedAttempts = 0;
                            entity.MobileOrOnLineBankingLoginState = entity.NumberOfAttemptsOfMobileOrOnLineBankingLogin == 0 ? "FIRST_TRY" : "SUCCESSFUL_TRY";
                            changeValidation = new ChangePinResponse()
                            {
                                LoginStatus = entity.MobileOrOnLineBankingLoginState,
                                ValidationStatus = true,
                                NumberOfFailedTries = entity.MobileOrOnLineBankingLoginFailedAttempts,
                                Telephone = request.Telephone,
                                Customer = customer,
                                Token = null
                            };
                        }
                        else
                        {
                            entity.MobileOrOnLineBankingLoginFailedAttempts = entity.MobileOrOnLineBankingLoginFailedAttempts + 1;
                            entity.NumberOfAttemptsOfMobileOrOnLineBankingLogin = entity.NumberOfAttemptsOfMobileOrOnLineBankingLogin + 1;
                            entity.MobileOrOnLineBankingLoginState = entity.NumberOfAttemptsOfMobileOrOnLineBankingLogin == 0 ? "FIRST_TRY" : "MORE_THAN_ONE_TRY";
                            // Failed PIN validation
                            changeValidation = new ChangePinResponse()
                            {
                                LoginStatus = entity.MobileOrOnLineBankingLoginState,
                                ValidationStatus = false,
                                NumberOfFailedTries = entity.MobileOrOnLineBankingLoginFailedAttempts,
                                Telephone = request.Telephone,
                                Customer = null,
                                Token = null
                            };
                        }

                        // Update the Customer entity in the repository
                        _customerRepository.Update(entity);

                        // Save changes to the database
                        if (await _uow.SaveAsync() <= 0)
                        {
                            return ServiceResponse<ChangePinResponse>.Return500();
                        }
                    }
                    else
                    {
                        // Failed attempts exceeded thrice
                        return ServiceResponse<ChangePinResponse>.Return500("Failed Attempts More than Thrice");
                    }
                }
                else
                {
                    // Customer not found
                    changeValidation = new ChangePinResponse()
                    {
                        LoginStatus = "",
                        ValidationStatus = false,
                        NumberOfFailedTries = 0,
                        Telephone = request.Telephone,
                    };
                }

                // Return the result with a 200 OK response
                return ServiceResponse<ChangePinResponse>.ReturnResultWith200(changeValidation);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while validating PIN: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<ChangePinResponse>.Return500(e);
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
                return BCrypt.Net.BCrypt.Verify(pincode, savedHashToken);
            }
            catch (Exception ex)
            {
                // Log and rethrow any exceptions during validation
                throw ex;
            }
        }
    }

}
