using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanDeliquencyConfigurationMediaR.Commands;
using CBS.NLoan.Repository.LoanApplicationP.LoanDeliquencyConfigurationP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanDeliquencyConfigurationMediaR.Handlers
{
    /// <summary>
    /// Handles the command to delete a Loan based on DeleteLoanCommand.
    /// </summary>
    public class DeleteLoanDeliquencyConfigurationHandler : IRequestHandler<DeleteLoanDeliquencyConfigurationCommand, ServiceResponse<bool>>
    {
        private readonly ILoanDeliquencyConfigurationRepository _LoanDeliquencyConfigurationRepository; // Repository for accessing LoanDeliquencyConfiguration data.
        private readonly ILogger<DeleteLoanDeliquencyConfigurationHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteLoanDeliquencyConfigurationCommandHandler.
        /// </summary>
        /// <param name="LoanDeliquencyConfigurationRepository">Repository for LoanDeliquencyConfiguration data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteLoanDeliquencyConfigurationHandler(
            ILoanDeliquencyConfigurationRepository LoanDeliquencyConfigurationRepository, IMapper mapper,
            ILogger<DeleteLoanDeliquencyConfigurationHandler> logger, IUnitOfWork<LoanContext> uow)
        {
            _LoanDeliquencyConfigurationRepository = LoanDeliquencyConfigurationRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteLoanDeliquencyConfigurationCommand to delete a LoanDeliquencyConfiguration.
        /// </summary>
        /// <param name="request">The DeleteLoanDeliquencyConfigurationCommand containing LoanDeliquencyConfiguration ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteLoanDeliquencyConfigurationCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the LoanDeliquencyConfiguration entity with the specified ID exists
                var existingLoanDeliquencyConfiguration = await _LoanDeliquencyConfigurationRepository.FindAsync(request.Id);
                if (existingLoanDeliquencyConfiguration == null)
                {
                    errorMessage = $"LoanDeliquencyConfiguration with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingLoanDeliquencyConfiguration.IsDeleted = true;
                _LoanDeliquencyConfigurationRepository.Update(existingLoanDeliquencyConfiguration);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting LoanDeliquencyConfiguration: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
