using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.RefundP;
using CBS.NLoan.Data.Entity.RefundP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.RefundMediaR.Commands;
using CBS.NLoan.Repository.RefundP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.RefundMediaR.Handlers
{

    /// <summary>
    /// Handles the command to update a Loan based on UpdateLoanCommand.
    /// </summary>
    public class UpdateRefundHandler : IRequestHandler<UpdateRefundCommand, ServiceResponse<RefundDto>>
    {
        private readonly IRefundRepository _RefundRepository; // Repository for accessing Refund data.
        private readonly ILogger<UpdateRefundHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateRefundCommandHandler.
        /// </summary>
        /// <param name="RefundRepository">Repository for Refund data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateRefundHandler(
            IRefundRepository RefundRepository,
            ILogger<UpdateRefundHandler> logger,
            IMapper mapper,
            IUnitOfWork<LoanContext> uow = null)
        {
            _RefundRepository = RefundRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateRefundCommand to update a Refund.
        /// </summary>
        /// <param name="request">The UpdateRefundCommand containing updated Refund data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<RefundDto>> Handle(UpdateRefundCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the Refund entity to be updated from the repository
                var existingRefund = await _RefundRepository.FindAsync(request.Id);

                // Check if the Refund entity exists
                if (existingRefund != null)
                {
                    // Update Refund entity properties with values from the request
                    var periodToUpdate = _mapper.Map<Refund>(request);
                    // Use the repository to update the existing Refund entity
                    _RefundRepository.Update(periodToUpdate);
                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<RefundDto>.Return500();
                    }
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<RefundDto>.ReturnResultWith200(_mapper.Map<RefundDto>(existingRefund));
                    _logger.LogInformation($"Refund {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the Refund entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<RefundDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Refund: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<RefundDto>.Return500(e);
            }
        }
    }

}
