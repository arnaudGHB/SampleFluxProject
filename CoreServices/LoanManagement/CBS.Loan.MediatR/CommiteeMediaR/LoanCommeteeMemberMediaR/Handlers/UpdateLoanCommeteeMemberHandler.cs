using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.CommiteeP;
using CBS.NLoan.Data.Entity.CommiteeP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.CommiteeMediaR.LoanCommeteeMemberMediaR.Commands;
using CBS.NLoan.Repository.CommiteeP.LoanCommeteeMemberP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.CommiteeMediaR.LoanCommeteeMemberMediaR.Handlers
{

    /// <summary>
    /// Handles the command to update a Loan based on UpdateLoanCommand.
    /// </summary>
    public class UpdateLoanCommeteeMemberCommandHandler : IRequestHandler<UpdateLoanCommeteeMemberCommand, ServiceResponse<LoanCommiteeMemberDto>>
    {
        private readonly ILoanCommeteeMemberRepository _LoanCommeteeMemberRepository; // Repository for accessing LoanCommeteeMember data.
        private readonly ILogger<UpdateLoanCommeteeMemberCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateLoanCommeteeMemberCommandHandler.
        /// </summary>
        /// <param name="LoanCommeteeMemberRepository">Repository for LoanCommeteeMember data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateLoanCommeteeMemberCommandHandler(
            ILoanCommeteeMemberRepository LoanCommeteeMemberRepository,
            ILogger<UpdateLoanCommeteeMemberCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<LoanContext> uow = null)
        {
            _LoanCommeteeMemberRepository = LoanCommeteeMemberRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateLoanCommeteeMemberCommand to update a LoanCommeteeMember.
        /// </summary>
        /// <param name="request">The UpdateLoanCommeteeMemberCommand containing updated LoanCommeteeMember data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanCommiteeMemberDto>> Handle(UpdateLoanCommeteeMemberCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the LoanCommeteeMember entity to be updated from the repository
                var existingLoanCommeteeMember = await _LoanCommeteeMemberRepository.FindAsync(request.Id);

                // Check if the LoanCommeteeMember entity exists
                if (existingLoanCommeteeMember != null)
                {
                    // Update LoanCommeteeMember entity properties with values from the request
                    _mapper.Map(request, existingLoanCommeteeMember);
                    // Use the repository to update the existing LoanCommeteeMember entity
                    _LoanCommeteeMemberRepository.Update(existingLoanCommeteeMember);
                    await _uow.SaveAsync();                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<LoanCommiteeMemberDto>.ReturnResultWith200(_mapper.Map<LoanCommiteeMemberDto>(existingLoanCommeteeMember));
                    _logger.LogInformation($"LoanCommeteeMember was successfully updated.");
                    return response;
                }
                else
                {
                    // If the LoanCommeteeMember entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<LoanCommiteeMemberDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating LoanCommeteeMember: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanCommiteeMemberDto>.Return500(e);
            }
        }
    }

}
