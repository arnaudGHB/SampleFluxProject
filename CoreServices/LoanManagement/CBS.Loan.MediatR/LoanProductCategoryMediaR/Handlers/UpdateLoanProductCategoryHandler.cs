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

namespace CBS.NLoan.MediatR.CycleNameMediaR.Handlers
{

    /// <summary>
    /// Handles the command to update a Loan based on UpdateLoanCommand.
    /// </summary>
    public class UpdateLoanProductCategoryHandler : IRequestHandler<UpdateLoanProductCategoryCommand, ServiceResponse<LoanProductCategoryDto>>
    {
        private readonly ILoanProductCategoryRepository _LoanCycleRepository; // Repository for accessing LoanProductCategory data.
        private readonly ILogger<UpdateLoanProductCategoryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateLoanCycleCommandHandler.
        /// </summary>
        /// <param name="LoanCycleRepository">Repository for LoanProductCategory data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateLoanProductCategoryHandler(
            ILoanProductCategoryRepository LoanCycleRepository,
            ILogger<UpdateLoanProductCategoryHandler> logger,
            IMapper mapper,
            IUnitOfWork<LoanContext> uow = null)
        {
            _LoanCycleRepository = LoanCycleRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateLoanProductCategoryCommand to update a LoanProductCategory.
        /// </summary>
        /// <param name="request">The UpdateLoanProductCategoryCommand containing updated LoanProductCategory data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanProductCategoryDto>> Handle(UpdateLoanProductCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the LoanProductCategory entity to be updated from the repository
                var existingLoanCycle = await _LoanCycleRepository.FindAsync(request.Id);

                // Check if the LoanProductCategory entity exists
                if (existingLoanCycle != null)
                {
                    // Update LoanProductCategory entity properties with values from the request
                    var periodToUpdate = _mapper.Map(request, existingLoanCycle);
                    // Use the repository to update the existing LoanProductCategory entity
                    _LoanCycleRepository.Update(periodToUpdate);
                    await _uow.SaveAsync();
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<LoanProductCategoryDto>.ReturnResultWith200(_mapper.Map<LoanProductCategoryDto>(existingLoanCycle));
                    _logger.LogInformation($"LoanProductCategory {request.Name} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the LoanProductCategory entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Name} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<LoanProductCategoryDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating LoanProductCategory: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanProductCategoryDto>.Return500(e);
            }
        }
    }

}
