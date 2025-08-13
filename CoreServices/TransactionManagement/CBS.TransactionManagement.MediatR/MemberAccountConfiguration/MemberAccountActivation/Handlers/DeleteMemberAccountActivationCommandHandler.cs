using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Repository.MemberAccountConfiguration;
using CBS.TransactionManagement.MemberAccountConfiguration.Commands;

namespace CBS.TransactionManagement.MemberAccountConfiguration.Handlers
{
    /// <summary>
    /// Handles the command to delete a MemberAccountActivation based on DeleteMemberAccountActivationCommand.
    /// </summary>
    public class DeleteMemberAccountActivationCommandHandler : IRequestHandler<DeleteMemberAccountActivationCommand, ServiceResponse<bool>>
    {
        private readonly IMemberAccountActivationRepository _MemberAccountActivationRepository; // Repository for accessing MemberAccountActivation data.
        private readonly ILogger<DeleteMemberAccountActivationCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteMemberAccountActivationCommandHandler.
        /// </summary>
        /// <param name="MemberAccountActivationRepository">Repository for MemberAccountActivation data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteMemberAccountActivationCommandHandler(
            IMemberAccountActivationRepository MemberAccountActivationRepository, IMapper mapper,
            ILogger<DeleteMemberAccountActivationCommandHandler> logger
, IUnitOfWork<TransactionContext> uow)
        {
            _MemberAccountActivationRepository = MemberAccountActivationRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteMemberAccountActivationCommand to delete a MemberAccountActivation.
        /// </summary>
        /// <param name="request">The DeleteMemberAccountActivationCommand containing MemberAccountActivation ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteMemberAccountActivationCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the MemberAccountActivation entity with the specified ID exists
                var existingMemberAccountActivation = await _MemberAccountActivationRepository.FindAsync(request.Id);
                if (existingMemberAccountActivation == null)
                {
                    errorMessage = $"MemberAccountActivation with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingMemberAccountActivation.IsDeleted = true;
                _MemberAccountActivationRepository.Update(existingMemberAccountActivation);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting MemberAccountActivation: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
