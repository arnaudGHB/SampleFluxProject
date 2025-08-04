using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data;
using CBS.NLoan.Data.Dto.CollateraP;
using CBS.NLoan.Data.Entity.CollateraP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanProductCollateralMediaR.Commands;
using CBS.NLoan.Repository.CollateralP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanProductCollateralMediaR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Loan.
    /// </summary>
    public class AddLoanProductCollateralHandler : IRequestHandler<AddLoanProductCollateralCommand, ServiceResponse<LoanProductCollateralDto>>
    {
        private readonly ILoanProductCollateralRepository _LoanProductCollateralRepository; // Repository for accessing LoanProductCollateral data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddLoanProductCollateralHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<LoanContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddLoanProductCollateralCommandHandler.
        /// </summary>
        /// <param name="LoanProductCollateralRepository">Repository for LoanProductCollateral data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddLoanProductCollateralHandler(
            ILoanProductCollateralRepository LoanProductCollateralRepository,
            IMapper mapper,
            ILogger<AddLoanProductCollateralHandler> logger,
            IUnitOfWork<LoanContext> uow)
        {
            _LoanProductCollateralRepository = LoanProductCollateralRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddLoanProductCollateralCommand to add a new LoanProductCollateral.
        /// </summary>
        /// <param name="request">The AddLoanProductCollateralCommand containing LoanProductCollateral data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanProductCollateralDto>> Handle(AddLoanProductCollateralCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a LoanProductCollateral with the same name already exists (case-insensitive)
                var existingLoanProductCollateralTag = await _LoanProductCollateralRepository.FindBy(c => c.LoanProductCollateralTag == request.LoanProductCollateralTag).Where(x => !x.IsDeleted).ToListAsync();

                //// If a LoanProductCollateral with the same name already exists, return a conflict response
                if (existingLoanProductCollateralTag.Any())
                {
                    var errorMessage = $"{request.LoanProductCollateralTag} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<LoanProductCollateralDto>.Return409(errorMessage);
                }
                // Check if a LoanProductCollateral with the same name already exists (case-insensitive)
                var existingLoanProductCollateral = await _LoanProductCollateralRepository.AllIncluding(x=>x.LoanProduct,x=>x.Collateral).Where(c => c.CollateralId == request.CollateralId && c.LoanProductId==request.LoanProductId && !c.IsDeleted).ToListAsync();

                //// If a LoanProductCollateral with the same name already exists, return a conflict response
                if (existingLoanProductCollateral.Any())
                {
                    var errorMessage = $"{existingLoanProductCollateral.FirstOrDefault().Collateral.Name} already exists for {existingLoanProductCollateral.FirstOrDefault().LoanProduct.ProductName}";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<LoanProductCollateralDto>.Return409(errorMessage);
                }


                // Map the AddLoanProductCollateralCommand to a LoanProductCollateral entity
                var LoanProductCollateralEntity = _mapper.Map<LoanProductCollateral>(request);
                // Convert UTC to local time and set it in the entity
                LoanProductCollateralEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
                LoanProductCollateralEntity.Id = BaseUtilities.GenerateUniqueNumber();

                // Add the new LoanProductCollateral entity to the repository
                _LoanProductCollateralRepository.Add(LoanProductCollateralEntity);
                await _uow.SaveAsync();

                // Map the LoanProductCollateral entity to LoanProductCollateralDto and return it with a success response
                var LoanProductCollateralDto = _mapper.Map<LoanProductCollateralDto>(LoanProductCollateralEntity);
                return ServiceResponse<LoanProductCollateralDto>.ReturnResultWith200(LoanProductCollateralDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving LoanProductCollateral: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanProductCollateralDto>.Return500(e);
            }
        }
    }

}
