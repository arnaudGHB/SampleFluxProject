using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanDeliquencyConfigurationMediaR.Commands;
using CBS.NLoan.Repository.LoanApplicationP.LoanDeliquencyConfigurationP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanDeliquencyConfigurationMediaR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Loan.
    /// </summary>
    public class AddLoanDeliquencyConfigurationHandler : IRequestHandler<AddLoanDeliquencyConfigurationCommand, ServiceResponse<LoanDeliquencyConfigurationDto>>
    {
        private readonly ILoanDeliquencyConfigurationRepository _LoanDeliquencyConfigurationRepository; // Repository for accessing LoanDeliquencyConfiguration data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddLoanDeliquencyConfigurationHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<LoanContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddLoanDeliquencyConfigurationCommandHandler.
        /// </summary>
        /// <param name="LoanDeliquencyConfigurationRepository">Repository for LoanDeliquencyConfiguration data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddLoanDeliquencyConfigurationHandler(
            ILoanDeliquencyConfigurationRepository LoanDeliquencyConfigurationRepository,
            IMapper mapper,
            ILogger<AddLoanDeliquencyConfigurationHandler> logger,
            IUnitOfWork<LoanContext> uow)
        {
            _LoanDeliquencyConfigurationRepository = LoanDeliquencyConfigurationRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddLoanDeliquencyConfigurationCommand to add a new LoanDeliquencyConfiguration.
        /// </summary>
        /// <param name="request">The AddLoanDeliquencyConfigurationCommand containing LoanDeliquencyConfiguration data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanDeliquencyConfigurationDto>> Handle(AddLoanDeliquencyConfigurationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a LoanDeliquencyConfiguration with the same name already exists(case -insensitive)
                var existingLoanDeliquencyConfiguration = await _LoanDeliquencyConfigurationRepository.FindBy(c => c.Name == request.Name).FirstOrDefaultAsync();

                //If a LoanDeliquencyConfiguration with the same name already exists, return a conflict response
                if (existingLoanDeliquencyConfiguration != null)
                {
                    var errorMessage = $"LoanDeliquencyConfiguration {request.Name} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<LoanDeliquencyConfigurationDto>.Return409(errorMessage);
                }
                // Map the AddLoanDeliquencyConfigurationCommand to a LoanDeliquencyConfiguration entity
                var LoanDeliquencyConfigurationEntity = _mapper.Map<LoanDeliquencyConfiguration>(request);
                LoanDeliquencyConfigurationEntity.Id = BaseUtilities.GenerateUniqueNumber();

                // Add the new LoanDeliquencyConfiguration entity to the repository
                _LoanDeliquencyConfigurationRepository.Add(LoanDeliquencyConfigurationEntity);
                await _uow.SaveAsync();
                // Map the LoanDeliquencyConfiguration entity to LoanDeliquencyConfigurationDto and return it with a success response
                var LoanDeliquencyConfigurationDto = _mapper.Map<LoanDeliquencyConfigurationDto>(LoanDeliquencyConfigurationEntity);
                return ServiceResponse<LoanDeliquencyConfigurationDto>.ReturnResultWith200(LoanDeliquencyConfigurationDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving LoanDeliquencyConfiguration: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanDeliquencyConfigurationDto>.Return500(e);
            }
        }
    }

}
