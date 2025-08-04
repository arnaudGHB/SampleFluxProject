using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Repository.MemberAccountConfiguration;
using CBS.TransactionManagement.MediatR.MemberAccountConfiguration.MemberAccountActivationPolici.Commands;

namespace CBS.TransactionManagement.MediatR.MemberAccountConfiguration.MemberAccountActivationPolici.Handlers
{
    /// <summary>
    /// Handles the command to delete a MemberAccountActivationPolicy based on DeleteMemberAccountActivationPolicyCommand.
    /// </summary>
    public class DeleteMemberAccountActivationPolicyCommandHandler : IRequestHandler<DeleteMemberAccountActivationPolicyCommand, ServiceResponse<bool>>
    {
        private readonly IMemberAccountActivationPolicyRepository _MemberAccountActivationPolicyRepository; // Repository for accessing MemberAccountActivationPolicy data.
        private readonly ILogger<DeleteMemberAccountActivationPolicyCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteMemberAccountActivationPolicyCommandHandler.
        /// </summary>
        /// <param name="MemberAccountActivationPolicyRepository">Repository for MemberAccountActivationPolicy data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteMemberAccountActivationPolicyCommandHandler(
            IMemberAccountActivationPolicyRepository MemberAccountActivationPolicyRepository, IMapper mapper,
            ILogger<DeleteMemberAccountActivationPolicyCommandHandler> logger
, IUnitOfWork<TransactionContext> uow)
        {
            _MemberAccountActivationPolicyRepository = MemberAccountActivationPolicyRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteMemberAccountActivationPolicyCommand to delete a MemberAccountActivationPolicy.
        /// </summary>
        /// <param name="request">The DeleteMemberAccountActivationPolicyCommand containing MemberAccountActivationPolicy ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteMemberAccountActivationPolicyCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the MemberAccountActivationPolicy entity with the specified ID exists
                var existingMemberAccountActivationPolicy = await _MemberAccountActivationPolicyRepository.FindAsync(request.Id);
                if (existingMemberAccountActivationPolicy == null)
                {
                    errorMessage = $"MemberAccountActivationPolicy with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingMemberAccountActivationPolicy.IsDeleted = true;
                _MemberAccountActivationPolicyRepository.Update(existingMemberAccountActivationPolicy);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting MemberAccountActivationPolicy: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
