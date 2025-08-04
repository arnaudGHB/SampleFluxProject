using AutoMapper;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data.Dto.SavingProductFeeP;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.SavingProductFeeP.Commands;
using CBS.TransactionManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.MediatR.SavingProductFeeP.Handlers
{

    /// <summary>
    /// Handles the command to update a Loan based on UpdateLoanCommand.
    /// </summary>
    public class UpdateSavingProductFeeHandler : IRequestHandler<UpdateSavingProductFeeCommand, ServiceResponse<SavingProductFeeDto>>
    {
        private readonly ISavingProductFeeRepository _SavingProductFeeRepository; // Repository for accessing SavingProductFee data.
        private readonly ILogger<UpdateSavingProductFeeHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateSavingProductFeeHandler.
        /// </summary>
        /// <param name="SavingProductFeeRepository">Repository for SavingProductFee data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="uow">Unit of work for database operations.</param>
        public UpdateSavingProductFeeHandler(
            ISavingProductFeeRepository SavingProductFeeRepository,
            ILogger<UpdateSavingProductFeeHandler> logger,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null)
        {
            _SavingProductFeeRepository = SavingProductFeeRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateSavingProductFeeCommand to update a SavingProductFee.
        /// </summary>
        /// <param name="request">The UpdateSavingProductFeeCommand containing updated SavingProductFee data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<SavingProductFeeDto>> Handle(UpdateSavingProductFeeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the SavingProductFee entity to be updated from the repository
                var existingSavingProductFee = await _SavingProductFeeRepository.FindAsync(request.Id);

                // Check if the SavingProductFee entity exists
                if (existingSavingProductFee != null)
                {
                    // Update SavingProductFee entity properties with values from the request
                    _mapper.Map(request, existingSavingProductFee);

                    // Use the repository to update the existing SavingProductFee entity
                    _SavingProductFeeRepository.Update(existingSavingProductFee);

                    // Save changes to the database
                    await _uow.SaveAsync();

                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<SavingProductFeeDto>.ReturnResultWith200(_mapper.Map<SavingProductFeeDto>(existingSavingProductFee));
                    return response;
                }
                else
                {
                    // If the SavingProductFee entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"Loan application fee with ID {request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<SavingProductFeeDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating loan application fee: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<SavingProductFeeDto>.Return500(e);
            }
        }
    }


}
