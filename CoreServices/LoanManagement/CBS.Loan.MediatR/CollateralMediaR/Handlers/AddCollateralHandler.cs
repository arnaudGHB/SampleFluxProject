using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.CollateraP;
using CBS.NLoan.Data.Entity.CollateraP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.CollateralMediaR.Commands;
using CBS.NLoan.Repository.CollateralP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.CollateralMediaR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Loan.
    /// </summary>
    public class AddCollateralHandler : IRequestHandler<AddCollateralCommand, ServiceResponse<CollateralDto>>
    {
        private readonly ICollateralRepository _CollateralRepository; // Repository for accessing Collateral data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddCollateralHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<LoanContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddCollateralCommandHandler.
        /// </summary>
        /// <param name="CollateralRepository">Repository for Collateral data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddCollateralHandler(
            ICollateralRepository CollateralRepository,
            IMapper mapper,
            ILogger<AddCollateralHandler> logger,
            IUnitOfWork<LoanContext> uow)
        {
            _CollateralRepository = CollateralRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddCollateralCommand to add a new Collateral.
        /// </summary>
        /// <param name="request">The AddCollateralCommand containing Collateral data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CollateralDto>> Handle(AddCollateralCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a Collateral with the same name already exists (case-insensitive)
                var existingCollateral = await _CollateralRepository.FindBy(c => c.Name == request.Name).Where(x => !x.IsDeleted).ToListAsync();

                // If a Collateral with the same name already exists, return a conflict response
                if (existingCollateral.Any())
                {
                    var errorMessage = $"{request.Name} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<CollateralDto>.Return409(errorMessage);
                }


                // Map the AddCollateralCommand to a Collateral entity
                var CollateralEntity = _mapper.Map<Collateral>(request);
                // Convert UTC to local time and set it in the entity
                CollateralEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
                CollateralEntity.Id = BaseUtilities.GenerateUniqueNumber();

                // Add the new Collateral entity to the repository
                _CollateralRepository.Add(CollateralEntity);
                await _uow.SaveAsync();

                // Map the Collateral entity to CollateralDto and return it with a success response
                var CollateralDto = _mapper.Map<CollateralDto>(CollateralEntity);
                return ServiceResponse<CollateralDto>.ReturnResultWith200(CollateralDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Collateral: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<CollateralDto>.Return500(e);
            }
        }
    }

}
