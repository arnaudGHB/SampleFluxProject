using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.CollateraP;
using CBS.NLoan.Data.Entity.CollateraP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.CollateralMediaR.Commands;
using CBS.NLoan.Repository.CollateralP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.CollateralMediaR.Handlers
{

    /// <summary>
    /// Handles the command to update a Loan based on UpdateLoanCommand.
    /// </summary>
    public class UpdateCollateralHandler : IRequestHandler<UpdateCollateralCommand, ServiceResponse<CollateralDto>>
    {
        private readonly ICollateralRepository _CollateralRepository; // Repository for accessing Collateral data.
        private readonly ILogger<UpdateCollateralHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateCollateralCommandHandler.
        /// </summary>
        /// <param name="CollateralRepository">Repository for Collateral data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateCollateralHandler(
            ICollateralRepository CollateralRepository,
            ILogger<UpdateCollateralHandler> logger,
            IMapper mapper,
            IUnitOfWork<LoanContext> uow = null)
        {
            _CollateralRepository = CollateralRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateCollateralCommand to update a Collateral.
        /// </summary>
        /// <param name="request">The UpdateCollateralCommand containing updated Collateral data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CollateralDto>> Handle(UpdateCollateralCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the Collateral entity to be updated from the repository
                var existingCollateral = await _CollateralRepository.FindAsync(request.Id);

                // Check if the Collateral entity exists
                if (existingCollateral != null)
                {
                    // Update Collateral entity properties with values from the request
                   _mapper.Map(request, existingCollateral);
                    // Use the repository to update the existing Collateral entity
                    _CollateralRepository.Update(existingCollateral);
                    await _uow.SaveAsync();
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<CollateralDto>.ReturnResultWith200(_mapper.Map<CollateralDto>(existingCollateral));
                    _logger.LogInformation($"Collateral {request.Name} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the Collateral entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Name} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<CollateralDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Collateral: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<CollateralDto>.Return500(e);
            }
        }
    }

}
