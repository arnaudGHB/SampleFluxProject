using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.CollateraP;
using CBS.NLoan.Data.Entity.CollateraP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanCollateralMediaR.Commands;
using CBS.NLoan.Repository.CollateralP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanCollateralMediaR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Loan.
    /// </summary>
    public class AddLoanCollateralHandler : IRequestHandler<AddLoanCollateralCommand, ServiceResponse<LoanApplicationCollateralDto>>
    {
        private readonly ILoanCollateralRepository _LoanCollateralRepository; // Repository for accessing LoanCollateral data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddLoanCollateralHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<LoanContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddLoanCollateralCommandHandler.
        /// </summary>
        /// <param name="LoanCollateralRepository">Repository for LoanCollateral data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddLoanCollateralHandler(
            ILoanCollateralRepository LoanCollateralRepository,
            IMapper mapper,
            ILogger<AddLoanCollateralHandler> logger,
            IUnitOfWork<LoanContext> uow)
        {
            _LoanCollateralRepository = LoanCollateralRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddLoanCollateralCommand to add a new LoanCollateral.
        /// </summary>
        /// <param name="request">The AddLoanCollateralCommand containing LoanCollateral data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanApplicationCollateralDto>> Handle(AddLoanCollateralCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a LoanCollateral with the same name already exists (case-insensitive)
                var existingLoanCollateral = await _LoanCollateralRepository.FindBy(c => c.LoanApplicationId == request.LoanApplicationId && c.LoanProductCollateralId==request.LoanProductCollateralId).ToListAsync();

                // If a LoanCollateral with the same name already exists, return a conflict response
                if (existingLoanCollateral .Any())
                {
                    var errorMessage = $"Collateral already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<LoanApplicationCollateralDto>.Return409(errorMessage);
                }


                // Map the AddLoanCollateralCommand to a LoanCollateral entity
                var LoanCollateralEntity = _mapper.Map<LoanApplicationCollateral>(request);
                // Convert UTC to local time and set it in the entity
                LoanCollateralEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
                LoanCollateralEntity.Id = BaseUtilities.GenerateUniqueNumber();

                // Add the new LoanCollateral entity to the repository
                _LoanCollateralRepository.Add(LoanCollateralEntity);
                await _uow.SaveAsync();

                // Map the LoanCollateral entity to LoanCollateralDto and return it with a success response
                var LoanCollateralDto = _mapper.Map<LoanApplicationCollateralDto>(LoanCollateralEntity);
                return ServiceResponse<LoanApplicationCollateralDto>.ReturnResultWith200(LoanCollateralDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving LoanCollateral: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanApplicationCollateralDto>.Return500(e);
            }
        }
    }

}
