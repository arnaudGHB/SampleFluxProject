using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanProductMediaR.Commands;
using CBS.NLoan.Repository.LoanApplicationP.LoanCommentryP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanProductMediaR.Handlers
{
    /// <summary>
    /// Handles the command to delete a Loan based on DeleteLoanCommand.
    /// </summary>
    public class DeleteLoanProductRepaymentOrderCommandHandler : IRequestHandler<DeleteLoanProductRepaymentCycleCommand, ServiceResponse<bool>>
    {
        private readonly ILoanProductRepaymentOrderRepository _LoanProductRepaymentOrderRepository; // Repository for accessing LoanProductRepaymentOrder data.
        private readonly ILogger<DeleteLoanProductRepaymentOrderCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteLoanProductRepaymentOrderCommandHandler.
        /// </summary>
        /// <param name="LoanProductRepaymentOrderRepository">Repository for LoanProductRepaymentOrder data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteLoanProductRepaymentOrderCommandHandler(
            ILoanProductRepaymentOrderRepository LoanProductRepaymentOrderRepository, IMapper mapper,
            ILogger<DeleteLoanProductRepaymentOrderCommandHandler> logger, IUnitOfWork<LoanContext> uow)
        {
            _LoanProductRepaymentOrderRepository = LoanProductRepaymentOrderRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteLoanProductRepaymentOrderCommand to delete a LoanProductRepaymentOrder.
        /// </summary>
        /// <param name="request">The DeleteLoanProductRepaymentOrderCommand containing LoanProductRepaymentOrder ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteLoanProductRepaymentCycleCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the LoanProductRepaymentOrder entity with the specified ID exists
                var existingLoanProductRepaymentOrder = await _LoanProductRepaymentOrderRepository.FindAsync(request.Id);
                if (existingLoanProductRepaymentOrder == null)
                {
                    errorMessage = $"LoanProductRepaymentOrder with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingLoanProductRepaymentOrder.IsDeleted = true;
                _LoanProductRepaymentOrderRepository.Update(existingLoanProductRepaymentOrder);
                await _uow.SaveAsync();
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting LoanProductRepaymentOrder: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
