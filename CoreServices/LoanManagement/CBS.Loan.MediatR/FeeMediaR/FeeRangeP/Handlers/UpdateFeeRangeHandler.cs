using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.FeeP;
using CBS.NLoan.Data.Entity.FeeP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.FeeMediaR.FeeRangeP.Commands;
using CBS.NLoan.Repository.FeeRangeP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.FeeRangeMediaR.FeeRangeRangeP.Handlers
{

    /// <summary>
    /// Handles the command to update a Loan based on UpdateLoanCommand.
    /// </summary>
    public class UpdateFeeRangeHandler : IRequestHandler<UpdateFeeRangeCommand, ServiceResponse<FeeRangeDto>>
    {
        private readonly IFeeRangeRepository _feeRangeRepository; // Repository for accessing FeeRange data.
        private readonly ILogger<UpdateFeeRangeHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateFeeRangeHandler.
        /// </summary>
        /// <param name="feeRangeRepository">Repository for FeeRange data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="uow">Unit of Work for database operations.</param>
        public UpdateFeeRangeHandler(
            IFeeRangeRepository feeRangeRepository,
            ILogger<UpdateFeeRangeHandler> logger,
            IMapper mapper,
            IUnitOfWork<LoanContext> uow = null)
        {
            _feeRangeRepository = feeRangeRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateFeeRangeCommand to update a FeeRange.
        /// </summary>
        /// <param name="request">The UpdateFeeRangeCommand containing updated FeeRange data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<FeeRangeDto>> Handle(UpdateFeeRangeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the FeeRange entity to be updated from the repository
                var existingFeeRange = await _feeRangeRepository.FindAsync(request.Id);

                // Check if the FeeRange entity exists
                if (existingFeeRange != null)
                {
                    // Update FeeRange entity properties with values from the request
                    _mapper.Map(request, existingFeeRange);

                    // Use the repository to update the existing FeeRange entity
                    _feeRangeRepository.Update(existingFeeRange);
                    await _uow.SaveAsync();
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<FeeRangeDto>.ReturnResultWith200(_mapper.Map<FeeRangeDto>(existingFeeRange));
                    return response;
                }
                else
                {
                    // If the FeeRange entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<FeeRangeDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating FeeRange: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<FeeRangeDto>.Return500(e);
            }
        }
    }

}
