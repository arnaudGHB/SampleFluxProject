using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.PeriodMediaR.Commands;
using CBS.NLoan.Repository.PeriodP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.PeriodMediaR.Handlers
{
    /// <summary>
    /// Handles the command to delete a Loan based on DeleteLoanCommand.
    /// </summary>
    public class DeletePeriodHandler : IRequestHandler<DeletePeriodCommand, ServiceResponse<bool>>
    {
        private readonly IPeriodRepository _PeriodRepository; // Repository for accessing Period data.
        private readonly ILogger<DeletePeriodHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeletePeriodCommandHandler.
        /// </summary>
        /// <param name="PeriodRepository">Repository for Period data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeletePeriodHandler(
            IPeriodRepository PeriodRepository, IMapper mapper,
            ILogger<DeletePeriodHandler> logger, IUnitOfWork<LoanContext> uow)
        {
            _PeriodRepository = PeriodRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeletePeriodCommand to delete a Period.
        /// </summary>
        /// <param name="request">The DeletePeriodCommand containing Period ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeletePeriodCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the Period entity with the specified ID exists
                var existingPeriod = await _PeriodRepository.FindAsync(request.Id);
                if (existingPeriod == null)
                {
                    errorMessage = $"Period with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingPeriod.IsDeleted = true;
                _PeriodRepository.Update(existingPeriod);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting Period: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
