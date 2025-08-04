using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanPurposeMediaR.Commands;
using CBS.NLoan.Repository.LoanPurposeP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanPurposeMediaR.Handlers
{
    /// <summary>
    /// Handles the command to delete a Loan based on DeleteLoanCommand.
    /// </summary>
    public class DeleteLoanPurposeHandler : IRequestHandler<DeleteLoanPurposeCommand, ServiceResponse<bool>>
    {
        private readonly ILoanPurposeRepository _LoanPurposeRepository; // Repository for accessing LoanPurpose data.
        private readonly ILogger<DeleteLoanPurposeHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteLoanPurposeCommandHandler.
        /// </summary>
        /// <param name="LoanPurposeRepository">Repository for LoanPurpose data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteLoanPurposeHandler(
            ILoanPurposeRepository LoanPurposeRepository, IMapper mapper,
            ILogger<DeleteLoanPurposeHandler> logger, IUnitOfWork<LoanContext> uow)
        {
            _LoanPurposeRepository = LoanPurposeRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteLoanPurposeCommand to delete a LoanPurpose.
        /// </summary>
        /// <param name="request">The DeleteLoanPurposeCommand containing LoanPurpose ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteLoanPurposeCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the LoanPurpose entity with the specified ID exists
                var existingLoanPurpose = await _LoanPurposeRepository.FindAsync(request.Id);
                if (existingLoanPurpose == null)
                {
                    errorMessage = $"LoanPurpose with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingLoanPurpose.IsDeleted = true;
                _LoanPurposeRepository.Update(existingLoanPurpose);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting LoanPurpose: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
