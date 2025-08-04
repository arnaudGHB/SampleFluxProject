using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Data;
using static CBS.TransactionManagement.Helper.BaseUtilities;

namespace CBS.TransactionManagement.Handlers
{

    /// <summary>
    /// Handles the command to update a WithdrawalLimits based on UpdateWithdrawalLimitsCommand.
    /// </summary>
    public class UpdateWithdrawalLimitsCommandHandler : IRequestHandler<UpdateWithdrawalLimitsCommand, ServiceResponse<WithdrawalParameterDto>>
    {
        private readonly IWithdrawalLimitsRepository _WithdrawalLimitsRepository; // Repository for accessing WithdrawalLimits data.
        private readonly ILogger<UpdateWithdrawalLimitsCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateWithdrawalLimitsCommandHandler.
        /// </summary>
        /// <param name="WithdrawalLimitsRepository">Repository for WithdrawalLimits data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateWithdrawalLimitsCommandHandler(
            IWithdrawalLimitsRepository WithdrawalLimitsRepository,
            ILogger<UpdateWithdrawalLimitsCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null)
        {
            _WithdrawalLimitsRepository = WithdrawalLimitsRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateWithdrawalLimitsCommand to update a WithdrawalLimits.
        /// </summary>
        /// <param name="request">The UpdateWithdrawalLimitsCommand containing updated WithdrawalLimits data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<WithdrawalParameterDto>> Handle(UpdateWithdrawalLimitsCommand request, CancellationToken cancellationToken)
        {
            try
            {

                // Validate the sum of shares using the helper class
                ShareValidator.Validate(SharingWithPartner.TRUST_SOFT_CREDIT_SHARING.ToString(),request.CamCCULShare, request.FluxAndPTMShare, request.HeadOfficeShare, request.SourceBrachOfficeShare, request.DestinationBranchOfficeShare);


                var existingWithdrawalLimits = await _WithdrawalLimitsRepository.FindAsync(request.Id);

                if (existingWithdrawalLimits == null)
                {
                    return ServiceResponse<WithdrawalParameterDto>.Return404($"{request.Id} was not found to be updated.");
                }

                // Use AutoMapper to map properties from the request to the existingWithdrawalLimits
                _mapper.Map(request, existingWithdrawalLimits);

                // Set the modified date
                existingWithdrawalLimits.ModifiedDate = DateTime.Now;

                // Use the repository to update the existing WithdrawalLimits entity
                _WithdrawalLimitsRepository.Update(existingWithdrawalLimits);

                // Save changes
                await _uow.SaveAsync();

                // Prepare the response and return a successful response with 200 Status code
                var response = ServiceResponse<WithdrawalParameterDto>.ReturnResultWith200(_mapper.Map<WithdrawalParameterDto>(existingWithdrawalLimits), $"WithdrawalLimits {request.Id} was successfully updated.");
                _logger.LogInformation($"WithdrawalLimits {request.Id} was successfully updated.");
                return response;
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating WithdrawalLimits: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<WithdrawalParameterDto>.Return500(e);
            }
        }
    }

}
