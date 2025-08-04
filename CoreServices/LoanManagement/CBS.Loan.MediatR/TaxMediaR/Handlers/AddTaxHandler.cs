using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.TaxP;
using CBS.NLoan.Data.Entity.TaxP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.TaxMediaR.Commands;
using CBS.NLoan.Repository.TaxP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.TaxMediaR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Loan.
    /// </summary>
    public class AddTaxHandler : IRequestHandler<AddTaxCommand, ServiceResponse<TaxDto>>
    {
        private readonly ITaxRepository _TaxRepository; // Repository for accessing Tax data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddTaxHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<LoanContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddTaxCommandHandler.
        /// </summary>
        /// <param name="TaxRepository">Repository for Tax data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddTaxHandler(
            ITaxRepository TaxRepository,
            IMapper mapper,
            ILogger<AddTaxHandler> logger,
            IUnitOfWork<LoanContext> uow)
        {
            _TaxRepository = TaxRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddTaxCommand to add a new Tax.
        /// </summary>
        /// <param name="request">The AddTaxCommand containing Tax data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<TaxDto>> Handle(AddTaxCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a Tax with the same name already exists (case-insensitive)
                var existingTax = await _TaxRepository.FindBy(c => c.Name == request.Name && !c.IsDeleted).ToListAsync();

                //If a Tax with the same name already exists, return a conflict response
                if (existingTax.Any())
                {
                    var errorMessage = $"{request.Name} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<TaxDto>.Return409(errorMessage);
                }


                // Map the AddTaxCommand to a Tax entity
                var TaxEntity = _mapper.Map<Tax>(request);
                // Convert UTC to local time and set it in the entity
                TaxEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
                TaxEntity.Id = BaseUtilities.GenerateUniqueNumber();

                // Add the new Tax entity to the repository
                _TaxRepository.Add(TaxEntity);
                await _uow.SaveAsync();

                // Map the Tax entity to TaxDto and return it with a success response
                var TaxDto = _mapper.Map<TaxDto>(TaxEntity);
                return ServiceResponse<TaxDto>.ReturnResultWith200(TaxDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Tax: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<TaxDto>.Return500(e);
            }
        }
    }

}
