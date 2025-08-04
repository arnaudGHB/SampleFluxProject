using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.FeeP;
using CBS.NLoan.Data.Entity.FeeP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanApplicationFeeMediaR.LoanApplicationFeeP.Commands;
using CBS.NLoan.Repository.LoanApplicationFeeP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanApplicationFeeMediaR.LoanApplicationFeeP.Handlers
{

    /// <summary>
    /// Handles the command to update a Loan based on UpdateLoanCommand.
    /// </summary>
    public class UpdateLoanApplicationFeeHandler : IRequestHandler<UpdateLoanApplicationFeeCommand, ServiceResponse<LoanApplicationFeeDto>>
    {
        private readonly ILoanApplicationFeeRepository _loanApplicationFeeRepository; // Repository for accessing LoanApplicationFee data.
        private readonly ILogger<UpdateLoanApplicationFeeHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateLoanApplicationFeeHandler.
        /// </summary>
        /// <param name="loanApplicationFeeRepository">Repository for LoanApplicationFee data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="uow">Unit of work for database operations.</param>
        public UpdateLoanApplicationFeeHandler(
            ILoanApplicationFeeRepository loanApplicationFeeRepository,
            ILogger<UpdateLoanApplicationFeeHandler> logger,
            IMapper mapper,
            IUnitOfWork<LoanContext> uow = null)
        {
            _loanApplicationFeeRepository = loanApplicationFeeRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateLoanApplicationFeeCommand to update a LoanApplicationFee.
        /// </summary>
        /// <param name="request">The UpdateLoanApplicationFeeCommand containing updated LoanApplicationFee data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanApplicationFeeDto>> Handle(UpdateLoanApplicationFeeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the LoanApplicationFee entity to be updated from the repository
                var existingLoanApplicationFee = await _loanApplicationFeeRepository.FindAsync(request.Id);

                // Check if the LoanApplicationFee entity exists
                if (existingLoanApplicationFee != null)
                {
                    // Update LoanApplicationFee entity properties with values from the request
                    _mapper.Map(request, existingLoanApplicationFee);

                    // Use the repository to update the existing LoanApplicationFee entity
                    _loanApplicationFeeRepository.Update(existingLoanApplicationFee);

                    // Save changes to the database
                    await _uow.SaveAsync();

                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<LoanApplicationFeeDto>.ReturnResultWith200(_mapper.Map<LoanApplicationFeeDto>(existingLoanApplicationFee));
                    return response;
                }
                else
                {
                    // If the LoanApplicationFee entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"Loan application fee with ID {request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<LoanApplicationFeeDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating loan application fee: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanApplicationFeeDto>.Return500(e);
            }
        }
    }


}
