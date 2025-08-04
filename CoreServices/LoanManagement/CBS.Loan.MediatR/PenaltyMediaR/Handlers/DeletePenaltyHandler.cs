using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.PenaltyMediaR.Commands;
using CBS.NLoan.Repository.PenaltyP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.PenaltyMediaR.Handlers
{
    /// <summary>
    /// Handles the command to delete a Loan based on DeleteLoanCommand.
    /// </summary>
    public class DeletePenaltyHandler : IRequestHandler<DeletePenaltyCommand, ServiceResponse<bool>>
    {
        private readonly IPenaltyRepository _PenaltyRepository; // Repository for accessing Penalty data.
        private readonly ILogger<DeletePenaltyHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeletePenaltyCommandHandler.
        /// </summary>
        /// <param name="PenaltyRepository">Repository for Penalty data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeletePenaltyHandler(
            IPenaltyRepository PenaltyRepository, IMapper mapper,
            ILogger<DeletePenaltyHandler> logger, IUnitOfWork<LoanContext> uow)
        {
            _PenaltyRepository = PenaltyRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeletePenaltyCommand to delete a Penalty.
        /// </summary>
        /// <param name="request">The DeletePenaltyCommand containing Penalty ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeletePenaltyCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the Penalty entity with the specified ID exists
                var existingPenalty = await _PenaltyRepository.FindAsync(request.Id);
                if (existingPenalty == null)
                {
                    errorMessage = $"Penalty with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingPenalty.IsDeleted = true;
                _PenaltyRepository.Update(existingPenalty);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting Penalty: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
