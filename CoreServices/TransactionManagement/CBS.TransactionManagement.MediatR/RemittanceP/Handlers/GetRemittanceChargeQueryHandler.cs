using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.RemittanceP.Queries;
using CBS.TransactionManagement.Data.Dto.RemittanceP;

namespace CBS.TransactionManagement.RemittanceP.Handlers
{
    /// <summary>
    /// Handles the logic to process a remittance charge query by validating the request, 
    /// retrieving necessary account and transfer parameters, calculating the charges, and returning the result.
    /// </summary>
    public class GetRemittanceChargeQueryHandler : IRequestHandler<GetRemittanceChargeQuery, ServiceResponse<RemittanceChargeDto>>
    {
        private readonly ILogger<GetRemittanceChargeQueryHandler> _logger; // Logger for capturing logs.
        private readonly IAccountRepository _accountRepository; // Repository to interact with account data.

        /// <summary>
        /// Initializes a new instance of the GetRemittanceChargeQueryHandler class.
        /// </summary>
        /// <param name="accountRepository">Repository to access account-related data.</param>
        /// <param name="logger">Logger to capture logs and errors.</param>
        public GetRemittanceChargeQueryHandler(
            IAccountRepository accountRepository,
            ILogger<GetRemittanceChargeQueryHandler> logger)
        {
            // Check for null dependencies and throw exceptions if required.
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles the incoming query to calculate remittance charges.
        /// </summary>
        /// <param name="request">The query containing remittance details.</param>
        /// <param name="cancellationToken">Token to handle cancellation of the request.</param>
        /// <returns>A ServiceResponse containing the calculated remittance charges.</returns>
        public async Task<ServiceResponse<RemittanceChargeDto>> Handle(GetRemittanceChargeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate the incoming request.
                if (request == null) throw new ArgumentNullException(nameof(request));

                // Fetch the account details based on the sender's account number.
                var sourceAccount = await _accountRepository.GetAccountByAccountNumber(request.SenderAccountNumber);

                // Check if the account exists.
                if (sourceAccount == null)
                {
                    // Log a warning and return a 404 response if the account is not found.
                    _logger.LogWarning("Account not found for Account Number: {SenderAccountNumber}", request.SenderAccountNumber);
                    return ServiceResponse<RemittanceChargeDto>.Return404($"Account with number {request.SenderAccountNumber} not found.");
                }

                // Retrieve the transfer parameters associated with the product of the account.
                var sourceTransferParameters = sourceAccount.Product?.TransferParameters?.FirstOrDefault();

                // Check if transfer parameters are configured for the account's product.
                if (sourceTransferParameters == null)
                {
                    // Log a warning and return a 404 response if transfer parameters are missing.
                    _logger.LogWarning("Transfer parameters not configured for Account Number: {SenderAccountNumber}", request.SenderAccountNumber);
                    return ServiceResponse<RemittanceChargeDto>.Return404("Transfer parameters not configured for the sender's account.");
                }

                // Parse the remittance type from the request and validate it.
                var remittanceTypes = GetRemittanceTypes(request.RemittanceType);

                // Calculate the remittance transfer charges based on the account and transfer details.
                var remittanceCharge = await _accountRepository.CalculateRemittanceTransferCharges(
                    request.Amount, // The amount being remitted.
                    sourceAccount.ProductId, // The product associated with the account.
                    remittanceTypes, // The parsed remittance type.
                    sourceAccount.BranchId,request.TransfterType); // The branch of the account.

                // Add the remittance amount to the total charges.
                remittanceCharge.TotalCharges += request.Amount;
                remittanceCharge.InitailAmount=request.Amount;
                remittanceCharge.ChargeType=ChargeType.Exclussive.ToString();
                if (request.ChargeType==ChargeType.Inclussive.ToString())
                {
                    remittanceCharge.InitailAmount=request.Amount;
                    remittanceCharge.Amount-=remittanceCharge.Charge;
                    remittanceCharge.TotalCharges = request.Amount;
                    remittanceCharge.ChargeType=ChargeType.Inclussive.ToString();
                }

                // Return a successful response with the calculated charges.
                return ServiceResponse<RemittanceChargeDto>.ReturnResultWith200(remittanceCharge);
            }
            catch (ArgumentException ex)
            {
                // Log a warning for validation errors and return a 400 response.
                _logger.LogWarning("Validation error: {Message}", ex.Message);
                return ServiceResponse<RemittanceChargeDto>.Return400(ex.Message);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Failed to calculate transfer charges for Account Number: {request.SenderAccountNumber}. Error: {ex.Message}";
                // Log unexpected errors and return a 500 response.
                _logger.LogError(errorMessage);
                return ServiceResponse<RemittanceChargeDto>.Return500(errorMessage);
            }
        }

        /// <summary>
        /// Parses and validates the remittance type from the request.
        /// </summary>
        /// <param name="remittanceTypes">The remittance type as a string.</param>
        /// <returns>A valid RemittanceTypes enum value.</returns>
        /// <exception cref="ArgumentException">Thrown if the remittance type is invalid.</exception>
        public RemittanceTypes GetRemittanceTypes(string remittanceTypes)
        {
            // Check if the remittance type is null or empty.
            if (string.IsNullOrEmpty(remittanceTypes))
            {
                // Log a warning and throw an exception for invalid input.
                _logger.LogWarning("Invalid remittance type: null or empty.");
                throw new ArgumentException("The fee operation type of remittance cannot be null or empty.");
            }

            // Attempt to parse the remittance type string into the RemittanceTypes enum.
            if (Enum.TryParse(remittanceTypes, true, out RemittanceTypes result))
            {
                return result; // Return the parsed value if successful.
            }

            // Log a warning and throw an exception for invalid remittance types.
            _logger.LogWarning("Invalid Remittance Type: {RemittanceTypes}", remittanceTypes);
            throw new ArgumentException($"Invalid Remittance Type: {remittanceTypes}");
        }
    }
}
