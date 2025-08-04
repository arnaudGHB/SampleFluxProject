using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Data.Enums;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.Commands;
using CBS.NLoan.MediatR.LoanCalculatorHelper;
using CBS.NLoan.MediatR.LoanMediaR.Commands;
using CBS.NLoan.Repository.LoanApplicationP;
using CBS.NLoan.Repository.LoanApplicationP.LoanP;
using CBS.NLoan.Repository.LoanProductP;
using CBS.NLoan.Repository.TaxP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanMediaR.Handlers
{
    public class AddLoanHandler : IRequestHandler<AddLoanCommand, ServiceResponse<LoanDto>>
    {
        private readonly ILoanRepository _loanRepository;
        private readonly ILoanProductRepository _loanProductRepository;
        private readonly ILoanApplicationRepository _loanApplicationRepository;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger<AddLoanHandler> _logger;
        private readonly IUnitOfWork<LoanContext> _unitOfWork;
        private readonly ITaxRepository _TaxRepository;
        private readonly UserInfoToken _userInfoToken;
        /// <summary>
        /// Constructor for initializing the AddLoanCommandHandler.
        /// </summary>
        public AddLoanHandler(
            ILoanRepository loanRepository,
            ILoanProductRepository loanProductRepository,
            IMapper mapper,
            ILogger<AddLoanHandler> logger,
            IUnitOfWork<LoanContext> unitOfWork,
            ILoanApplicationRepository loanApplicationRepository = null,
            IMediator mediator = null,
            ITaxRepository taxRepository = null,
            UserInfoToken userInfoToken = null)
        {
            _loanRepository = loanRepository ?? throw new ArgumentNullException(nameof(loanRepository));
            _loanProductRepository = loanProductRepository ?? throw new ArgumentNullException(nameof(loanProductRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _loanApplicationRepository = loanApplicationRepository;
            _mediator = mediator;
            _TaxRepository = taxRepository;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the AddLoanCommand to add a new Loan.
        /// </summary>
        public async Task<ServiceResponse<LoanDto>> Handle(AddLoanCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve loan application with related data
                var loanApplication = await _loanApplicationRepository.AllIncluding(x => x.LoanProduct)
                    .Include(x => x.LoanProduct.LoanProductRepaymentCycles)
                    .Include(x => x.LoanApplicationFees)
                    .FirstOrDefaultAsync(x => x.Id == request.LoanApplicationId);

                if (loanApplication == null)
                {
                    var errorMessage = $"Loan application with ID {request.LoanApplicationId} does not exist.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<LoanDto>.Return404(errorMessage);
                }
                var loanProduct = await _loanProductRepository.FindAsync(loanApplication.LoanProductId);
                string oldLoanId = "N/A";
                var oldLoan = await _loanRepository.FindAsync(loanApplication.LoanId);

                // Generate a unique loan ID
                string loanId = BaseUtilities.GenerateInsuranceUniqueNumber(10, $"{_userInfoToken.BranchCode}-L");
                if (loanApplication.LoanApplicationType==LoanApplicationTypes.Refinancing.ToString())
                {
                    loanId = BaseUtilities.GenerateInsuranceUniqueNumber(10, $"{_userInfoToken.BranchCode}-R");
                }

                if (loanApplication.LoanApplicationType == LoanApplicationTypes.Normal.ToString()|| loanApplication.LoanApplicationType == LoanApplicationTypes.Refinancing.ToString())
                {

                    if (oldLoan != null)
                    {
                        oldLoanId = HandleLoanApplicationType(loanApplication, oldLoan, loanId);
                    }
                    // Generate loan parameters and amortization schedule
                    var loanParameter = LoanValidation.AddLoanAmortizationCommandRequest(loanApplication, loanApplication.VatRate, 0, loanId);
                    var loanAmortizations = LoanCalculator.GenerateAmortizationSchedule(loanParameter);
                    // Map loan application to a new loan object
                    var loan = LoanValidation.NewLoanObjectMappring(loanApplication, loanAmortizations, loanId, request.BranchCode, request.CustomerName, loanProduct);
                    loan.LoanId = oldLoanId;

                    _loanRepository.Add(loan);
                    //if (oldLoan != null)
                    //{
                        
                    //}

                    // Create Loan Amortization
                    var addLoanAmortization = new AddLoanAmortizationCommand { LoanApplicationId = request.LoanApplicationId, LoanId = loanId, LoanApplication=loanApplication };
                    var responseAmortization = await _mediator.Send(addLoanAmortization, cancellationToken);

                    if (responseAmortization.StatusCode == 200)
                    {
                        var loanDto = _mapper.Map<LoanDto>(loan);
                        loanDto.NumberOfInstallments = loanAmortizations.Count();
                        loanDto.RepaymentCycle = loanParameter.RepaymentCycle;
                        return ServiceResponse<LoanDto>.ReturnResultWith200(loanDto);
                    }
                    return ServiceResponse<LoanDto>.Return403(responseAmortization.Message);


                }

                else 
                {
                    if (oldLoan != null)
                    {
                        if (loanApplication.LoanApplicationType == LoanApplicationTypes.Reschedule.ToString())
                        {
                            loanApplication.OldLoanVat = 0;
                            loanApplication.OldLoanInterest = 0;
                            loanApplication.OldLoanPenalty = 0;
                        }
                        oldLoanId = HandleLoanApplicationType(loanApplication, oldLoan, loanId);
                    }


                    // Generate loan parameters and amortization schedule
                    var loanParameter = LoanValidation.AddLoanAmortizationCommandRequest(loanApplication, loanApplication.VatRate, 0, oldLoan.Id);
                    var loanAmortizations = LoanCalculator.GenerateAmortizationSchedule(loanParameter);

                    // Map loan application to a new loan object
                    var loan = LoanValidation.NewLoanObjectMappring(loanApplication, loanAmortizations,oldLoan);
                    _loanRepository.Update(loan);

                    // Create Loan Amortization
                    var addLoanAmortization = new AddLoanAmortizationCommand { LoanApplicationId = request.LoanApplicationId, LoanId = oldLoan.Id };
                    var responseAmortization = await _mediator.Send(addLoanAmortization, cancellationToken);

                    if (responseAmortization.StatusCode == 200)
                    {
                        var loanDto = _mapper.Map<LoanDto>(loan);
                        loanDto.NumberOfInstallments = loanAmortizations.Count();
                        loanDto.RepaymentCycle = loanParameter.RepaymentCycle;
                        return ServiceResponse<LoanDto>.ReturnResultWith200(loanDto);
                    }

                    return ServiceResponse<LoanDto>.Return403(responseAmortization.Message);

                }


            }
            catch (Exception e)
            {
                var errorMessage = $"An error occurred while creating the loan: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanDto>.Return500(e);
            }
        }

        private string HandleLoanApplicationType(LoanApplication loanApplication, Loan oldLoan,string newLoanId)
        {
            // Calculate the restructured balance based on whether the old loan's balance and interest are overridden
            decimal CalculateRestructuredBalance()
            {
                return loanApplication.IsOverRightOldLoanInterestAndBalance
                    ? loanApplication.OldLoanCapital + loanApplication.OldLoanInterest + loanApplication.OldLoanPenalty + loanApplication.OldLoanVat
                    : oldLoan.Balance + oldLoan.AccrualInterest;
            }

            // Update old loan status and set the restructured balance
            void UpdateLoanStatusAndBalance(string newStatus)
            {
                oldLoan.LoanStatus = newStatus;
                decimal restructuredBalance = CalculateRestructuredBalance();

                loanApplication.RequestedAmount = loanApplication.Amount;
                loanApplication.RestructuredBalance = restructuredBalance;
                loanApplication.Amount += restructuredBalance; // Increase the loan amount by the restructured balance
                                                               // Mark the old loan as no longer current and indicate it has been restructured
                oldLoan.IsCurrentLoan = false;
                oldLoan.LoanStructuringStatus = loanApplication.LoanApplicationType;

                oldLoan.OldBalance = oldLoan.Balance;
                oldLoan.OldCapital = oldLoan.Principal;
                oldLoan.OldInterest = oldLoan.AccrualInterest;
                oldLoan.OldPenalty = oldLoan.Penalty;
                oldLoan.OldDueAmount = oldLoan.DueAmount;

                oldLoan.NewLoanId = newLoanId;
                oldLoan.LoanId = loanApplication.LoanId;
                oldLoan.Balance = loanApplication.OldLoanCapital;
                oldLoan.DueAmount = loanApplication.OldLoanAmount;
                oldLoan.AccrualInterest = loanApplication.OldLoanInterest;
                oldLoan.Penalty = loanApplication.OldLoanPenalty;
                oldLoan.LoanStructuringDate = BaseUtilities.UtcNowToDoualaTime();
                oldLoan.LoanJourneyStatus = newStatus;
                oldLoan.LoanStatus = newStatus;

                _loanRepository.Update(oldLoan);
            }

            // Switch based on the loan application type and update the corresponding loan status
            switch (loanApplication.LoanApplicationType)
            {
                case nameof(LoanApplicationTypes.Refinancing):
                    UpdateLoanStatusAndBalance("Refinanced");
                    break;

                case nameof(LoanApplicationTypes.Reschedule):
                    UpdateLoanStatusAndBalance("Rescheduled");
                    break;

                case nameof(LoanApplicationTypes.Restructure):
                    UpdateLoanStatusAndBalance("Restructured");
                    break;
            }



            // Return the ID of the old loan for tracking or further processing
            return oldLoan.Id;
        }


        //    private string HandleLoanApplicationType(LoanApplication loanApplication, Loan oldLoan)
        //    {
        //        decimal restrB = 0;

        //        switch (loanApplication.LoanApplicationType)
        //        {
        //            case nameof(LoanApplicationTypes.Refinancing):
        //                oldLoan.LoanStatus = "Refinanced";
        //                restrB = loanApplication.IsOverRightOldLoanInterestAndBalance
        //                    ? loanApplication.NewBalance + loanApplication.NewInterest + loanApplication.NewPenalty + loanApplication.NewVAT
        //                    : oldLoan.Balance + oldLoan.AccrualInterest;

        //                loanApplication.RequestedAmount = loanApplication.Amount;
        //                loanApplication.RestructuredBalance = restrB;
        //                loanApplication.Amount += restrB;

        //                break;

        //            case nameof(LoanApplicationTypes.Reschedule):
        //                oldLoan.LoanStatus = "Rescheduled";
        //                restrB = loanApplication.IsOverRightOldLoanInterestAndBalance
        //? loanApplication.NewBalance + loanApplication.NewInterest + loanApplication.NewPenalty + loanApplication.NewVAT
        //: oldLoan.Balance + oldLoan.AccrualInterest;

        //                loanApplication.RequestedAmount = loanApplication.Amount;
        //                loanApplication.RestructuredBalance = restrB;
        //                loanApplication.Amount += restrB;
        //                break;

        //            case nameof(LoanApplicationTypes.Restructure):
        //                oldLoan.LoanStatus = "Restructured";
        //                restrB = loanApplication.IsOverRightOldLoanInterestAndBalance
        //? loanApplication.NewBalance + loanApplication.NewInterest + loanApplication.NewPenalty + loanApplication.NewVAT
        //: oldLoan.Balance + oldLoan.AccrualInterest;
        //                loanApplication.RequestedAmount = loanApplication.Amount;
        //                loanApplication.RestructuredBalance = restrB;
        //                loanApplication.Amount += restrB;
        //                break;
        //        }

        //        oldLoan.IsCurrentLoan = false;
        //        oldLoan.IsRestructured = true;
        //        return oldLoan.Id;
        //    }
    }

}
