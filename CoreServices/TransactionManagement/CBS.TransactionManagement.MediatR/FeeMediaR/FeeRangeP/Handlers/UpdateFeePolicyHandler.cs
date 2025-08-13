using AutoMapper;
using CBS.TransactionManagement.MediatR.FeePolicyP.Commands;
using CBS.NLoan.Repository.FeePolicyP;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data.Dto.FeeP;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Helper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.FeePolicyMediaR.FeePolicyRangeP.Handlers
{

    /// <summary>
    /// Handles the command to update a Loan based on UpdateLoanCommand.
    /// </summary>
    public class UpdateFeePolicyHandler : IRequestHandler<UpdateFeePolicyCommand, ServiceResponse<FeePolicyDto>>
    {
        private readonly IFeePolicyRepository _FeePolicyRepository; // Repository for accessing FeePolicy data.
        private readonly ILogger<UpdateFeePolicyHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateFeePolicyHandler.
        /// </summary>
        /// <param name="FeePolicyRepository">Repository for FeePolicy data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="uow">Unit of Work for database operations.</param>
        public UpdateFeePolicyHandler(
            IFeePolicyRepository FeePolicyRepository,
            ILogger<UpdateFeePolicyHandler> logger,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null)
        {
            _FeePolicyRepository = FeePolicyRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateFeePolicyCommand to update a FeePolicy.
        /// </summary>
        /// <param name="request">The UpdateFeePolicyCommand containing updated FeePolicy data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<FeePolicyDto>> Handle(UpdateFeePolicyCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the FeePolicy entity to be updated from the repository
                var existingFeePolicy = await _FeePolicyRepository.FindAsync(request.Id);
                // Check if the FeePolicy entity exists
                if (existingFeePolicy != null)
                {
                    // Update FeePolicy entity properties with values from the request
                    _mapper.Map(request, existingFeePolicy);

                    // Use the repository to update the existing FeePolicy entity
                    _FeePolicyRepository.Update(existingFeePolicy);
                    await _uow.SaveAsync();
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<FeePolicyDto>.ReturnResultWith200(_mapper.Map<FeePolicyDto>(existingFeePolicy));
                    return response;
                }
                else
                {
                    // If the FeePolicy entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<FeePolicyDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating FeePolicy: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<FeePolicyDto>.Return500(e);
            }
        }
    }

}
