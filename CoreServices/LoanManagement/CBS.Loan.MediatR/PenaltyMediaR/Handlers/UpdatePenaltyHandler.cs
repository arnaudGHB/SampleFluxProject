using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.PenaltyP;
using CBS.NLoan.Data.Entity.PenaltyP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.PenaltyMediaR.Commands;
using CBS.NLoan.Repository.PenaltyP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.PenaltyMediaR.Handlers
{

    /// <summary>
    /// Handles the command to update a Loan based on UpdateLoanCommand.
    /// </summary>
    public class UpdatePenaltyHandler : IRequestHandler<UpdatePenaltyCommand, ServiceResponse<PenaltyDto>>
    {
        private readonly IPenaltyRepository _PenaltyRepository; // Repository for accessing Penalty data.
        private readonly ILogger<UpdatePenaltyHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdatePenaltyCommandHandler.
        /// </summary>
        /// <param name="PenaltyRepository">Repository for Penalty data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdatePenaltyHandler(
            IPenaltyRepository PenaltyRepository,
            ILogger<UpdatePenaltyHandler> logger,
            IMapper mapper,
            IUnitOfWork<LoanContext> uow = null)
        {
            _PenaltyRepository = PenaltyRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdatePenaltyCommand to update a Penalty.
        /// </summary>
        /// <param name="request">The UpdatePenaltyCommand containing updated Penalty data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<PenaltyDto>> Handle(UpdatePenaltyCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the Penalty entity to be updated from the repository
                var existingPenalty = await _PenaltyRepository.FindAsync(request.Id);

                // Check if the Penalty entity exists
                if (existingPenalty != null)
                {
                    // Update Penalty entity properties with values from the request
                    _mapper.Map(request, existingPenalty);
                    // Use the repository to update the existing Penalty entity
                    _PenaltyRepository.Update(existingPenalty);
                    await _uow.SaveAsync();
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<PenaltyDto>.ReturnResultWith200(_mapper.Map<PenaltyDto>(existingPenalty));
                    _logger.LogInformation($"Penalty {request.PenaltyName} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the Penalty entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.PenaltyName} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<PenaltyDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Penalty: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<PenaltyDto>.Return500(e);
            }
        }
    }

}
