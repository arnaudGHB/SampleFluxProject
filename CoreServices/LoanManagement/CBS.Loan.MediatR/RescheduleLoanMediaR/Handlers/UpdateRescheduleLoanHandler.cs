using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.RescheduleLoanMediaR.Commands;
using CBS.NLoan.Repository.LoanApplicationP.RescheduleLoanP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.RescheduleLoanMediaR.Handlers
{

    /// <summary>
    /// Handles the command to update a Loan based on UpdateLoanCommand.
    /// </summary>
    public class UpdateRescheduleLoanHandler : IRequestHandler<UpdateRescheduleLoanCommand, ServiceResponse<RescheduleLoanDto>>
    {
        private readonly IRescheduleLoanRepository _RescheduleLoanRepository; // Repository for accessing RescheduleLoan data.
        private readonly ILogger<UpdateRescheduleLoanHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateRescheduleLoanCommandHandler.
        /// </summary>
        /// <param name="RescheduleLoanRepository">Repository for RescheduleLoan data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateRescheduleLoanHandler(
            IRescheduleLoanRepository RescheduleLoanRepository,
            ILogger<UpdateRescheduleLoanHandler> logger,
            IMapper mapper,
            IUnitOfWork<LoanContext> uow = null)
        {
            _RescheduleLoanRepository = RescheduleLoanRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateRescheduleLoanCommand to update a RescheduleLoan.
        /// </summary>
        /// <param name="request">The UpdateRescheduleLoanCommand containing updated RescheduleLoan data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<RescheduleLoanDto>> Handle(UpdateRescheduleLoanCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the RescheduleLoan entity to be updated from the repository
                var existingRescheduleLoan = await _RescheduleLoanRepository.FindAsync(request.Id);

                // Check if the RescheduleLoan entity exists
                if (existingRescheduleLoan != null)
                {
                    // Update RescheduleLoan entity properties with values from the request
                    var RescheduleLoanToUpdate = _mapper.Map<RescheduleLoan>(request);
                    // Use the repository to update the existing RescheduleLoan entity
                    _RescheduleLoanRepository.Update(RescheduleLoanToUpdate);
                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<RescheduleLoanDto>.Return500();
                    }
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<RescheduleLoanDto>.ReturnResultWith200(_mapper.Map<RescheduleLoanDto>(existingRescheduleLoan));
                    _logger.LogInformation($"RescheduleLoan {request.Name} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the RescheduleLoan entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Name} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<RescheduleLoanDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating RescheduleLoan: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<RescheduleLoanDto>.Return500(e);
            }
        }
    }

}
