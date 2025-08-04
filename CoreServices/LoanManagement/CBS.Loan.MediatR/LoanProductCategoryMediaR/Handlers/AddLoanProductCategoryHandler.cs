using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.CycleNameMediaR.Commands;
using CBS.NLoan.Repository.LoanApplicationP.LoanCycleP;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Data.Entity;

namespace CBS.NLoan.MediatR.CycleNameMediaR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Loan.
    /// </summary>
    public class AddLoanProductCategoryHandler : IRequestHandler<AddLoanProductCategoryCommand, ServiceResponse<LoanProductCategoryDto>>
    {
        private readonly ILoanProductCategoryRepository _LoanCycleRepository; // Repository for accessing LoanProductCategory data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddLoanProductCategoryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<LoanContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddLoanCycleCommandHandler.
        /// </summary>
        /// <param name="LoanCycleRepository">Repository for LoanProductCategory data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddLoanProductCategoryHandler(
            ILoanProductCategoryRepository LoanCycleRepository,
            IMapper mapper,
            ILogger<AddLoanProductCategoryHandler> logger,
            IUnitOfWork<LoanContext> uow)
        {
            _LoanCycleRepository = LoanCycleRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddLoanProductCategoryCommand to add a new LoanProductCategory.
        /// </summary>
        /// <param name="request">The AddLoanProductCategoryCommand containing LoanProductCategory data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanProductCategoryDto>> Handle(AddLoanProductCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a LoanProductCategory with the same name already exists (case-insensitive)
                var existingLoanCycle = _LoanCycleRepository.FindBy(c => c.Name == request.Name).ToList();

                if (existingLoanCycle.Any())
                {
                    var errorMessage = $"Loan product, {request.Name} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<LoanProductCategoryDto>.Return409(errorMessage);
                }



                // Map the AddLoanProductCategoryCommand to a LoanProductCategory entity
                var LoanCycleEntity = _mapper.Map<LoanProductCategory>(request);
                // Convert UTC to local time and set it in the entity
                LoanCycleEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
                LoanCycleEntity.Id = BaseUtilities.GenerateUniqueNumber();

                // Add the new LoanProductCategory entity to the repository
                _LoanCycleRepository.Add(LoanCycleEntity);
                await _uow.SaveAsync();

                // Map the LoanProductCategory entity to LoanProductCategoryDto and return it with a success response
                var LoanCycleDto = _mapper.Map<LoanProductCategoryDto>(LoanCycleEntity);
                return ServiceResponse<LoanProductCategoryDto>.ReturnResultWith200(LoanCycleDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving LoanProductCategory: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanProductCategoryDto>.Return500(e);
            }
        }
    }

}
