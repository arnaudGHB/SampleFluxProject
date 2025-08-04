using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.CollateraP;
using CBS.NLoan.Data.Entity.CollateraP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanCollateralMediaR.Commands;
using CBS.NLoan.Repository.CollateralP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanCollateralMediaR.Handlers
{

    /// <summary>
    /// Handles the command to update a Loan based on UpdateLoanCommand.
    /// </summary>
    public class UpdateLoanCollateralHandler : IRequestHandler<UpdateLoanCollateralCommand, ServiceResponse<LoanApplicationCollateralDto>>
    {
        private readonly ILoanCollateralRepository _LoanCollateralRepository; // Repository for accessing LoanCollateral data.
        private readonly ILogger<UpdateLoanCollateralHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateLoanCollateralCommandHandler.
        /// </summary>
        /// <param name="LoanCollateralRepository">Repository for LoanCollateral data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateLoanCollateralHandler(
            ILoanCollateralRepository LoanCollateralRepository,
            ILogger<UpdateLoanCollateralHandler> logger,
            IMapper mapper,
            IUnitOfWork<LoanContext> uow = null)
        {
            _LoanCollateralRepository = LoanCollateralRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateLoanCollateralCommand to update a LoanCollateral.
        /// </summary>
        /// <param name="request">The UpdateLoanCollateralCommand containing updated LoanCollateral data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanApplicationCollateralDto>> Handle(UpdateLoanCollateralCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the LoanCollateral entity to be updated from the repository
                var existingLoanCollateral = await _LoanCollateralRepository.FindAsync(request.Id);

                // Check if the LoanCollateral entity exists
                if (existingLoanCollateral != null)
                {
                    // Update LoanCollateral entity properties with values from the request
                   _mapper.Map(request, existingLoanCollateral);
                    // Use the repository to update the existing LoanCollateral entity
                    _LoanCollateralRepository.Update(existingLoanCollateral);
                    await _uow.SaveAsync();
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<LoanApplicationCollateralDto>.ReturnResultWith200(_mapper.Map<LoanApplicationCollateralDto>(existingLoanCollateral));
                    _logger.LogInformation($"LoanCollateral {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the LoanCollateral entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<LoanApplicationCollateralDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating LoanCollateral: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanApplicationCollateralDto>.Return500(e);
            }
        }
    }

}
