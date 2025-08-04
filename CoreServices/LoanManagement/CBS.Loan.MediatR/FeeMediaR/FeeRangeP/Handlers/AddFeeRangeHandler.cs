using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.FeeP;
using CBS.NLoan.Data.Entity.FeeP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.FeeMediaR.FeeP.Commands;
using CBS.NLoan.MediatR.FeeMediaR.FeeRangeP.Commands;
using CBS.NLoan.Repository.FeeP.FeeP;
using CBS.NLoan.Repository.FeeRangeP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.FeeMediaR.FeeRangeP.Handlers
{
    /// <summary>
    /// Handles the command to add a new Loan.
    /// </summary>
    public class AddFeeRangeHandler : IRequestHandler<AddFeeRangeCommand, ServiceResponse<FeeRangeDto>>
    {
        private readonly IFeeRangeRepository _feeRangeRepository; // Repository for accessing Fee data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddFeeRangeHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<LoanContext> _unitOfWork;

        /// <summary>
        /// Constructor for initializing the AddFeeRangeHandler.
        /// </summary>
        /// <param name="feeRangeRepository">Repository for Fee data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="unitOfWork">Unit of Work for database operations.</param>
        public AddFeeRangeHandler(
            IFeeRangeRepository feeRangeRepository,
            IMapper mapper,
            ILogger<AddFeeRangeHandler> logger,
            IUnitOfWork<LoanContext> unitOfWork)
        {
            _feeRangeRepository = feeRangeRepository;
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Handles the AddFeeRangeCommand to add a new Fee Range.
        /// </summary>
        /// <param name="request">The AddFeeRangeCommand containing Fee Range data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<FeeRangeDto>> Handle(AddFeeRangeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a Fee with the same details already exists
                var existingFee = await _feeRangeRepository.FindBy(c =>
                    c.FeeId == request.FeeId &&
                    c.AmountFrom == request.AmountFrom &&
                    c.AmountTo == request.AmountTo)
                    .FirstOrDefaultAsync();

                // If a Fee with the same details already exists, return a conflict response
                if (existingFee != null)
                {
                    var errorMessage = $"Fee range already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<FeeRangeDto>.Return409(errorMessage);
                }

                // Map the AddFeeRangeCommand to a Fee Range entity
                var feeRangeEntity = _mapper.Map<FeeRange>(request);
                feeRangeEntity.Id = BaseUtilities.GenerateUniqueNumber();

                // Add the new Fee Range entity to the repository
                _feeRangeRepository.Add(feeRangeEntity);
                await _unitOfWork.SaveAsync();

                // Map the Fee Range entity to FeeRangeDto and return it with a success response
                var feeRangeDto = _mapper.Map<FeeRangeDto>(feeRangeEntity);
                return ServiceResponse<FeeRangeDto>.ReturnResultWith200(feeRangeDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Fee Range: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<FeeRangeDto>.Return500(e);
            }
        }
    }

}
