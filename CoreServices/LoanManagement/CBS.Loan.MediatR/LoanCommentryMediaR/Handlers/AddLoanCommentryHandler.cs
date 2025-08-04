using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanCommentryMediaR.Commands;
using CBS.NLoan.Repository.LoanApplicationP.LoanCommentryP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanCommentryMediaR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Loan.
    /// </summary>
    public class AddLoanCommentryHandler : IRequestHandler<AddLoanCommentryCommand, ServiceResponse<LoanCommentryDto>>
    {
        private readonly ILoanCommentryRepository _LoanCommentryRepository; // Repository for accessing LoanCommentry data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddLoanCommentryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<LoanContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddLoanCommentryCommandHandler.
        /// </summary>
        /// <param name="LoanCommentryRepository">Repository for LoanCommentry data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddLoanCommentryHandler(
            ILoanCommentryRepository LoanCommentryRepository,
            IMapper mapper,
            ILogger<AddLoanCommentryHandler> logger,
            IUnitOfWork<LoanContext> uow)
        {
            _LoanCommentryRepository = LoanCommentryRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddLoanCommentryCommand to add a new LoanCommentry.
        /// </summary>
        /// <param name="request">The AddLoanCommentryCommand containing LoanCommentry data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanCommentryDto>> Handle(AddLoanCommentryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a LoanCommentry with the same name already exists (case-insensitive)
                //var existingLoanCommentry = await _LoanCommentryRepository.FindBy(c => c.Id == request.Id).FirstOrDefaultAsync();

                // If a LoanCommentry with the same name already exists, return a conflict response
                //if (existingLoanCommentry != null)
                //{
                //    var errorMessage = $"LoanCommentry {request.Id} already exists.";
                //    _logger.LogError(errorMessage);
                //    return ServiceResponse<LoanCommentryDto>.Return409(errorMessage);
                //}


                // Map the AddLoanCommentryCommand to a LoanCommentry entity
                var LoanCommentryEntity = _mapper.Map<LoanCommentry>(request);
                // Convert UTC to local time and set it in the entity
                LoanCommentryEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
                LoanCommentryEntity.Id = BaseUtilities.GenerateUniqueNumber();

                // Add the new LoanCommentry entity to the repository
                _LoanCommentryRepository.Add(LoanCommentryEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<LoanCommentryDto>.Return500();
                }

                // Map the LoanCommentry entity to LoanCommentryDto and return it with a success response
                var LoanCommentryDto = _mapper.Map<LoanCommentryDto>(LoanCommentryEntity);
                return ServiceResponse<LoanCommentryDto>.ReturnResultWith200(LoanCommentryDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving LoanCommentry: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanCommentryDto>.Return500(e);
            }
        }
    }

}
