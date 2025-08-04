using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanGuarantorMediaR.Commands;
using CBS.NLoan.Repository.LoanApplicationP.LoanGuarantorP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanGuarantorMediaR.Handlers
{

    /// <summary>
    /// Handles the command to update a Loan based on UpdateLoanCommand.
    /// </summary>
    public class UpdateLoanGuarantorHandler : IRequestHandler<UpdateLoanGuarantorCommand, ServiceResponse<LoanGuarantorDto>>
    {
        private readonly ILoanGuarantorRepository _LoanGuarantorRepository; // Repository for accessing LoanGuarantor data.
        private readonly ILogger<UpdateLoanGuarantorHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateLoanGuarantorCommandHandler.
        /// </summary>
        /// <param name="LoanGuarantorRepository">Repository for LoanGuarantor data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateLoanGuarantorHandler(
            ILoanGuarantorRepository LoanGuarantorRepository,
            ILogger<UpdateLoanGuarantorHandler> logger,
            IMapper mapper,
            IUnitOfWork<LoanContext> uow = null)
        {
            _LoanGuarantorRepository = LoanGuarantorRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateLoanGuarantorCommand to update a LoanGuarantor.
        /// </summary>
        /// <param name="request">The UpdateLoanGuarantorCommand containing updated LoanGuarantor data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanGuarantorDto>> Handle(UpdateLoanGuarantorCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the LoanGuarantor entity to be updated from the repository
                var existingLoanGuarantor = await _LoanGuarantorRepository.FindAsync(request.Id);

                // Check if the LoanGuarantor entity exists
                if (existingLoanGuarantor != null)
                {
                    // Update LoanGuarantor entity properties with values from the request
                    _mapper.Map(request, existingLoanGuarantor);
                    // Use the repository to update the existing LoanGuarantor entity
                    _LoanGuarantorRepository.Update(existingLoanGuarantor);
                    await _uow.SaveAsync();
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<LoanGuarantorDto>.ReturnResultWith200(_mapper.Map<LoanGuarantorDto>(existingLoanGuarantor));
                    _logger.LogInformation($"LoanGuarantor {request.GuarantorName} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the LoanGuarantor entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.GuarantorName} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<LoanGuarantorDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating LoanGuarantor: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanGuarantorDto>.Return500(e);
            }
        }
    }

}
