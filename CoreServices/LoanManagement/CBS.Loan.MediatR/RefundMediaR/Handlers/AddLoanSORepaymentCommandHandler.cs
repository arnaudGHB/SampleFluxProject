using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.BankP;
using CBS.NLoan.Data.Dto.CustomerP;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Dto.RefundP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Data.Entity.RefundP;
using CBS.NLoan.Data.Enums;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.BankP.Command;
using CBS.NLoan.MediatR.CustomerP.Query;
using CBS.NLoan.MediatR.RefundMediaR.Commands;
using CBS.NLoan.MediatR.SMS.Command;
using CBS.NLoan.Repository.LoanApplicationP;
using CBS.NLoan.Repository.LoanApplicationP.LoanAmortizationP;
using CBS.NLoan.Repository.LoanApplicationP.LoanCommentryP;
using CBS.NLoan.Repository.LoanApplicationP.LoanP;
using CBS.NLoan.Repository.LoanProductP;
using CBS.NLoan.Repository.RefundP;
using CBS.NLoan.Repository.TaxP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.RefundMediaR.Handlers
{
    /// <summary>
    /// Handles the command to add a new loan repayment and create a refund if applicable.
    /// </summary>
    public class AddLoanSORepaymentCommandHandler : IRequestHandler<AddLoanSORepaymentCommand, ServiceResponse<SavingAmtWithRefundDto>>
    {
        private readonly IRefundRepository _refundRepository; // Repository for accessing refund data.
        private readonly ILoanRepository _loanRepository; // Repository for accessing loan data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddLoanSORepaymentCommandHandler> _logger; // Logger for tracking actions and errors.
        private readonly IUnitOfWork<LoanContext> _uow; // Unit of Work for transaction management.
        private readonly IRefundDetailRepository _refundDetailRepository; // Repository for refund details.
        private readonly IMediator _mediator; // Mediator for sending internal requests.
        private readonly ITaxRepository _taxRepository; // Repository for tax-related operations.
        private readonly ILoanAmortizationRepository _loanAmortizationRepository; // Repository for loan amortization.
        private readonly ILoanProductRepaymentOrderRepository _loanProductRepaymentOrderRepository; // Repository for repayment order rules.
        private readonly ILoanProductRepository _loanProductRepository; // Repository for loan product details.
        private readonly ILoanApplicationRepository _loanApplicationRepository; // Repository for loan applications.

        /// <summary>
        /// Initializes a new instance of the <see cref="AddLoanSORepaymentCommandHandler"/> class.
        /// </summary>
        /// <param name="refundRepository">Repository for accessing refund data.</param>
        /// <param name="mapper">AutoMapper instance for object transformation.</param>
        /// <param name="logger">Logger instance for error and process tracking.</param>
        /// <param name="uow">Unit of Work for handling transactions.</param>
        /// <param name="loanRepository">Repository for accessing loan records.</param>
        /// <param name="mediator">Mediator instance for handling internal service calls.</param>
        /// <param name="loanAmortizationRepository">Repository for managing loan amortization schedules.</param>
        /// <param name="refundDetailRepository">Repository for refund detail management.</param>
        /// <param name="taxRepository">Repository for tax-related operations (optional).</param>
        /// <param name="loanProductRepaymentOrderRepository">Repository for repayment order configurations (optional).</param>
        /// <param name="loanProductRepository">Repository for loan product data (optional).</param>
        /// <param name="loanApplicationRepository">Repository for handling loan applications (optional).</param>
        public AddLoanSORepaymentCommandHandler(
            IRefundRepository refundRepository,
            IMapper mapper,
            ILogger<AddLoanSORepaymentCommandHandler> logger,
            IUnitOfWork<LoanContext> uow,
            ILoanRepository loanRepository,
            IMediator mediator,
            ILoanAmortizationRepository loanAmortizationRepository,
            IRefundDetailRepository refundDetailRepository,
            ITaxRepository taxRepository = null,
            ILoanProductRepaymentOrderRepository loanProductRepaymentOrderRepository = null,
            ILoanProductRepository loanProductRepository = null,
            ILoanApplicationRepository loanApplicationRepository = null)
        {
            _refundRepository = refundRepository ?? throw new ArgumentNullException(nameof(refundRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _loanRepository = loanRepository ?? throw new ArgumentNullException(nameof(loanRepository));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _loanAmortizationRepository = loanAmortizationRepository ?? throw new ArgumentNullException(nameof(loanAmortizationRepository));
            _refundDetailRepository = refundDetailRepository ?? throw new ArgumentNullException(nameof(refundDetailRepository));

            // Optional dependencies
            _taxRepository = taxRepository;
            _loanProductRepaymentOrderRepository = loanProductRepaymentOrderRepository;
            _loanProductRepository = loanProductRepository;
            _loanApplicationRepository = loanApplicationRepository;
        }

        /// <summary>
        /// Handles the AddLoanSORepaymentCommand to process a loan repayment.
        /// </summary>
        /// <param name="request">The AddLoanSORepaymentCommand containing repayment data.</param>
        /// <param name="cancellationToken">A cancellation token for operation cancellation.</param>
        /// <returns>A service response containing repayment details.</returns>
        public async Task<ServiceResponse<SavingAmtWithRefundDto>> Handle(AddLoanSORepaymentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Fetch the loan details based on the provided LoanId
                var loan = await _loanRepository.FindAsync(request.LoanId);
                if (loan == null)
                {
                    string message = $"Loan with ID {request.LoanId} not found. Amount={request.PrincipalAmount + request.ChargeAmount + request.Interest + request.TaxAmount}.";
                    _logger.LogError(message);
                    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.NotFound, LogAction.LoanSORepaymentAttempted, LogLevelInfo.Error);
                    return ServiceResponse<SavingAmtWithRefundDto>.Return404(message); ;
                }

                // Fetch the corresponding loan application details
                var loanApplication = await _loanApplicationRepository.FindAsync(loan.LoanApplicationId);
                if (loanApplication == null)
                {
                    string message = $"Loan application with ID {loan.LoanApplicationId} not found. Loan ID: {request.LoanId}, CustomerID: {loan.CustomerId}.";
                    _logger.LogError(message);
                    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.NotFound, LogAction.LoanSORepaymentAttempted, LogLevelInfo.Error);
                    return ServiceResponse<SavingAmtWithRefundDto>.Return404(message);
                }

                // Fetch the loan product details associated with the application
                var loanProduct = await _loanProductRepository.FindAsync(loanApplication.LoanProductId);
                if (loanProduct == null)
                {
                    string message = $"Loan product with ID {loanApplication.LoanProductId} not found. Loan Application ID: {loan.LoanApplicationId}.";
                    _logger.LogError(message);
                    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.NotFound, LogAction.LoanSORepaymentAttempted, LogLevelInfo.Warning);
                    return ServiceResponse<SavingAmtWithRefundDto>.Return404(message);
                }

                // Fetch customer details through mediator call
                var customerPICallCommandResult = await _mediator.Send(new GetCustomerCallCommand { CustomerId = loanApplication.CustomerId }, cancellationToken);
                if (customerPICallCommandResult.StatusCode != 200)
                {
                    string message = $"Failed to retrieve customer information for Customer ID {loanApplication.CustomerId}. Response Status: {customerPICallCommandResult.StatusCode}.";
                    _logger.LogError(message);
                    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Forbidden, LogAction.LoanSORepaymentAttempted, LogLevelInfo.Warning);
                    return ServiceResponse<SavingAmtWithRefundDto>.Return403(message);
                }

                var customer = customerPICallCommandResult.Data;

                // Fetch branch details using the customer's branch ID
                var branchPICallCommandResult = await _mediator.Send(new BranchPICallCommand { BranchId = customer.branchId }, cancellationToken);
                if (branchPICallCommandResult.StatusCode != 200)
                {
                    string message = $"Failed to retrieve branch details for Branch ID {customer.branchId}. Response Status: {branchPICallCommandResult.StatusCode}.";
                    _logger.LogError(message);
                    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Forbidden, LogAction.LoanSORepaymentAttempted, LogLevelInfo.Warning);
                    return ServiceResponse<SavingAmtWithRefundDto>.Return403(message);
                }
                var branch = branchPICallCommandResult.Data;

                // Initialize variables to track repayment components
                decimal interestPaid = 0m, taxPaid = 0m, principalPaid = 0m, chargesPaid = 0m;

                // Process and allocate repayment amounts based on different categories
                interestPaid = ProcessInterestRepayment(loan, request.Interest);
                principalPaid = ProcessPrincipalRepayment(loan, request.PrincipalAmount);
                chargesPaid = ProcessChargesRepayment(loan, request.ChargeAmount);
                taxPaid = ProcessTaxRepayment(loan, request.TaxAmount);

                // Calculate the total expected repayment amount
                decimal expectedAmountToPay = request.PrincipalAmount + request.ChargeAmount + request.Interest + request.TaxAmount;

                // Calculate the total amount actually paid
                decimal totalAmountPaid = interestPaid + principalPaid + chargesPaid + taxPaid;

                /* 
                The following commented-out section suggests an alternative approach where repayments are processed
                according to a specific repayment order (e.g., interest first, then principal, etc.). If needed in the future,
                uncomment and implement the logic accordingly.

                foreach (var order in GetRepaymentOrder(repaymentOrder))
                {
                    switch (order)
                    {
                        case "interest":
                            (interestPaid, remainingAmount) = ProcessInterestRepayment(loan, interestAmount, remainingAmount);
                            break;
                        case "principal":
                            principalPaid = ProcessPrincipalRepayment(loan, remainingAmount);
                            remainingAmount -= principalPaid;
                            break;
                        case "charges":
                            chargesPaid = ProcessChargesRepayment(loan, chargesAmount, remainingAmount);
                            remainingAmount -= chargesPaid;
                            break;
                    }

                    if (remainingAmount <= 0) break;
                }
                */

                // Create a new refund record to track this repayment transaction
                var refund = CreateRefund(request, loan, principalPaid, interestPaid, taxPaid, chargesPaid, totalAmountPaid);
                _refundRepository.Add(refund);

                // Update the loan details based on the payments made
                loan = UpdateLoanDetails(loan, totalAmountPaid, interestPaid, principalPaid, taxPaid, chargesPaid);

                // Update amortization schedule and create refund details for each component
                var (amortizations, refundDetails) = UpdateAmortizationAndRefundDetails(loan, interestPaid, principalPaid, taxPaid, chargesPaid, totalAmountPaid, refund);
                _loanAmortizationRepository.UpdateRange(amortizations);
                _refundDetailRepository.AddRange(refundDetails);

                // Persist changes to the database
                await _uow.SaveAsync();

                // Optionally send an SMS confirmation for the loan repayment
                // await LoanRefundConfirmationSMS(loan, branch, customer, request.Amount, refund);

                // Map refund entity to DTO and prepare the response object
                var refundDto = _mapper.Map<RefundDto>(refund);
                refundDto.LoanProduct = loanProduct;

                // Prepare the response containing refund details and the remaining savings amount
                SavingAmtWithRefundDto savingAmtWithRefund = new()
                {
                    SavingAmt = new() { Amount = expectedAmountToPay - totalAmountPaid },
                    Refund = refundDto,
                };
                string successMessage = $"Loan repayment of {BaseUtilities.FormatCurrency(totalAmountPaid)} was successful. Remaining savings amount: {BaseUtilities.FormatCurrency(expectedAmountToPay - totalAmountPaid)}.";
                _logger.LogInformation(successMessage);
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.LoanSORepaymentCompleted, LogLevelInfo.Information);



                return ServiceResponse<SavingAmtWithRefundDto>.ReturnResultWith200(savingAmtWithRefund,successMessage);
            }
            catch (Exception e)
            {
                // Log any exceptions that occur during the repayment process
                string errorMessage = $"Error occurred while processing loan repayment. Error details: {e.Message}. Stack trace: {e.StackTrace}.";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.LoanSORepaymentFailed, LogLevelInfo.Error);

                return ServiceResponse<SavingAmtWithRefundDto>.Return500(e);
            }
        }
        private decimal ProcessInterestRepayment(Loan loan, decimal interestAmount)
        {
            var interestPaid = ProcessRepayment(loan.AccrualInterest, interestAmount);
            loan.AccrualInterest -= interestPaid;
            return interestPaid;
        }

        private decimal ProcessPrincipalRepayment(Loan loan, decimal principalAmount)
        {
            var principalPaid = ProcessRepayment(loan.Balance, principalAmount);
            loan.Balance -= principalPaid;
            loan.TotalPrincipalPaid += principalPaid;
            return principalPaid;
        }

        private decimal ProcessChargesRepayment(Loan loan, decimal chargeAmount)
        {
            var chargesPaid = ProcessRepayment(loan.Fee, chargeAmount);
            loan.Fee -= chargesPaid;
            return chargesPaid;
        }

        private decimal ProcessTaxRepayment(Loan loan, decimal taxAmount)
        {
            var taxPaid = ProcessRepayment(loan.Tax, taxAmount);
            loan.Tax -= taxPaid;
            return taxPaid;
        }

        /// <summary>
        /// Processes a generic repayment by calculating how much can be paid.
        /// </summary>
        /// <param name="loanComponent">The current loan component value.</param>
        /// <param name="amountToPay">The amount being paid.</param>
        /// <returns>The actual amount paid.</returns>
        private decimal ProcessRepayment(decimal loanComponent, decimal amountToPay)
        {
            return (amountToPay > 0 && loanComponent > 0) ? Math.Min(amountToPay, loanComponent) : 0m;
        }

        private Refund CreateRefund(AddLoanSORepaymentCommand request, Loan loan, decimal principalPaid, decimal interestPaid, decimal taxPaid, decimal chargesPaid,decimal totalAmountPaid)
        {
            return new Refund
            {
                Id = BaseUtilities.GenerateUniqueNumber(),
                LoanId = request.LoanId,
                CustomerId = loan.CustomerId,
                Amount = totalAmountPaid,
                Principal = principalPaid,
                Interest = interestPaid,
                Tax = taxPaid,
                Penalty = chargesPaid,
                BranchId = loan.BranchId,
                BankId = loan.BankId,
                DateOfPayment = BaseUtilities.UtcNowToDoualaTime(),
                PaymentMethod = request.PaymentMethod,
                PaymentChannel = request.PaymentChannel,
                Balance = loan.Balance,
                Comment = "Loan Repayment"
            };
        }

        private Loan UpdateLoanDetails(Loan loan, decimal amountPaid, decimal interestPaid, decimal principalPaid, decimal taxPaid, decimal chargesPaid)
        {
            loan.LastRefundDate = BaseUtilities.UtcNowToDoualaTime();
            loan.LastPayment = amountPaid;
            loan.Paid += amountPaid;
            loan.AccrualInterestPaid += interestPaid;
            loan.TotalPrincipalPaid += principalPaid;
            loan.TaxPaid += taxPaid;
            loan.DueAmount -= amountPaid;
            loan.PenaltyPaid += chargesPaid;
            if (loan.DueAmount <= 0)
                loan.LoanStatus = LoanStatus.Closed.ToString();
            _loanRepository.Update(loan);
            return loan;
        }

        private (List<LoanAmortization> amortizations, List<RefundDetail> refundDetails) UpdateAmortizationAndRefundDetails(Loan loan, decimal interestPaid, decimal principalPaid, decimal taxPaid, decimal chargesPaid, decimal amountPaid, Refund refund)
        {
            var amortizations = new List<LoanAmortization>();
            var refundDetails = new List<RefundDetail>();

            var loanAmortizations = _loanAmortizationRepository.FindBy(x => x.LoanId == loan.Id).OrderBy(a => a.NextPaymentDate).ToList();

            foreach (var amortization in loanAmortizations)
            {
                var totalPaid = 0m;
                var totalDue = amortization.TotalDue;

                if (interestPaid > 0)
                {
                    var interestToPay = Math.Min(interestPaid, amortization.Interest - amortization.InterestPaid);
                    amortization.InterestPaid += interestToPay;
                    amortization.Balance -= interestToPay;
                    interestPaid -= interestToPay;
                    totalPaid += interestToPay;
                    totalDue -= totalPaid;
                }

                if (principalPaid > 0)
                {
                    var principalToPay = Math.Min(principalPaid, amortization.Principal - amortization.PrincipalPaid);
                    amortization.PrincipalPaid += principalToPay;
                    amortization.Balance -= principalToPay;
                    principalPaid -= principalToPay;
                    totalPaid += principalToPay;
                    totalDue -= totalPaid;
                }

                if (amortization.InterestPaid > 0 && loan.Tax > 0)
                {
                    var taxToPay = Math.Min(taxPaid, amortization.InterestPaid * 0.1925m);
                    amortization.TaxPaid += taxToPay;
                    loan.Tax -= taxToPay;
                    taxPaid -= taxToPay;
                    totalPaid += taxToPay;
                    totalDue -= totalPaid;
                }

                amortization.Due = totalDue;
                amortization.Paid += totalPaid;
                amortization.TotalDue -= totalPaid;

                if (amortization.TotalDue <= 0)
                {
                    amortization.Status = "Completed";
                    amortization.IsCompleted = true;
                }

                refundDetails.Add(new RefundDetail
                {
                    RefundId = refund.Id,
                    PrincipalAmount = principalPaid,
                    Interest = interestPaid,
                    TaxAmount = taxPaid,
                    PenaltyAmount = chargesPaid,
                    Balance = amortization.Balance,
                    BankId = amortization.BankId,
                    BranchId = amortization.BranchId,
                    CollectedAmount = amountPaid,
                    InterestBalance = amortization.Interest - amortization.InterestPaid,
                    LoanAmortizationId = amortization.Id,
                    Id = BaseUtilities.GenerateUniqueNumber(),
                    PenaltyAmountBalance = amortization.Penalty - amortization.PenaltyPaid,
                    PrincipalBalance = amortization.Principal - amortization.PrincipalPaid,
                    TaxAmountBalance = amortization.Tax - amortization.TaxPaid,
                });

                amortizations.Add(amortization);

                if (principalPaid <= 0 && interestPaid <= 0)
                    break;
            }

            return (amortizations, refundDetails);
        }

        private IEnumerable<string> GetRepaymentOrder(LoanProductRepaymentOrder repaymentOrder)
        {
            var orderList = new List<KeyValuePair<string, int>>
                {
                    new KeyValuePair<string, int>("interest", repaymentOrder.InterestOrder),
                    new KeyValuePair<string, int>("principal", repaymentOrder.CapitalOrder)
                };

            return orderList.OrderBy(x => x.Value).Select(x => x.Key);
        }

        private string GenerateLoanRefundConfirmationMessage(Loan loan, BranchDto branch, CustomerDto customer, decimal amountPaid, decimal capital, decimal interest)
        {
            string bankName = branch.name;
            string msg;

            if (customer.language.ToLower() == "english")
            {
                msg = $"Hello {customer.firstName} {customer.lastName}, ";
                msg += $"A refund of {BaseUtilities.FormatCurrency(amountPaid)} has been made on your loan with {bankName}.";

                //if (loan.Balance <= 0)
                //{
                //    msg += $"Your loan with {bankName} has been fully refunded.";
                //}
                //else
                //{
                //    msg += $"A partial refund of {BaseUtilities.FormatCurrency(amountPaid)} has been made on your loan with {bankName}. Principal amount refunded: {BaseUtilities.FormatCurrency(capital)} & Interest amount refunded: {BaseUtilities.FormatCurrency(interest)}";
                //    //msg += $"Your new balance is {BaseUtilities.FormatCurrency(loan.Balance)}. ";
                //}

                msg += $"Thank you for choosing {branch.name}.";

                //if (loan.Balance <= 0)
                //{
                //    msg += $" For more information, contact customer service {branch.customerServiceContact}.";
                //}
            }
            else // Assuming if not English, it's French
            {
                msg = $"Bonjour {customer.firstName} {customer.lastName}, ";
                msg += $"Un remboursement de {BaseUtilities.FormatCurrency(amountPaid)} a été effectué sur votre prêt avec {bankName}.";

                //if (loan.Balance <= 0)
                //{
                //    msg += $"Votre prêt avec {bankName} a été entièrement remboursé. ";
                //}
                //else
                //{
                //    msg += $"Un remboursement partiel de {BaseUtilities.FormatCurrency(amountPaid)} a été effectué sur votre prêt avec {bankName}.  Capital paid: {BaseUtilities.FormatCurrency(capital)} & Interest paid: {BaseUtilities.FormatCurrency(interest)}";
                //    //msg += $"Votre nouveau solde est de {BaseUtilities.FormatCurrency(loan.Balance)}. ";
                //}

                msg += $"Merci d'avoir choisi {branch.name}.";

                //if (loan.Balance <= 0)
                //{
                //    msg += $" Pour plus d'informations, contactez le service clientèle {branch.customerServiceContact}.";
                //}
            }

            return msg;
        }


        private async Task LoanRefundConfirmationSMS(Loan loan, BranchDto branch, CustomerDto customer, decimal amountPaid, Refund refund)
        {



            string msg = GenerateLoanRefundConfirmationMessage(loan, branch, customer, amountPaid, refund.Principal, refund.Interest);

            var sMSPICallCommand = new SendSMSPICallCommand
            {
                messageBody = msg,
                recipient = customer.phone
                //recipient = "237650535634"
            };
            // Send command to mediator
            await _mediator.Send(sMSPICallCommand);

            var sMSPICallCommandx = new SendSMSPICallCommand
            {
                messageBody = msg,
                //recipient = customer.phone
                recipient = "237650535634"
            };
            // Send command to mediator
            await _mediator.Send(sMSPICallCommandx);
        }



    }



}
