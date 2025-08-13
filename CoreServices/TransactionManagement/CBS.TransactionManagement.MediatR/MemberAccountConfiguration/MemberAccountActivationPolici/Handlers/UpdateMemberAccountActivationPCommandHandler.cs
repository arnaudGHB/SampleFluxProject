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
using CBS.TransactionManagement.Repository.MemberAccountConfiguration;
using CBS.TransactionManagement.MediatR.MemberAccountConfiguration.MemberAccountActivationPolici.Commands;
using CBS.TransactionManagement.Data.Dto.MemberAccountConfiguration;

namespace CBS.TransactionManagement.MediatR.MemberAccountConfiguration.MemberAccountActivationPolici.Handlers
{

    /// <summary>
    /// Handles the command to update a MemberAccountActivationPolicy based on UpdateMemberAccountActivationPolicyCommand.
    /// </summary>
    public class UpdateMemberAccountActivationPolicyCommandHandler : IRequestHandler<UpdateMemberAccountActivationPolicyCommand, ServiceResponse<MemberAccountActivationPolicyDto>>
    {
        private readonly IMemberAccountActivationPolicyRepository _MemberAccountActivationPolicyRepository; // Repository for accessing MemberAccountActivationPolicy data.
        private readonly ILogger<UpdateMemberAccountActivationPolicyCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateMemberAccountActivationPolicyCommandHandler.
        /// </summary>
        /// <param name="MemberAccountActivationPolicyRepository">Repository for MemberAccountActivationPolicy data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateMemberAccountActivationPolicyCommandHandler(
            IMemberAccountActivationPolicyRepository MemberAccountActivationPolicyRepository,
            ILogger<UpdateMemberAccountActivationPolicyCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null)
        {
            _MemberAccountActivationPolicyRepository = MemberAccountActivationPolicyRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateMemberAccountActivationPolicyCommand to update a MemberAccountActivationPolicy.
        /// </summary>
        /// <param name="request">The UpdateMemberAccountActivationPolicyCommand containing updated MemberAccountActivationPolicy data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<MemberAccountActivationPolicyDto>> Handle(UpdateMemberAccountActivationPolicyCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the MemberAccountActivationPolicy entity to be updated from the repository
                var existingMemberAccountActivationPolicy = await _MemberAccountActivationPolicyRepository.FindAsync(request.Id);

                // Check if the MemberAccountActivationPolicy entity exists
                if (existingMemberAccountActivationPolicy != null)
                {
                    _mapper.Map(request, existingMemberAccountActivationPolicy);
                    existingMemberAccountActivationPolicy.ModifiedDate = DateTime.Now;
                    _MemberAccountActivationPolicyRepository.Update(existingMemberAccountActivationPolicy);
                    await _uow.SaveAsync();
                    // Prepare the response and return a successful response with 200 Status code
                    var response = ServiceResponse<MemberAccountActivationPolicyDto>.ReturnResultWith200(_mapper.Map<MemberAccountActivationPolicyDto>(existingMemberAccountActivationPolicy));
                    _logger.LogInformation($"MemberAccountActivationPolicy {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the MemberAccountActivationPolicy entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<MemberAccountActivationPolicyDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating MemberAccountActivationPolicy: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<MemberAccountActivationPolicyDto>.Return500(e);
            }
        }
    }

}
