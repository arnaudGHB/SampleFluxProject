using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanGuarantorMediaR.Commands;
using CBS.NLoan.Repository.LoanApplicationP.LoanGuarantorP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanGuarantorMediaR.Handlers
{
    /// <summary>
    /// Handles the command to delete a Loan based on DeleteLoanCommand.
    /// </summary>
    public class DeleteLoanGuarantorHandler : IRequestHandler<DeleteLoanGuarantorCommand, ServiceResponse<bool>>
    {
        private readonly ILoanGuarantorRepository _LoanGuarantorRepository; // Repository for accessing LoanGuarantor data.
        private readonly ILogger<DeleteLoanGuarantorHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteLoanGuarantorCommandHandler.
        /// </summary>
        /// <param name="LoanGuarantorRepository">Repository for LoanGuarantor data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteLoanGuarantorHandler(
            ILoanGuarantorRepository LoanGuarantorRepository, IMapper mapper,
            ILogger<DeleteLoanGuarantorHandler> logger, IUnitOfWork<LoanContext> uow)
        {
            _LoanGuarantorRepository = LoanGuarantorRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteLoanGuarantorCommand to delete a LoanGuarantor.
        /// </summary>
        /// <param name="request">The DeleteLoanGuarantorCommand containing LoanGuarantor ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteLoanGuarantorCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the LoanGuarantor entity with the specified ID exists
                var existingLoanGuarantor = await _LoanGuarantorRepository.FindAsync(request.Id);
                if (existingLoanGuarantor == null)
                {
                    errorMessage = $"LoanGuarantor with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingLoanGuarantor.IsDeleted = true;
                _LoanGuarantorRepository.Update(existingLoanGuarantor);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting LoanGuarantor: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
