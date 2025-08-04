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
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanProductCollateralMediaR.Handlers
{

    /// <summary>
    /// Handles the command to update a Loan based on UpdateLoanCommand.
    /// </summary>
    public class UpdateLoanProductCollateralHandler : IRequestHandler<UpdateLoanProductCollateralCommand, ServiceResponse<LoanProductCollateralDto>>
    {
        private readonly ILoanProductCollateralRepository _LoanProductCollateralRepository; // Repository for accessing LoanProductCollateral data.
        private readonly ILogger<UpdateLoanProductCollateralHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateLoanProductCollateralCommandHandler.
        /// </summary>
        /// <param name="LoanProductCollateralRepository">Repository for LoanProductCollateral data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateLoanProductCollateralHandler(
            ILoanProductCollateralRepository LoanProductCollateralRepository,
            ILogger<UpdateLoanProductCollateralHandler> logger,
            IMapper mapper,
            IUnitOfWork<LoanContext> uow = null)
        {
            _LoanProductCollateralRepository = LoanProductCollateralRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateLoanProductCollateralCommand to update a LoanProductCollateral.
        /// </summary>
        /// <param name="request">The UpdateLoanProductCollateralCommand containing updated LoanProductCollateral data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanProductCollateralDto>> Handle(UpdateLoanProductCollateralCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the LoanProductCollateral entity to be updated from the repository
                var existingLoanProductCollateral = await _LoanProductCollateralRepository.FindAsync(request.Id);

                // Check if the LoanProductCollateral entity exists
                if (existingLoanProductCollateral != null)
                {
                    // Update LoanProductCollateral entity properties with values from the request
                    _mapper.Map(request, existingLoanProductCollateral); 
                    _LoanProductCollateralRepository.Update(existingLoanProductCollateral);
                    await _uow.SaveAsync();
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<LoanProductCollateralDto>.ReturnResultWith200(_mapper.Map<LoanProductCollateralDto>(existingLoanProductCollateral));
                    _logger.LogInformation($"{request.LoanProductCollateralTag} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the LoanProductCollateral entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<LoanProductCollateralDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating LoanProductCollateral: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanProductCollateralDto>.Return500(e);
            }
        }
    }

}
