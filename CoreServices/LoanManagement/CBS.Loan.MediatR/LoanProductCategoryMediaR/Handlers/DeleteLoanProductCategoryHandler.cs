using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.CycleNameMediaR.Commands;
using CBS.NLoan.Repository.LoanApplicationP.LoanCycleP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.CycleNameMediaR.Handlers
{
    /// <summary>
    /// Handles the command to delete a Loan based on DeleteLoanCommand.
    /// </summary>
    public class DeleteLoanProductCategoryHandler : IRequestHandler<DeleteLoanProductCategoryCommand, ServiceResponse<bool>>
    {
        private readonly ILoanProductCategoryRepository _LoanCycleRepository; // Repository for accessing LoanProductCategory data.
        private readonly ILogger<DeleteLoanProductCategoryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteLoanCycleCommandHandler.
        /// </summary>
        /// <param name="LoanCycleRepository">Repository for LoanProductCategory data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteLoanProductCategoryHandler(
            ILoanProductCategoryRepository LoanCycleRepository, IMapper mapper,
            ILogger<DeleteLoanProductCategoryHandler> logger, IUnitOfWork<LoanContext> uow)
        {
            _LoanCycleRepository = LoanCycleRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteLoanProductCategoryCommand to delete a LoanProductCategory.
        /// </summary>
        /// <param name="request">The DeleteLoanProductCategoryCommand containing LoanProductCategory ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteLoanProductCategoryCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the LoanProductCategory entity with the specified ID exists
                var existingLoanCycle = await _LoanCycleRepository.FindAsync(request.Id);
                if (existingLoanCycle == null)
                {
                    errorMessage = $"LoanProductCategory with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingLoanCycle.IsDeleted = true;
                _LoanCycleRepository.Update(existingLoanCycle);
                await _uow.SaveAsync();
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting LoanProductCategory: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
