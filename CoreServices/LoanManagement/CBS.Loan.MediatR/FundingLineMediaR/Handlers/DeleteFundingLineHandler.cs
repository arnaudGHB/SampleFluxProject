using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.FundingLineMediaR.Commands;
using CBS.NLoan.Repository.FundingLineP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.FundingLineMediaR.Handlers
{
    /// <summary>
    /// Handles the command to delete a Loan based on DeleteLoanCommand.
    /// </summary>
    public class DeleteFundingLineHandler : IRequestHandler<DeleteFundingLineCommand, ServiceResponse<bool>>
    {
        private readonly IFundingLineRepository _FundingLineRepository; // Repository for accessing FundingLine data.
        private readonly ILogger<DeleteFundingLineHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteFundingLineCommandHandler.
        /// </summary>
        /// <param name="FundingLineRepository">Repository for FundingLine data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteFundingLineHandler(
            IFundingLineRepository FundingLineRepository, IMapper mapper,
            ILogger<DeleteFundingLineHandler> logger, IUnitOfWork<LoanContext> uow)
        {
            _FundingLineRepository = FundingLineRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteFundingLineCommand to delete a FundingLine.
        /// </summary>
        /// <param name="request">The DeleteFundingLineCommand containing FundingLine ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteFundingLineCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the FundingLine entity with the specified ID exists
                var existingFundingLine = await _FundingLineRepository.FindAsync(request.Id);
                if (existingFundingLine == null)
                {
                    errorMessage = $"FundingLine with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingFundingLine.IsDeleted = true;
                _FundingLineRepository.Update(existingFundingLine);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting FundingLine: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
