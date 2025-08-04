using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanTermP.Commands;
using CBS.NLoan.Repository.LoanTermP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanTermP.Handlers
{
    /// <summary>
    /// Handles the command to delete a Loan based on DeleteLoanCommand.
    /// </summary>
    public class DeleteLoanTermHandler : IRequestHandler<DeleteLoanTermCommand, ServiceResponse<bool>>
    {
        private readonly ILoanTermRepository _LoanTermRepository; // Repository for accessing LoanTerm data.
        private readonly ILogger<DeleteLoanTermHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteLoanTermCommandHandler.
        /// </summary>
        /// <param name="LoanTermRepository">Repository for LoanTerm data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteLoanTermHandler(
            ILoanTermRepository LoanTermRepository, IMapper mapper,
            ILogger<DeleteLoanTermHandler> logger, IUnitOfWork<LoanContext> uow)
        {
            _LoanTermRepository = LoanTermRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteLoanTermCommand to delete a LoanTerm.
        /// </summary>
        /// <param name="request">The DeleteLoanTermCommand containing LoanTerm ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteLoanTermCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the LoanTerm entity with the specified ID exists
                var existingLoanTerm = await _LoanTermRepository.FindAsync(request.Id);
                if (existingLoanTerm == null)
                {
                    errorMessage = $"LoanTerm with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingLoanTerm.IsDeleted = true;
                _LoanTermRepository.Update(existingLoanTerm);
                await _uow.SaveAsync();
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting LoanTerm: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
