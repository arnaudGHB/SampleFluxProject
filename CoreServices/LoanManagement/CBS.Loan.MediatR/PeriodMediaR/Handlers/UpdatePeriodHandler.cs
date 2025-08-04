using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.PeriodP;
using CBS.NLoan.Data.Entity.PeriodP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.PeriodMediaR.Commands;
using CBS.NLoan.Repository.PeriodP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.PeriodMediaR.Handlers
{

    /// <summary>
    /// Handles the command to update a Loan based on UpdateLoanCommand.
    /// </summary>
    public class UpdatePeriodHandler : IRequestHandler<UpdatePeriodCommand, ServiceResponse<PeriodDto>>
    {
        private readonly IPeriodRepository _PeriodRepository; // Repository for accessing Period data.
        private readonly ILogger<UpdatePeriodHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdatePeriodCommandHandler.
        /// </summary>
        /// <param name="PeriodRepository">Repository for Period data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdatePeriodHandler(
            IPeriodRepository PeriodRepository,
            ILogger<UpdatePeriodHandler> logger,
            IMapper mapper,
            IUnitOfWork<LoanContext> uow = null)
        {
            _PeriodRepository = PeriodRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdatePeriodCommand to update a Period.
        /// </summary>
        /// <param name="request">The UpdatePeriodCommand containing updated Period data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<PeriodDto>> Handle(UpdatePeriodCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the Period entity to be updated from the repository
                var existingPeriod = await _PeriodRepository.FindAsync(request.Id);

                // Check if the Period entity exists
                if (existingPeriod != null)
                {
                    // Update Period entity properties with values from the request
                    var periodToUpdate = _mapper.Map<Period>(request);
                    // Use the repository to update the existing Period entity
                    _PeriodRepository.Update(periodToUpdate);
                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<PeriodDto>.Return500();
                    }
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<PeriodDto>.ReturnResultWith200(_mapper.Map<PeriodDto>(existingPeriod));
                    _logger.LogInformation($"Period {request.Name} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the Period entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Name} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<PeriodDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Period: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<PeriodDto>.Return500(e);
            }
        }
    }

}
