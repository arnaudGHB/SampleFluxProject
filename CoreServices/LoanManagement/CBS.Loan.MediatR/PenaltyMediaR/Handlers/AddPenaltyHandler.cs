using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.PenaltyP;
using CBS.NLoan.Data.Entity.PenaltyP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.PenaltyMediaR.Commands;
using CBS.NLoan.Repository.PenaltyP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.PenaltyMediaR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Loan.
    /// </summary>
    public class AddPenaltyHandler : IRequestHandler<AddPenaltyCommand, ServiceResponse<PenaltyDto>>
    {
        private readonly IPenaltyRepository _PenaltyRepository; // Repository for accessing Penalty data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddPenaltyHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<LoanContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddPenaltyCommandHandler.
        /// </summary>
        /// <param name="PenaltyRepository">Repository for Penalty data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddPenaltyHandler(
            IPenaltyRepository PenaltyRepository,
            IMapper mapper,
            ILogger<AddPenaltyHandler> logger,
            IUnitOfWork<LoanContext> uow)
        {
            _PenaltyRepository = PenaltyRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddPenaltyCommand to add a new Penalty.
        /// </summary>
        /// <param name="request">The AddPenaltyCommand containing Penalty data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<PenaltyDto>> Handle(AddPenaltyCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a Penalty with the same name already exists (case-insensitive)
                var existingPenalty = await _PenaltyRepository.FindBy(c => c.PenaltyType == request.PenaltyType && c.LoanProductId==request.LoanProductId).Include(x=>x.LoanProduct).ToListAsync();

                // If a Penalty with the same name already exists, return a conflict response
                if (existingPenalty.Any())
                {
                    var errorMessage = $"Penalty type {request.PenaltyType} already exists for product {existingPenalty.FirstOrDefault().LoanProduct.ProductName}";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<PenaltyDto>.Return409(errorMessage);
                }

                // Check if a Penalty with the same name already exists (case-insensitive)
                var existingPenaltyName = await _PenaltyRepository.FindBy(c => c.PenaltyName == request.PenaltyName).ToListAsync();

                // If a Penalty with the same name already exists, return a conflict response
                if (existingPenaltyName.Any())
                {
                    var errorMessage = $"Penalty name {request.PenaltyName} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<PenaltyDto>.Return409(errorMessage);
                }


                // Map the AddPenaltyCommand to a Penalty entity
                var PenaltyEntity = _mapper.Map<Penalty>(request);
                // Convert UTC to local time and set it in the entity
                PenaltyEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
                PenaltyEntity.Id = BaseUtilities.GenerateUniqueNumber();

                // Add the new Penalty entity to the repository
                _PenaltyRepository.Add(PenaltyEntity);
                await _uow.SaveAsync();

                // Map the Penalty entity to PenaltyDto and return it with a success response
                var PenaltyDto = _mapper.Map<PenaltyDto>(PenaltyEntity);
                return ServiceResponse<PenaltyDto>.ReturnResultWith200(PenaltyDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Penalty: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<PenaltyDto>.Return500(e);
            }
        }
    }

}
