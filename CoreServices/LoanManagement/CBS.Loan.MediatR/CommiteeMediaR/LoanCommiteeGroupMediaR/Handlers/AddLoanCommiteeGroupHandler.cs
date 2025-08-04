using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.CommiteeP;
using CBS.NLoan.Data.Entity.CommiteeP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.CommiteeMediaR.LoanCommiteeValidationCriteriaMediaR.Commands;
using CBS.NLoan.Repository.CommiteeP.LoanCommiteeGroupP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.CommiteeMediaR.LoanCommiteeGroupMediaR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Loan.
    /// </summary>
    public class AddLoanCommiteeGroupHandler : IRequestHandler<AddLoanCommiteeGroupCommand, ServiceResponse<LoanCommiteeGroupDto>>
    {
        private readonly ILoanCommiteeGroupRepository _LoanCommiteeGroupRepository; // Repository for accessing LoanCommiteeGroup data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddLoanCommiteeGroupHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<LoanContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddLoanCommiteeGroupCommandHandler.
        /// </summary>
        /// <param name="LoanCommiteeGroupRepository">Repository for LoanCommiteeGroup data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddLoanCommiteeGroupHandler(
            ILoanCommiteeGroupRepository LoanCommiteeGroupRepository,
            IMapper mapper,
            ILogger<AddLoanCommiteeGroupHandler> logger,
            IUnitOfWork<LoanContext> uow)
        {
            _LoanCommiteeGroupRepository = LoanCommiteeGroupRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddLoanCommiteeGroupCommand to add a new LoanCommiteeGroup.
        /// </summary>
        /// <param name="request">The AddLoanCommiteeGroupCommand containing LoanCommiteeGroup data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanCommiteeGroupDto>> Handle(AddLoanCommiteeGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a LoanCommiteeGroup with the same name already exists (case-insensitive)
                var existingLoanCommiteeGroup = await _LoanCommiteeGroupRepository.FindBy(c => c.Name == request.Name).FirstOrDefaultAsync();

                // If a LoanCommiteeGroup with the same name already exists, return a conflict response
                if (existingLoanCommiteeGroup != null)
                {
                    var errorMessage = $"LoanCommiteeGroup {request.Name} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<LoanCommiteeGroupDto>.Return409(errorMessage);
                }


                // Map the AddLoanCommiteeGroupCommand to a LoanCommiteeGroup entity
                var LoanCommiteeGroupEntity = _mapper.Map<LoanCommiteeGroup>(request);
                // Convert UTC to local time and set it in the entity
                LoanCommiteeGroupEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
                LoanCommiteeGroupEntity.Id = BaseUtilities.GenerateUniqueNumber();

                // Add the new LoanCommiteeGroup entity to the repository
                _LoanCommiteeGroupRepository.Add(LoanCommiteeGroupEntity);
                await _uow.SaveAsync();

                // Map the LoanCommiteeGroup entity to LoanCommiteeGroupDto and return it with a success response
                var LoanCommiteeGroupDto = _mapper.Map<LoanCommiteeGroupDto>(LoanCommiteeGroupEntity);
                return ServiceResponse<LoanCommiteeGroupDto>.ReturnResultWith200(LoanCommiteeGroupDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving LoanCommiteeGroup: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanCommiteeGroupDto>.Return500(e);
            }
        }
    }

}
