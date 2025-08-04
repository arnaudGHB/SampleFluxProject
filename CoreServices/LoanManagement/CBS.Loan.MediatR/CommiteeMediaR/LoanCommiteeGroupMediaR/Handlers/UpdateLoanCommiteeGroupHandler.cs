using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.CommiteeP;
using CBS.NLoan.Data.Entity.CommiteeP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.CommiteeMediaR.LoanCommiteeValidationCriteriaMediaR.Commands;
using CBS.NLoan.Repository.CommiteeP.LoanCommiteeGroupP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.CommiteeMediaR.LoanCommiteeGroupMediaR.Handlers
{

    /// <summary>
    /// Handles the command to update a Loan based on UpdateLoanCommand.
    /// </summary>
    public class UpdateLoanCommiteeGroupHandler : IRequestHandler<UpdateLoanCommiteeGroupCommand, ServiceResponse<LoanCommiteeGroupDto>>
    {
        private readonly ILoanCommiteeGroupRepository _LoanCommiteeGroupRepository; // Repository for accessing LoanCommiteeGroup data.
        private readonly ILogger<UpdateLoanCommiteeGroupHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateLoanCommiteeGroupCommandHandler.
        /// </summary>
        /// <param name="LoanCommiteeGroupRepository">Repository for LoanCommiteeGroup data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateLoanCommiteeGroupHandler(
            ILoanCommiteeGroupRepository LoanCommiteeGroupRepository,
            ILogger<UpdateLoanCommiteeGroupHandler> logger,
            IMapper mapper,
            IUnitOfWork<LoanContext> uow = null)
        {
            _LoanCommiteeGroupRepository = LoanCommiteeGroupRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateLoanCommiteeGroupCommand to update a LoanCommiteeGroup.
        /// </summary>
        /// <param name="request">The UpdateLoanCommiteeGroupCommand containing updated LoanCommiteeGroup data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanCommiteeGroupDto>> Handle(UpdateLoanCommiteeGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the LoanCommiteeGroup entity to be updated from the repository
                var existingLoanCommiteeGroup = await _LoanCommiteeGroupRepository.FindAsync(request.Id);

                // Check if the LoanCommiteeGroup entity exists
                if (existingLoanCommiteeGroup != null)
                {
                    // Update LoanCommiteeGroup entity properties with values from the request
                   _mapper.Map(request, existingLoanCommiteeGroup);
                    // Use the repository to update the existing LoanCommiteeGroup entity
                    _LoanCommiteeGroupRepository.Update(existingLoanCommiteeGroup);
                    await _uow.SaveAsync();
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<LoanCommiteeGroupDto>.ReturnResultWith200(_mapper.Map<LoanCommiteeGroupDto>(existingLoanCommiteeGroup));
                    _logger.LogInformation($"LoanCommiteeGroup {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the LoanCommiteeGroup entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<LoanCommiteeGroupDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating LoanCommiteeGroup: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanCommiteeGroupDto>.Return500(e);
            }
        }
    }

}
