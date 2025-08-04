using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.RefundMediaR.Commands;
using CBS.NLoan.Repository.RefundP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.RefundMediaR.Handlers
{
    /// <summary>
    /// Handles the command to delete a Loan based on DeleteLoanCommand.
    /// </summary>
    public class DeleteRefundHandler : IRequestHandler<DeleteRefundCommand, ServiceResponse<bool>>
    {
        private readonly IRefundRepository _RefundRepository; // Repository for accessing Refund data.
        private readonly ILogger<DeleteRefundHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteRefundCommandHandler.
        /// </summary>
        /// <param name="RefundRepository">Repository for Refund data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteRefundHandler(
            IRefundRepository RefundRepository, IMapper mapper,
            ILogger<DeleteRefundHandler> logger, IUnitOfWork<LoanContext> uow)
        {
            _RefundRepository = RefundRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteRefundCommand to delete a Refund.
        /// </summary>
        /// <param name="request">The DeleteRefundCommand containing Refund ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteRefundCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the Refund entity with the specified ID exists
                var existingRefund = await _RefundRepository.FindAsync(request.Id);
                if (existingRefund == null)
                {
                    errorMessage = $"Refund with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingRefund.IsDeleted = true;
                _RefundRepository.Update(existingRefund);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting Refund: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
