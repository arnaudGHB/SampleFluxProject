using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanApplicationMediaR.Validations;
using CBS.NLoan.MediatR.LoanProductMediaR.Commands;
using CBS.NLoan.Repository.LoanApplicationP.LoanCommentryP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanProductMediaR.Handlers
{
    /// <summary>
    /// Handles the command to update a Loan based on UpdateLoanCommand.
    /// </summary>
    public class UpdateLoanProductRepaymentCycleCommandHandler : IRequestHandler<UpdateLoanProductRepaymentCycleCommand, ServiceResponse<LoanProductRepaymentCycleDto>>
    {
        private readonly ILoanProductRepaymentCycleRepository _LoanProductRepaymentCycleRepository; // Repository for accessing LoanProductRepaymentCycle data.
        private readonly ILogger<UpdateLoanProductRepaymentCycleCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateLoanProductRepaymentCycleCommandHandler.
        /// </summary>
        /// <param name="LoanProductRepaymentCycleRepository">Repository for LoanProductRepaymentCycle data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateLoanProductRepaymentCycleCommandHandler(
            ILoanProductRepaymentCycleRepository LoanProductRepaymentCycleRepository,
            ILogger<UpdateLoanProductRepaymentCycleCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<LoanContext> uow = null)
        {
            _LoanProductRepaymentCycleRepository = LoanProductRepaymentCycleRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateLoanProductRepaymentCycleCommand to update a LoanProductRepaymentCycle.
        /// </summary>
        /// <param name="request">The UpdateLoanProductRepaymentCycleCommand containing updated LoanProductRepaymentCycle data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanProductRepaymentCycleDto>> Handle(UpdateLoanProductRepaymentCycleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the LoanProductRepaymentCycle entity to be updated from the repository
                var existingLoanProductRepaymentCycle = await _LoanProductRepaymentCycleRepository.FindAsync(request.Id);

                // Check if the LoanProductRepaymentCycle entity exists
                if (existingLoanProductRepaymentCycle != null)
                {
                    // Map properties from existingLoanProductRepaymentCycle to request
                    _mapper.Map(request, existingLoanProductRepaymentCycle);

                    // Use the repository to update the existing LoanProductRepaymentCycle entity
                    _LoanProductRepaymentCycleRepository.Update(existingLoanProductRepaymentCycle);

                    await _uow.SaveAsync();

                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<LoanProductRepaymentCycleDto>.ReturnResultWith200(_mapper.Map<LoanProductRepaymentCycleDto>(existingLoanProductRepaymentCycle));
                    _logger.LogInformation($"LoanProductRepaymentCycle {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the LoanProductRepaymentCycle entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<LoanProductRepaymentCycleDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating LoanProductRepaymentCycle: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanProductRepaymentCycleDto>.Return500(e);
            }
        }
    }

}
