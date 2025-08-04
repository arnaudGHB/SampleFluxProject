using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.FeeP;
using CBS.NLoan.Data.Entity.FeeP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanApplicationFeeMediaR.LoanApplicationFeeP.Commands;
using CBS.NLoan.Repository.FeeP.FeeP;
using CBS.NLoan.Repository.FeeRangeP;
using CBS.NLoan.Repository.LoanApplicationFeeP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NPOI.SS.UserModel;

namespace CBS.NLoan.MediatR.LoanApplicationFeeMediaR.LoanApplicationFeeP.Handlers
{
    /// <summary>
    /// Handles the command to add a new Loan.
    /// </summary>
    public class AddLoanApplicationFeeHandler : IRequestHandler<AddLoanApplicationFeeCommand, ServiceResponse<decimal>>
    {
        private readonly ILoanApplicationFeeRepository _loanApplicationFeeRepository; // Repository for accessing LoanApplicationFee data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IFeeRangeRepository _feeRangeRepository; // Repository for accessing LoanApplicationFee data.
        private readonly ILogger<AddLoanApplicationFeeHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<LoanContext> _uow;
        private readonly IFeeRepository _feeRepository; // Repository for accessing LoanApplicationFee data.

        /// <summary>
        /// Constructor for initializing the AddLoanApplicationFeeHandler.
        /// </summary>
        /// <param name="loanApplicationFeeRepository">Repository for LoanApplicationFee data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="uow">Unit of Work for database operations.</param>
        public AddLoanApplicationFeeHandler(
            ILoanApplicationFeeRepository loanApplicationFeeRepository,
            IMapper mapper,
            ILogger<AddLoanApplicationFeeHandler> logger,
            IUnitOfWork<LoanContext> uow,
            IFeeRangeRepository feeRangeRepository = null,
            IFeeRepository feeRepository = null)
        {
            _loanApplicationFeeRepository = loanApplicationFeeRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _feeRangeRepository = feeRangeRepository;
            _feeRepository = feeRepository;
        }

        /// <summary>
        /// Handles the AddLoanApplicationFeeCommand to add a new LoanApplicationFee.
        /// </summary>
        /// <param name="request">The AddLoanApplicationFeeCommand containing LoanApplicationFee data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<decimal>> Handle(AddLoanApplicationFeeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                decimal Totalcharge = 0;

                foreach (var feeId in request.FeeId)
                {
                    var fee = await _feeRepository.FindAsync(feeId);
                    if (fee == null)
                    {
                        var errorMessage = $"The fee with ID {feeId} does not exist. Please check and try again.";
                        _logger.LogError(errorMessage);
                        throw new InvalidOperationException(errorMessage);
                    }

                    var feeRange = fee.FeeBase == "Range" ?
                        await _feeRangeRepository.FindBy(x => x.AmountFrom <= request.Amount && x.AmountTo >= request.Amount && x.FeeId == feeId && x.IsDeleted == false).FirstOrDefaultAsync() :
                        await _feeRangeRepository.FindBy(policy => policy.FeeId == feeId && policy.IsDeleted == false).FirstOrDefaultAsync();

                    if (feeRange == null)
                    {
                        var errorMessage = $"No applicable charge found in the range for fee {fee.Name}. Ensure the requested amount falls within a defined range.";
                        _logger.LogError(errorMessage);
                        throw new InvalidOperationException(errorMessage);
                    }

                    var loanApplicationFeeEntity = _mapper.Map<LoanApplicationFee>(request);
                    var loanApplicationFee = await _loanApplicationFeeRepository.FindBy(x => x.FeeRangeId == feeId && x.IsDeleted == false).FirstOrDefaultAsync();

                    if (loanApplicationFee == null)
                    {
                        loanApplicationFeeEntity.Id = BaseUtilities.GenerateUniqueNumber();
                        loanApplicationFeeEntity.FeeRangeId = feeRange.Id;
                        decimal charge = BaseUtilities.GetChargeRoundedUp(request.Amount, feeRange.PercentageValue, fee.FeeBase, feeRange.Charge);
                        Totalcharge += charge;
                        loanApplicationFeeEntity.Period = fee.IsBoforeProcesing ? "Before" : "After";
                        loanApplicationFeeEntity.AmountPaid = 0;
                        loanApplicationFeeEntity.IsCashDeskPayment = fee.IsBoforeProcesing;
                        loanApplicationFeeEntity.CustomerId = request.CustomerId;
                        loanApplicationFeeEntity.Status = "Pending";
                        loanApplicationFeeEntity.FeeAmount = charge;
                        loanApplicationFeeEntity.EventCode = fee.AccountingEventCode ?? "N/A";
                        loanApplicationFeeEntity.FeeLable = fee.Name;
                        _loanApplicationFeeRepository.Add(loanApplicationFeeEntity);
                    }
                }

                if (!request.IsWithin)
                {
                    await _uow.SaveAsync();
                }

                return ServiceResponse<decimal>.ReturnResultWith200(Totalcharge, "Charges processed successfully.");
            }
            catch (InvalidOperationException ex)
            {
                // Specific user-friendly error handling
                var userFriendlyError = $"Validation Error: {ex.Message}";
                _logger.LogError(userFriendlyError);
                return ServiceResponse<decimal>.Return400(userFriendlyError);
            }
            catch (Exception e)
            {
                // General error handling
                var errorMessage = $"An unexpected error occurred while processing loan application fees: {e.Message}. Please try again later or contact support.";
                _logger.LogError(errorMessage);
                return ServiceResponse<decimal>.Return500(e);
            }
        }
        public decimal GetCharge(decimal amount, string feerangeid, FeeRange feeRange, Fee fee)
        {
            // Find the FeeRangeDto based on the provided FeeRangeId

            if (feeRange == null)
            {
                var errorMessage = $"Fee range with ID {feerangeid} not found.";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            decimal value = 0;

            // Determine the value based on whether it's a range or percentage
            if (fee.FeeBase == "Range")
            {
                value = feeRange.Charge;
            }
            else
            {
                value = feeRange.PercentageValue / 100 * amount;
            }
            value=BaseUtilities.RoundUpValue(value);
            return value;
        }


    }

}
