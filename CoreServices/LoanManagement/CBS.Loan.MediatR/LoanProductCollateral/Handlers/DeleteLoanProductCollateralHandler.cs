using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanProductCollateralMediaR.Commands;
using CBS.NLoan.Repository.CollateralP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanProductCollateralMediaR.Handlers
{
    /// <summary>
    /// Handles the command to delete a Loan based on DeleteLoanCommand.
    /// </summary>
    public class DeleteLoanProductCollateralHandler : IRequestHandler<DeleteLoanProductCollateralCommand, ServiceResponse<bool>>
    {
        private readonly ILoanProductCollateralRepository _LoanProductCollateralRepository; // Repository for accessing LoanProductCollateral data.
        private readonly ILogger<DeleteLoanProductCollateralHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteLoanProductCollateralCommandHandler.
        /// </summary>
        /// <param name="LoanProductCollateralRepository">Repository for LoanProductCollateral data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteLoanProductCollateralHandler(
            ILoanProductCollateralRepository LoanProductCollateralRepository, IMapper mapper,
            ILogger<DeleteLoanProductCollateralHandler> logger, IUnitOfWork<LoanContext> uow)
        {
            _LoanProductCollateralRepository = LoanProductCollateralRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteLoanProductCollateralCommand to delete a LoanProductCollateral.
        /// </summary>
        /// <param name="request">The DeleteLoanProductCollateralCommand containing LoanProductCollateral ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteLoanProductCollateralCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the LoanProductCollateral entity with the specified ID exists
                var existingLoanProductCollateral = await _LoanProductCollateralRepository.FindAsync(request.Id);
                if (existingLoanProductCollateral == null)
                {
                    errorMessage = $"LoanProductCollateral with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingLoanProductCollateral.IsDeleted = true;
                _LoanProductCollateralRepository.Update(existingLoanProductCollateral);
                await _uow.SaveAsync();
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting LoanProductCollateral: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
