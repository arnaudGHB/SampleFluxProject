using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.LoanPurposeP;
using CBS.NLoan.Data.Entity.LoanPurposeP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanPurposeMediaR.Commands;
using CBS.NLoan.Repository.LoanPurposeP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanPurposeMediaR.Handlers
{

    /// <summary>
    /// Handles the command to update a Loan based on UpdateLoanCommand.
    /// </summary>
    public class UpdateLoanPurposeHandler : IRequestHandler<UpdateLoanPurposeCommand, ServiceResponse<LoanPurposeDto>>
    {
        private readonly ILoanPurposeRepository _LoanPurposeRepository; // Repository for accessing LoanPurpose data.
        private readonly ILogger<UpdateLoanPurposeHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateLoanPurposeCommandHandler.
        /// </summary>
        /// <param name="LoanPurposeRepository">Repository for LoanPurpose data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateLoanPurposeHandler(
            ILoanPurposeRepository LoanPurposeRepository,
            ILogger<UpdateLoanPurposeHandler> logger,
            IMapper mapper,
            IUnitOfWork<LoanContext> uow = null)
        {
            _LoanPurposeRepository = LoanPurposeRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateLoanPurposeCommand to update a LoanPurpose.
        /// </summary>
        /// <param name="request">The UpdateLoanPurposeCommand containing updated LoanPurpose data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanPurposeDto>> Handle(UpdateLoanPurposeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the LoanPurpose entity to be updated from the repository
                var existingLoanPurpose = await _LoanPurposeRepository.FindAsync(request.Id);

                // Check if the LoanPurpose entity exists
                if (existingLoanPurpose != null)
                {
                    // Update LoanPurpose entity properties with values from the request
                    var LoanPurposeToUpdate = _mapper.Map(request, existingLoanPurpose);
                    // Use the repository to update the existing LoanPurpose entity
                    _LoanPurposeRepository.Update(LoanPurposeToUpdate);
                    await _uow.SaveAsync();
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<LoanPurposeDto>.ReturnResultWith200(_mapper.Map<LoanPurposeDto>(existingLoanPurpose));
                    _logger.LogInformation($"LoanPurpose {request.PurposeName} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the LoanPurpose entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.PurposeName} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<LoanPurposeDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating LoanPurpose: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanPurposeDto>.Return500(e);
            }
        }
    }

}
