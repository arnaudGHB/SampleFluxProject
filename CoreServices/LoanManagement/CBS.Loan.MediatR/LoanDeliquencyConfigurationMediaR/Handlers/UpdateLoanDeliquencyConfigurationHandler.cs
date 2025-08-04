using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanDeliquencyConfigurationMediaR.Commands;
using CBS.NLoan.Repository.LoanApplicationP.LoanDeliquencyConfigurationP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanDeliquencyConfigurationMediaR.Handlers
{

    /// <summary>
    /// Handles the command to update a Loan based on UpdateLoanCommand.
    /// </summary>
    public class UpdateLoanDeliquencyConfigurationHandler : IRequestHandler<UpdateLoanDeliquencyConfigurationCommand, ServiceResponse<LoanDeliquencyConfigurationDto>>
    {
        private readonly ILoanDeliquencyConfigurationRepository _LoanDeliquencyConfigurationRepository; // Repository for accessing LoanDeliquencyConfiguration data.
        private readonly ILogger<UpdateLoanDeliquencyConfigurationHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateLoanDeliquencyConfigurationCommandHandler.
        /// </summary>
        /// <param name="LoanDeliquencyConfigurationRepository">Repository for LoanDeliquencyConfiguration data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateLoanDeliquencyConfigurationHandler(
            ILoanDeliquencyConfigurationRepository LoanDeliquencyConfigurationRepository,
            ILogger<UpdateLoanDeliquencyConfigurationHandler> logger,
            IMapper mapper,
            IUnitOfWork<LoanContext> uow = null)
        {
            _LoanDeliquencyConfigurationRepository = LoanDeliquencyConfigurationRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateLoanDeliquencyConfigurationCommand to update a LoanDeliquencyConfiguration.
        /// </summary>
        /// <param name="request">The UpdateLoanDeliquencyConfigurationCommand containing updated LoanDeliquencyConfiguration data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanDeliquencyConfigurationDto>> Handle(UpdateLoanDeliquencyConfigurationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the LoanDeliquencyConfiguration entity to be updated from the repository
                var existingLoanDeliquencyConfiguration = await _LoanDeliquencyConfigurationRepository.FindAsync(request.Id);

                // Check if the LoanDeliquencyConfiguration entity exists
                if (existingLoanDeliquencyConfiguration != null)
                {
                    // Update LoanDeliquencyConfiguration entity properties with values from the request
                    var LoanDeliquencyConfigurationToUpdate = _mapper.Map(request, existingLoanDeliquencyConfiguration);
                    // Use the repository to update the existing LoanDeliquencyConfiguration entity
                    _LoanDeliquencyConfigurationRepository.Update(LoanDeliquencyConfigurationToUpdate);
                    await _uow.SaveAsync();
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<LoanDeliquencyConfigurationDto>.ReturnResultWith200(_mapper.Map<LoanDeliquencyConfigurationDto>(existingLoanDeliquencyConfiguration));
                    _logger.LogInformation($"LoanDeliquencyConfiguration {request.Name} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the LoanDeliquencyConfiguration entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Name} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<LoanDeliquencyConfigurationDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating LoanDeliquencyConfiguration: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanDeliquencyConfigurationDto>.Return500(e);
            }
        }
    }

}
