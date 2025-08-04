using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.PeriodP;
using CBS.NLoan.Data.Entity.PeriodP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.PeriodMediaR.Commands;
using CBS.NLoan.Repository.PeriodP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.PeriodMediaR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Loan.
    /// </summary>
    public class AddPeriodHandler : IRequestHandler<AddPeriodCommand, ServiceResponse<PeriodDto>>
    {
        private readonly IPeriodRepository _PeriodRepository; // Repository for accessing Period data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddPeriodHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<LoanContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddPeriodCommandHandler.
        /// </summary>
        /// <param name="PeriodRepository">Repository for Period data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddPeriodHandler(
            IPeriodRepository PeriodRepository,
            IMapper mapper,
            ILogger<AddPeriodHandler> logger,
            IUnitOfWork<LoanContext> uow)
        {
            _PeriodRepository = PeriodRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddPeriodCommand to add a new Period.
        /// </summary>
        /// <param name="request">The AddPeriodCommand containing Period data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<PeriodDto>> Handle(AddPeriodCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a Period with the same name already exists (case-insensitive)
                //var existingPeriod = await _PeriodRepository.FindBy(c => c.Id == request.Id).FirstOrDefaultAsync();

                // If a Period with the same name already exists, return a conflict response
                //if (existingPeriod != null)
                //{
                //    var errorMessage = $"Period {request.Id} already exists.";
                //    _logger.LogError(errorMessage);
                //    return ServiceResponse<PeriodDto>.Return409(errorMessage);
                //}


                // Map the AddPeriodCommand to a Period entity
                var PeriodEntity = _mapper.Map<Period>(request);
                // Convert UTC to local time and set it in the entity
                PeriodEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
                PeriodEntity.Id = BaseUtilities.GenerateUniqueNumber();

                // Add the new Period entity to the repository
                _PeriodRepository.Add(PeriodEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<PeriodDto>.Return500();
                }

                // Map the Period entity to PeriodDto and return it with a success response
                var PeriodDto = _mapper.Map<PeriodDto>(PeriodEntity);
                return ServiceResponse<PeriodDto>.ReturnResultWith200(PeriodDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Period: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<PeriodDto>.Return500(e);
            }
        }
    }

}
