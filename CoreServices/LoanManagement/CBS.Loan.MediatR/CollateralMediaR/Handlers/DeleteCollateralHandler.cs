using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.CollateralMediaR.Commands;
using CBS.NLoan.Repository.CollateralP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.CollateralMediaR.Handlers
{
    /// <summary>
    /// Handles the command to delete a Loan based on DeleteLoanCommand.
    /// </summary>
    public class DeleteCollateralHandler : IRequestHandler<DeleteCollateralCommand, ServiceResponse<bool>>
    {
        private readonly ICollateralRepository _CollateralRepository; // Repository for accessing Collateral data.
        private readonly ILogger<DeleteCollateralHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteCollateralCommandHandler.
        /// </summary>
        /// <param name="CollateralRepository">Repository for Collateral data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteCollateralHandler(
            ICollateralRepository CollateralRepository, IMapper mapper,
            ILogger<DeleteCollateralHandler> logger, IUnitOfWork<LoanContext> uow)
        {
            _CollateralRepository = CollateralRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteCollateralCommand to delete a Collateral.
        /// </summary>
        /// <param name="request">The DeleteCollateralCommand containing Collateral ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteCollateralCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the Collateral entity with the specified ID exists
                var existingCollateral = await _CollateralRepository.FindAsync(request.Id);
                if (existingCollateral == null)
                {
                    errorMessage = $"Collateral with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingCollateral.IsDeleted = true;
                _CollateralRepository.Update(existingCollateral);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting Collateral: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
