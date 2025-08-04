using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanCollateralMediaR.Commands;
using CBS.NLoan.Repository.CollateralP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanCollateralMediaR.Handlers
{
    /// <summary>
    /// Handles the command to delete a Loan based on DeleteLoanCommand.
    /// </summary>
    public class DeleteLoanCollateralHandler : IRequestHandler<DeleteLoanCollateralCommand, ServiceResponse<bool>>
    {
        private readonly ILoanCollateralRepository _LoanCollateralRepository; // Repository for accessing LoanCollateral data.
        private readonly ILogger<DeleteLoanCollateralHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteLoanCollateralCommandHandler.
        /// </summary>
        /// <param name="LoanCollateralRepository">Repository for LoanCollateral data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteLoanCollateralHandler(
            ILoanCollateralRepository LoanCollateralRepository, IMapper mapper,
            ILogger<DeleteLoanCollateralHandler> logger, IUnitOfWork<LoanContext> uow)
        {
            _LoanCollateralRepository = LoanCollateralRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteLoanCollateralCommand to delete a LoanCollateral.
        /// </summary>
        /// <param name="request">The DeleteLoanCollateralCommand containing LoanCollateral ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteLoanCollateralCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the LoanCollateral entity with the specified ID exists
                var existingLoanCollateral = await _LoanCollateralRepository.FindAsync(request.Id);
                if (existingLoanCollateral == null)
                {
                    errorMessage = $"LoanCollateral with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingLoanCollateral.IsDeleted = true;
                _LoanCollateralRepository.Update(existingLoanCollateral);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting LoanCollateral: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
