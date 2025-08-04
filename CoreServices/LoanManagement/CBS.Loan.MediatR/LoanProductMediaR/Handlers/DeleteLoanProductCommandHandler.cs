using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanProductMediaR.Commands;
using CBS.NLoan.Repository.LoanApplicationP;
using CBS.NLoan.Repository.LoanProductP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanProductMediaR.Handlers
{
    /// <summary>
    /// Handles the command to delete a Loan based on DeleteLoanCommand.
    /// </summary>
    public class DeleteLoanProductCommandHandler : IRequestHandler<DeleteLoanProductCommand, ServiceResponse<bool>>
    {
        private readonly ILoanProductRepository _LoanProductRepository; // Repository for accessing LoanProduct data.
        private readonly ILoanApplicationRepository _loanApplicationRepository; // Repository for accessing LoanProduct data.

        private readonly ILogger<DeleteLoanProductCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteLoanProductCommandHandler.
        /// </summary>
        /// <param name="LoanProductRepository">Repository for LoanProduct data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteLoanProductCommandHandler(
            ILoanProductRepository LoanProductRepository, IMapper mapper,
            ILogger<DeleteLoanProductCommandHandler> logger, IUnitOfWork<LoanContext> uow, ILoanApplicationRepository loanApplicationRepository = null)
        {
            _LoanProductRepository = LoanProductRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
            _loanApplicationRepository = loanApplicationRepository;
        }

        /// <summary>
        /// Handles the DeleteLoanProductCommand to delete a LoanProduct.
        /// </summary>
        /// <param name="request">The DeleteLoanProductCommand containing LoanProduct ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteLoanProductCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the LoanProduct entity with the specified ID exists

                var existingLoanProduct = await _LoanProductRepository.FindAsync(request.LoanProductId);
                if (existingLoanProduct == null)
                {
                    errorMessage = $"LoanProduct with ID {request.LoanProductId} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }
                var loanApplications = await _loanApplicationRepository.FindBy(x => x.LoanProductId == request.LoanProductId).ToListAsync();
                if (loanApplications.Any())
                {
                    errorMessage = $"Can not delete {existingLoanProduct.ProductName}. Its used by {loanApplications.Count()} loan applications.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return403(errorMessage);
                }
                existingLoanProduct.IsDeleted = true;
                existingLoanProduct.DeletedBy = request.UserId;
                _LoanProductRepository.Update(existingLoanProduct);
                await _uow.SaveAsync();
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting LoanProduct: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
