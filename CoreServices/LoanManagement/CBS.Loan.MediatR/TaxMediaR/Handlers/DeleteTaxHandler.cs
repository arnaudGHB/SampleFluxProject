using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.TaxMediaR.Commands;
using CBS.NLoan.Repository.TaxP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.TaxMediaR.Handlers
{
    /// <summary>
    /// Handles the command to delete a Loan based on DeleteLoanCommand.
    /// </summary>
    public class DeleteTaxHandler : IRequestHandler<DeleteTaxCommand, ServiceResponse<bool>>
    {
        private readonly ITaxRepository _TaxRepository; // Repository for accessing Tax data.
        private readonly ILogger<DeleteTaxHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteTaxCommandHandler.
        /// </summary>
        /// <param name="TaxRepository">Repository for Tax data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteTaxHandler(
            ITaxRepository TaxRepository, IMapper mapper,
            ILogger<DeleteTaxHandler> logger, IUnitOfWork<LoanContext> uow)
        {
            _TaxRepository = TaxRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteTaxCommand to delete a Tax.
        /// </summary>
        /// <param name="request">The DeleteTaxCommand containing Tax ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteTaxCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the Tax entity with the specified ID exists
                var existingTax = await _TaxRepository.FindAsync(request.Id);
                if (existingTax == null)
                {
                    errorMessage = $"Tax with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingTax.IsDeleted = true;
                _TaxRepository.Update(existingTax);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting Tax: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
