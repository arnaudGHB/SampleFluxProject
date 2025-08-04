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
    /// Handles the command to add a new Refund.
    /// </summary>
    public class AddLoanRepaymentCommandHandler : IRequestHandler<AddLoanRepaymentCommand, ServiceResponse<RefundDto>>
    {
        private readonly IRefundRepository _RefundRepository; // Repository for accessing Refund data.
        private readonly ILoanRepository _LoanRepository; // Repository for accessing Refund data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddLoanRepaymentCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<LoanContext> _uow;
        private readonly IRefundDetailRepository _RefundDetailRepository;
        private readonly IMediator _mediator;
        private readonly ITaxRepository _TaxRepository;
        private readonly ILoanAmortizationRepository _loanAmortizationRepository;
        private readonly ILoanProductRepaymentOrderRepository _loanProductRepaymentOrderRepository;
        private readonly ILoanProductRepository _loanProductRepository;
        private readonly ILoanApplicationRepository _loanApplicationRepository;

        /// <summary>
        /// Constructor for initializing the AddLoanRepaymentCommandHandler.
        /// </summary>
        /// <param name="RefundRepository">Repository for Refund data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddLoanRepaymentCommandHandler(
            IRefundRepository RefundRepository,
            IMapper mapper,
            ILogger<AddLoanRepaymentCommandHandler> logger,
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
            _RefundRepository = RefundRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _LoanRepository = loanRepository;
            _mediator = mediator;
            _loanAmortizationRepository = loanAmortizationRepository;
            _RefundDetailRepository = refundDetailRepository;
            _TaxRepository = taxRepository;
            _loanProductRepaymentOrderRepository = loanProductRepaymentOrderRepository;
            _loanProductRepository = loanProductRepository;
            _loanApplicationRepository = loanApplicationRepository;
        }

        /// <summary>
        /// Handles the AddLoanRepaymentCommand to add a new Refund.
        /// </summary>
        /// <param name="request">The AddLoanRepaymentCommand containing Refund data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>


        public async Task<ServiceResponse<RefundDto>> Handle(AddLoanRepaymentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Fetch the loan details
                var loan = await _LoanRepository.FindAsync(request.LoanId);
                if (loan == null)
                    return ServiceResponse<RefundDto>.Return404("Loan not found");

                // Fetch the loan application details
                var loanApplication = await _loanApplicationRepository.FindAsync(loan.LoanApplicationId);
                if (loanApplication == null)
                    return ServiceResponse<RefundDto>.Return404("Loan application not found");

                // Fetch the loan product details
                var loanProduct = await _loanProductRepository.FindAsync(loanApplication.LoanProductId);
                if (loanProduct == null)
                    return ServiceResponse<RefundDto>.Return404("Loan product not found");

                // Fetch or default repayment order details
                var repaymentOrder = await _loanProductRepaymentOrderRepository
                    .FindBy(x => x.LoanProductRepaymentOrderType==LoanProductRepaymentOrderType.NormalRefund.ToString())
                    .FirstOrDefaultAsync() ?? new LoanProductRepaymentOrder
                    {
                        InterestOrder = 1,
                        InterestRate = 25,
                        CapitalOrder = 2,
                        CapitalRate = 75,
                        FineOrder = 3,
                        FineRate = 0
                    };

                // Fetch customer details
                var customerPICallCommandResult = await _mediator.Send(new GetCustomerCallCommand { CustomerId = loanApplication.CustomerId }, cancellationToken);
                if (customerPICallCommandResult.StatusCode != 200)
                    return ServiceResponse<RefundDto>.Return403(customerPICallCommandResult.Message);

                var customer = customerPICallCommandResult.Data;

                // Fetch branch details
                var branchPICallCommandResult = await _mediator.Send(new BranchPICallCommand { BranchId = customer.branchId }, cancellationToken);
                if (branchPICallCommandResult.StatusCode != 200)
                    return ServiceResponse<RefundDto>.Return403("Failed getting member's branch.");

                var branch = branchPICallCommandResult.Data;

                // Initialize payment tracking variables
                var remainingAmount = request.Amount;
                var interestPaid = 0m;
                var taxPaid = 0m;
                var principalPaid = 0m;
                var chargesPaid = 0m;

                // Calculate amounts based on repayment order rates
                var interestAmount = request.Amount * repaymentOrder.InterestRate / 100;
                var principalAmount = request.Amount * repaymentOrder.CapitalRate / 100;
                var chargesAmount = request.Amount * repaymentOrder.FineRate / 100;

                // Process repayment based on repayment order
                foreach (var order in GetRepaymentOrder(repaymentOrder))
                {
                    switch (order)
                    {
                        case "interest":
                            (interestPaid, taxPaid, remainingAmount) = ProcessInterestRepayment(loan, interestAmount, remainingAmount);
                            break;
                        case "principal":
                            principalPaid = ProcessPrincipalRepayment(loan, principalAmount, remainingAmount);
                            remainingAmount -= principalPaid;
                            break;
                        case "charges":
                            chargesPaid = ProcessChargesRepayment(loan, chargesAmount, remainingAmount);
                            remainingAmount -= chargesPaid;
                            break;
                    }

                    if (remainingAmount <= 0) break;
                }

                // Create refund record
                var refund = CreateRefund(request, loan, principalPaid, interestPaid, taxPaid, chargesPaid);
                _RefundRepository.Add(refund);

                // Update loan status and repayment details
                loan = UpdateLoanDetails(loan, request.Amount, interestPaid, principalPaid, taxPaid, chargesPaid);

                // Update amortization schedule and create refund details
                var (amortizations, refundDetails) = UpdateAmortizationAndRefundDetails(loan, interestPaid, principalPaid, taxPaid, chargesPaid, request.Amount, refund);
                _loanAmortizationRepository.UpdateRange(amortizations);
                _RefundDetailRepository.AddRange(refundDetails);

                await _uow.SaveAsync();

                // Send repayment confirmation SMS
                //await LoanRefundConfirmationSMS(loan, branch, customer, request.Amount, refund);

                // Map refund to DTO and return success response
                var refundDto = _mapper.Map<RefundDto>(refund);
                refundDto.LoanProduct = loanProduct;
                return ServiceResponse<RefundDto>.ReturnResultWith200(refundDto, $"Repayment of {request.Amount} was successful.");
            }
            catch (Exception e)
            {
                _logger.LogError($"Error occurred while saving Refund: {e.Message}");
                return ServiceResponse<RefundDto>.Return500(e);
            }
        }

        private (decimal interestPaid, decimal taxPaid, decimal remainingAmount) ProcessInterestRepayment(Loan loan, decimal interestAmount, decimal remainingAmount)
        {
            var interestPaid = 0m;
            var taxPaid = 0m;

            if (remainingAmount > 0 && loan.AccrualInterest > 0)
            {
                var interestToPay = Math.Min(interestAmount, loan.AccrualInterest);
                loan.AccrualInterest -= interestToPay;
                interestPaid += interestToPay;
                remainingAmount -= interestToPay;

                var vat = interestPaid * 0.1925m;
                if (vat > 0 && loan.Tax > 0)
                {
                    var taxToPay = Math.Min(vat, loan.Tax);
                    loan.Tax -= taxToPay;
                    taxPaid += taxToPay;
                    remainingAmount -= taxToPay;
                }
            }

            return (interestPaid, taxPaid, remainingAmount);
        }

        private decimal ProcessPrincipalRepayment(Loan loan, decimal principalAmount, decimal remainingAmount)
        {
            if (remainingAmount > 0 && loan.Balance > 0)
            {
                if (remainingAmount> principalAmount)
                {
                    principalAmount = remainingAmount;
                }
                // Determine how much of the principal can be paid
                var principalToPay = Math.Min(principalAmount, loan.Balance);

                // Reduce the loan balance by the amount paid
                loan.Balance -= principalToPay;

                // Track the total principal paid
                loan.TotalPrincipalPaid += principalToPay;

                return principalToPay; // Return the amount of principal paid
            }

            return 0m; // Return 0 if no payment was made
        }

        private decimal ProcessChargesRepayment(Loan loan, decimal chargesAmount, decimal remainingAmount)
        {
            if (remainingAmount > 0 && loan.Fee > 0)
            {
                var feeToPay = Math.Min(chargesAmount, loan.Fee);
                loan.Fee -= feeToPay;

                return feeToPay;
            }

            return 0m;
        }

        private Refund CreateRefund(AddLoanRepaymentCommand request, Loan loan, decimal principalPaid, decimal interestPaid, decimal taxPaid, decimal chargesPaid)
        {
            return new Refund
            {
                Id = BaseUtilities.GenerateUniqueNumber(),
                LoanId = request.LoanId,
                CustomerId = loan.CustomerId,
                Amount = request.Amount,
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
            _LoanRepository.Update(loan);
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
                    new KeyValuePair<string, int>("principal", repaymentOrder.CapitalOrder),
                    new KeyValuePair<string, int>("charges", repaymentOrder.FineOrder)
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
