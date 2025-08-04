using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.CommiteeP;
using CBS.NLoan.Data.Entity.CommiteeP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.CommiteeMediaR.LoanCommeteeMemberMediaR.Commands;
using CBS.NLoan.Repository.CommiteeP.LoanCommeteeMemberP;
using CBS.NLoan.Repository.CommiteeP.LoanCommiteeGroupP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CBS.NLoan.MediatR.CommiteeMediaR.LoanCommeteeMemberMediaR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Loan.
    /// </summary>
    public class AddLoanCommeteeMemberCommandHandler : IRequestHandler<AddLoanCommeteeMemberCommand, ServiceResponse<LoanCommiteeMemberDto>>
    {
        private readonly ILoanCommeteeMemberRepository _LoanCommeteeMemberRepository; // Repository for accessing LoanCommeteeMember data.
        private readonly ILoanCommiteeGroupRepository _LoanCommiteeGroupRepository; // Repository for accessing LoanCommeteeMember data.

        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddLoanCommeteeMemberCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<LoanContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddLoanCommeteeMemberCommandHandler.
        /// </summary>
        /// <param name="LoanCommeteeMemberRepository">Repository for LoanCommeteeMember data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddLoanCommeteeMemberCommandHandler(
            ILoanCommeteeMemberRepository LoanCommeteeMemberRepository,
            IMapper mapper,
            ILogger<AddLoanCommeteeMemberCommandHandler> logger,
            IUnitOfWork<LoanContext> uow,
            ILoanCommiteeGroupRepository loanCommiteeGroupRepository)
        {
            _LoanCommeteeMemberRepository = LoanCommeteeMemberRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _LoanCommiteeGroupRepository = loanCommiteeGroupRepository;
        }

        /// <summary>
        /// Handles the AddLoanCommeteeMemberCommand to add a new LoanCommeteeMember.
        /// </summary>
        /// <param name="request">The AddLoanCommeteeMemberCommand containing LoanCommeteeMember data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanCommiteeMemberDto>> Handle(AddLoanCommeteeMemberCommand request, CancellationToken cancellationToken)
        {
            var errorMessage = $"User already exists.";

            try
            {
                // Check if a LoanCommeteeMember with the same name already exists (case-insensitive)
                var existingLoanCommeteeMember = await _LoanCommeteeMemberRepository.FindBy(c => c.UserId == request.UserId && c.LoanCommiteeGroupId==request.LoanCommiteeGroupId && c.IsDeleted==false).ToListAsync();

                // If a LoanCommeteeMember with the same name already exists, return a conflict response
                if (existingLoanCommeteeMember.Any())
                {
                    _logger.LogError(errorMessage);
                    return ServiceResponse<LoanCommiteeMemberDto>.Return409(errorMessage);
                }
                var loanCommiteeMembers = await _LoanCommeteeMemberRepository.FindBy(c => c.LoanCommiteeGroupId == request.LoanCommiteeGroupId).ToListAsync();

                var loanCommiteeGroup = await _LoanCommiteeGroupRepository.FindAsync(request.LoanCommiteeGroupId);

                // If a LoanCommeteeMember with the same name already exists, return a conflict response
                if (loanCommiteeMembers.Count()<=loanCommiteeGroup.NumberOfMembers)
                {
                    // Map the AddLoanCommeteeMemberCommand to a LoanCommeteeMember entity
                    var LoanCommeteeMemberEntity = _mapper.Map<LoanCommiteeMember>(request);
                    // Convert UTC to local time and set it in the entity
                    LoanCommeteeMemberEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
                    LoanCommeteeMemberEntity.Id = BaseUtilities.GenerateUniqueNumber();

                    // Add the new LoanCommeteeMember entity to the repository
                    _LoanCommeteeMemberRepository.Add(LoanCommeteeMemberEntity);
                    await _uow.SaveAsync();

                    // Map the LoanCommeteeMember entity to LoanCommeteeMemberDto and return it with a success response
                    var LoanCommeteeMemberDto = _mapper.Map<LoanCommiteeMemberDto>(LoanCommeteeMemberEntity);
                    return ServiceResponse<LoanCommiteeMemberDto>.ReturnResultWith200(LoanCommeteeMemberDto);

                }
                 errorMessage = $"You can't add more members in to {loanCommiteeGroup.Name} commitee.";
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanCommiteeMemberDto>.Return403(errorMessage);



            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                 errorMessage = $"Error occurred while saving LoanCommeteeMember: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanCommiteeMemberDto>.Return500(e);
            }
        }
    }

}
