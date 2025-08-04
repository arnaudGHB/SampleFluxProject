using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.CommiteeP;
using CBS.NLoan.Data.Entity.CommiteeP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.CommiteeMediaR.LoanCommiteeValidationMediaR.Commands;
using CBS.NLoan.Repository.CommiteeP.LoanCommiteeValidationP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.CommiteeMediaR.LoanCommiteeValidationMediaR.Handlers
{

    /// <summary>
    /// Handles the command to update a Loan based on UpdateLoanCommand.
    /// </summary>
    public class UpdateLoanCommiteeValidationHistoryHandler : IRequestHandler<UpdateLoanCommiteeValidationHistoryCommand, ServiceResponse<LoanCommiteeValidationHistoryDto>>
    {
        private readonly ILoanCommiteeValidationHistoryRepository _LoanCommiteeValidationRepository; // Repository for accessing LoanCommiteeValidation data.
        private readonly ILogger<UpdateLoanCommiteeValidationHistoryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateLoanCommiteeValidationCommandHandler.
        /// </summary>
        /// <param name="LoanCommiteeValidationRepository">Repository for LoanCommiteeValidation data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateLoanCommiteeValidationHistoryHandler(
            ILoanCommiteeValidationHistoryRepository LoanCommiteeValidationRepository,
            ILogger<UpdateLoanCommiteeValidationHistoryHandler> logger,
            IMapper mapper,
            IUnitOfWork<LoanContext> uow = null)
        {
            _LoanCommiteeValidationRepository = LoanCommiteeValidationRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateLoanCommiteeValidationCommand to update a LoanCommiteeValidation.
        /// </summary>
        /// <param name="request">The UpdateLoanCommiteeValidationCommand containing updated LoanCommiteeValidation data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanCommiteeValidationHistoryDto>> Handle(UpdateLoanCommiteeValidationHistoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the LoanCommiteeValidation entity to be updated from the repository
                var existingLoanCommiteeValidation = await _LoanCommiteeValidationRepository.FindAsync(request.Id);

                // Check if the LoanCommiteeValidation entity exists
                if (existingLoanCommiteeValidation != null)
                {
                    // Update LoanCommiteeValidation entity properties with values from the request
                    _mapper.Map(request, existingLoanCommiteeValidation);
                    // Use the repository to update the existing LoanCommiteeValidation entity
                    _LoanCommiteeValidationRepository.Update(existingLoanCommiteeValidation);
                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<LoanCommiteeValidationHistoryDto>.Return500();
                    }
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<LoanCommiteeValidationHistoryDto>.ReturnResultWith200(_mapper.Map<LoanCommiteeValidationHistoryDto>(existingLoanCommiteeValidation));
                    _logger.LogInformation($"LoanCommiteeValidation {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the LoanCommiteeValidation entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<LoanCommiteeValidationHistoryDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating LoanCommiteeValidation: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanCommiteeValidationHistoryDto>.Return500(e);
            }
        }
    }

}
