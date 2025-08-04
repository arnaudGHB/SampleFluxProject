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

namespace CBS.TransactionManagement.Handlers
{

    /// <summary>
    /// Handles the command to update a TransferLimits based on UpdateTransferLimitsCommand.
    /// </summary>
    public class UpdateTransferLimitsCommandHandler : IRequestHandler<UpdateTransferLimitsCommand, ServiceResponse<TransferParameterDto>>
    {
        private readonly ITransferLimitsRepository _TransferLimitsRepository; // Repository for accessing TransferLimits data.
        private readonly ILogger<UpdateTransferLimitsCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateTransferLimitsCommandHandler.
        /// </summary>
        /// <param name="TransferLimitsRepository">Repository for TransferLimits data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateTransferLimitsCommandHandler(
            ITransferLimitsRepository TransferLimitsRepository,
            ILogger<UpdateTransferLimitsCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null)
        {
            _TransferLimitsRepository = TransferLimitsRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateTransferLimitsCommand to update a TransferLimits.
        /// </summary>
        /// <param name="request">The UpdateTransferLimitsCommand containing updated TransferLimits data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<TransferParameterDto>> Handle(UpdateTransferLimitsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate the sum of shares using the helper class
                ShareValidator.Validate(SharingWithPartner.TRUST_SOFT_CREDIT_SHARING.ToString(),request.CamCCULShare, request.FluxAndPTMShare, request.HeadOfficeShare, request.SourceBrachOfficeShare, request.DestinationBranchOfficeShare);

                // Validate the sum of shares using the helper class
                ShareValidator.Validate(SharingWithPartner.C_MONWY_SHARING.ToString(),request.CamCCULShareCMoney, request.FluxAndPTMShareCMoney, request.HeadOfficeShareCMoney, request.SourceBrachOfficeShareCMoney, request.DestinationBranchOfficeShareCMoney);

                // Retrieve the TransferLimits entity to be updated from the repository
                var existingTransferLimits = await _TransferLimitsRepository.FindAsync(request.Id);

                // Check if the TransferLimits entity exists
                if (existingTransferLimits != null)
                {
                    existingTransferLimits = _mapper.Map(request, existingTransferLimits);
                    _TransferLimitsRepository.Update(existingTransferLimits);
                    await _uow.SaveAsync();
                    // Prepare the response and return a successful response with 200 Status code
                    var response = ServiceResponse<TransferParameterDto>.ReturnResultWith200(_mapper.Map<TransferParameterDto>(existingTransferLimits));
                    _logger.LogInformation($"TransferLimits {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the TransferLimits entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<TransferParameterDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating TransferLimits: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<TransferParameterDto>.Return500(e);
            }
        }
    }

}
