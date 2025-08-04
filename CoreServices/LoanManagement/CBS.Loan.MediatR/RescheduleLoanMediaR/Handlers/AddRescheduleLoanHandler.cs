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
    /// Handles the command to add a new Loan.
    /// </summary>
    public class AddRescheduleLoanHandler : IRequestHandler<AddRescheduleLoanCommand, ServiceResponse<RescheduleLoanDto>>
    {
        private readonly IRescheduleLoanRepository _RescheduleLoanRepository; // Repository for accessing RescheduleLoan data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddRescheduleLoanHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<LoanContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddRescheduleLoanCommandHandler.
        /// </summary>
        /// <param name="RescheduleLoanRepository">Repository for RescheduleLoan data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddRescheduleLoanHandler(
            IRescheduleLoanRepository RescheduleLoanRepository,
            IMapper mapper,
            ILogger<AddRescheduleLoanHandler> logger,
            IUnitOfWork<LoanContext> uow)
        {
            _RescheduleLoanRepository = RescheduleLoanRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddRescheduleLoanCommand to add a new RescheduleLoan.
        /// </summary>
        /// <param name="request">The AddRescheduleLoanCommand containing RescheduleLoan data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<RescheduleLoanDto>> Handle(AddRescheduleLoanCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a RescheduleLoan with the same name already exists (case-insensitive)
                //var existingRescheduleLoan = await _RescheduleLoanRepository.FindBy(c => c.Id == request.Id).FirstOrDefaultAsync();

                // If a RescheduleLoan with the same name already exists, return a conflict response
                //if (existingRescheduleLoan != null)
                //{
                //    var errorMessage = $"RescheduleLoan {request.Id} already exists.";
                //    _logger.LogError(errorMessage);
                //    return ServiceResponse<RescheduleLoanDto>.Return409(errorMessage);
                //}


                // Map the AddRescheduleLoanCommand to a RescheduleLoan entity
                var RescheduleLoanEntity = _mapper.Map<RescheduleLoan>(request);
                // Convert UTC to local time and set it in the entity
                RescheduleLoanEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
                RescheduleLoanEntity.Id = BaseUtilities.GenerateUniqueNumber();

                // Add the new RescheduleLoan entity to the repository
                _RescheduleLoanRepository.Add(RescheduleLoanEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<RescheduleLoanDto>.Return500();
                }

                // Map the RescheduleLoan entity to RescheduleLoanDto and return it with a success response
                var RescheduleLoanDto = _mapper.Map<RescheduleLoanDto>(RescheduleLoanEntity);
                return ServiceResponse<RescheduleLoanDto>.ReturnResultWith200(RescheduleLoanDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving RescheduleLoan: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<RescheduleLoanDto>.Return500(e);
            }
        }
    }

}
