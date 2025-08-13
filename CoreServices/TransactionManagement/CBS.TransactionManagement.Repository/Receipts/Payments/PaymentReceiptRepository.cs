using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data.Entity.Receipts.Payments;
using CBS.TransactionManagement.Domain;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Repository.Receipts.Details;
using CBS.TransactionManagement.Data.Entity.Receipts.Details;
using CBS.TransactionManagement.Data.Dto.Receipts.Details;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Data.Dto.Receipts.Payments;
using DocumentFormat.OpenXml.Bibliography;
using CBS.TransactionManagement.Data;

namespace CBS.TransactionManagement.Repository.Receipts.Payments
{

    public class PaymentReceiptRepository : GenericRepository<PaymentReceipt, TransactionContext>, IPaymentReceiptRepository
    {
        private readonly ILogger<TellerRepository> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        private readonly IPaymentDetailRepository _paymentDetailRepository;
        public PaymentReceiptRepository(IUnitOfWork<TransactionContext> unitOfWork, ILogger<TellerRepository> logger, UserInfoToken userInfoToken, IPaymentDetailRepository paymentDetailRepository) : base(unitOfWork)
        {
            _logger = logger;
            _userInfoToken = userInfoToken;
            _paymentDetailRepository = paymentDetailRepository;
        }
        public PaymentReceipt ProcessPaymentAsync(PaymentProcessingRequest request)
        {
            string referenceId = "N/A"; // Default reference ID for logging

            try
            {
                // Step 1: Validate Input Request
                if (request == null)
                {
                    string nullRequestMessage = "[ERROR] Payment processing request is null.";
                    _logger.LogError(nullRequestMessage);
                    throw new ArgumentNullException(nameof(request), "Payment processing request cannot be null.");
                }

                var transaction = request.Transactions?.FirstOrDefault();
                if (transaction == null)
                {
                    string noTransactionMessage = "[ERROR] No transactions found in the payment processing request.";
                    _logger.LogError(noTransactionMessage);
                    throw new InvalidOperationException(noTransactionMessage);
                }

                // Step 2: Extract and Validate Key Payment Information
                referenceId = transaction.TransactionReference ?? "N/A";
                var notesRequest = request.NotesRequest ?? new CurrencyNotesRequest(); // Ensure non-null NotesRequest
                var paymentDetails = request.PaymentDetails; // Ensure non-null PaymentDetails list

                // Step 3: Construct Payment Receipt Object
                var paymentReceipt = new PaymentReceipt
                {
                    Id = transaction.TransactionReference ?? BaseUtilities.GenerateUniqueNumber(),
                    AccountingDay = request.AccountingDay,
                    Amount = request.Amount,
                    Charges = request.TotalCharges,
                    PortalUsed = OperationSourceType.Web_Portal.ToString(),
                    OperationTypeGrouping = request.OperationTypeGrouping,
                    OperationType = request.OperationType,
                    TillName = transaction.Teller?.Name ?? "Unknown Teller",
                    TotalAmount = request.TotalAmount,
                    AmountInWord = BaseUtilities.ConvertToWords(request.Amount),
                    BankId = transaction.BankId ?? "Unknown Bank",
                    DepositorCNI = transaction.DepositorIDNumber ?? "N/A",
                    DepositorName = transaction.DepositorName ?? "Unknown Depositor",
                    DepositorPhone = transaction.DepositerTelephone ?? "N/A",
                    BranchId = transaction.BranchId ?? "Unknown Branch",
                    CashierName = _userInfoToken?.FullName ?? "System",
                    TellerId = transaction.TellerId ?? "Unknown Teller ID",
                    ExternalReferenceNumber = transaction.ExternalReference ?? "N/A",
                    InternalReferenceNumber = transaction.TransactionReference ?? "N/A",
                    MemberName = request.MemberName ?? "Unknown Member",
                    MemberReference = transaction.CustomerId ?? "Unknown Member Ref",
                    Date = BaseUtilities.UtcNowToDoualaTime(),
                    ReceiptTitle = transaction.ReceiptTitle ?? "Payment Receipt",
                    ServiceType = request.ServiceType ?? "Unknown Service",
                    SourceOfRequest = request.SourceOfRequest ?? "Unknown Source",

                    // Coin & Note Breakdown
                    Coin1 = notesRequest.Coin1,
                    Coin10 = notesRequest.Coin10,
                    Coin100 = notesRequest.Coin100,
                    Coin25 = notesRequest.Coin25,
                    Coin5 = notesRequest.Coin5,
                    Coin50 = notesRequest.Coin50,
                    Coin500 = notesRequest.Coin500,
                    Note1000 = notesRequest.Note1000,
                    Note500 = notesRequest.Note500,
                    Note10000 = notesRequest.Note10000,
                    Note2000 = notesRequest.Note2000,
                    Note5000 = notesRequest.Note5000
                };

                // Step 4: Process Payment Details
                var details = new List<PaymentDetail>();
                foreach (var payment in paymentDetails)
                {
                    if (payment == null) continue; // Skip null entries

                    var detail = new PaymentDetail
                    {
                        AccountingDay = paymentReceipt.AccountingDay,
                        AccountNumber = payment.AccountNumber ?? "Unknown Account",
                        Amount = payment.Amount,
                        BankId = paymentReceipt.BankId,
                        BranchId = paymentReceipt.BranchId,
                        Date = paymentReceipt.Date,
                        Fee = payment.Fee,
                        Interest = payment.Interest,
                        LoanCapital = payment.LoanCapital,
                        MemberName = paymentReceipt.MemberName,
                        MemberReference = paymentReceipt.MemberReference,
                        Id = BaseUtilities.GenerateUniqueNumber(),
                        PaymentReceiptId = paymentReceipt.Id,
                        SericeName = payment.SericeOrEventName ?? "Unknown Service",
                        VAT = payment.VAT
                    };

                    details.Add(detail);
                }

                // Step 5: Persist Payment Data
                Add(paymentReceipt);
                _paymentDetailRepository.AddRange(details);
                paymentReceipt.PaymentDetails = details;

                // Step 6: Log and Audit Successful Payment Processing
                string successMessage = $"[SUCCESS] Payment processed successfully for Receipt ID: {paymentReceipt.Id}. Amount: {paymentReceipt.Amount:C}, Depositor: {paymentReceipt.DepositorName}, Member: {paymentReceipt.MemberName}.";
                _logger.LogInformation(successMessage);

                return paymentReceipt;
            }
            catch (Exception ex)
            {
                // Step 7: Log and Audit Error for Payment Processing
                string errorMessage = $"[ERROR] Failed to process payment for Reference ID: {referenceId}. Error: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                throw new ArgumentNullException(nameof(request), errorMessage);
            }
        }
    }
}
