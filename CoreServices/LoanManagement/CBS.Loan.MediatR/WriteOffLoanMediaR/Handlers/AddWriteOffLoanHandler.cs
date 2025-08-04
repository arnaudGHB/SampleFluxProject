using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.WriteOffLoanP;
using CBS.NLoan.Data.Entity.WriteOffLoanP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.WriteOffLoanMediaR.Commands;
using CBS.NLoan.Repository.WriteOffLoanP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.WriteOffLoanMediaR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Loan.
    /// </summary>
    public class AddWriteOffLoanHandler : IRequestHandler<AddWriteOffLoanCommand, ServiceResponse<WriteOffLoanDto>>
    {
        private readonly IWriteOffLoanRepository _WriteOffLoanRepository; // Repository for accessing WriteOffLoan data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddWriteOffLoanHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<LoanContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddWriteOffLoanCommandHandler.
        /// </summary>
        /// <param name="WriteOffLoanRepository">Repository for WriteOffLoan data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddWriteOffLoanHandler(
            IWriteOffLoanRepository WriteOffLoanRepository,
            IMapper mapper,
            ILogger<AddWriteOffLoanHandler> logger,
            IUnitOfWork<LoanContext> uow)
        {
            _WriteOffLoanRepository = WriteOffLoanRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddWriteOffLoanCommand to add a new WriteOffLoan.
        /// </summary>
        /// <param name="request">The AddWriteOffLoanCommand containing WriteOffLoan data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<WriteOffLoanDto>> Handle(AddWriteOffLoanCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a WriteOffLoan with the same name already exists (case-insensitive)
                //var existingWriteOffLoan = await _WriteOffLoanRepository.FindBy(c => c.Id == request.Id).FirstOrDefaultAsync();

                // If a WriteOffLoan with the same name already exists, return a conflict response
                //if (existingWriteOffLoan != null)
                //{
                //    var errorMessage = $"WriteOffLoan {request.Id} already exists.";
                //    _logger.LogError(errorMessage);
                //    return ServiceResponse<WriteOffLoanDto>.Return409(errorMessage);
                //}


                // Map the AddWriteOffLoanCommand to a WriteOffLoan entity
                var WriteOffLoanEntity = _mapper.Map<WriteOffLoan>(request);
                // Convert UTC to local time and set it in the entity
                WriteOffLoanEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
                WriteOffLoanEntity.Id = BaseUtilities.GenerateUniqueNumber();

                // Add the new WriteOffLoan entity to the repository
                _WriteOffLoanRepository.Add(WriteOffLoanEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<WriteOffLoanDto>.Return500();
                }

                // Map the WriteOffLoan entity to WriteOffLoanDto and return it with a success response
                var WriteOffLoanDto = _mapper.Map<WriteOffLoanDto>(WriteOffLoanEntity);
                return ServiceResponse<WriteOffLoanDto>.ReturnResultWith200(WriteOffLoanDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving WriteOffLoan: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<WriteOffLoanDto>.Return500(e);
            }
        }
    }

}
