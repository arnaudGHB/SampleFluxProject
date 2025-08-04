using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to delete a AccountClass based on DeleteAccountClassCommand.
    /// </summary>
    public class DeleteAccountClassCommandHandler : IRequestHandler<DeleteAccountClassCommand, ServiceResponse<bool>>
    {
        private readonly IAccountClassRepository _AccountClassRepository; // Repository for accessing AccountClass data.
        private readonly ILogger<DeleteOperationEventCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteAccountClassCommandHandler.
        /// </summary>
        /// <param name="AccountClassRepository">Repository for AccountClass data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteAccountClassCommandHandler(
            IAccountClassRepository AccountClassRepository, IMapper mapper,
            ILogger<DeleteOperationEventCommandHandler> logger
, IUnitOfWork<POSContext> uow)
        {
            _AccountClassRepository = AccountClassRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteAccountClassCommand to delete a AccountClass.
        /// </summary>
        /// <param name="request">The DeleteAccountClassCommand containing AccountClass ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteAccountClassCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the AccountClass entity with the specified ID exists
                var existingAccountClass = await _AccountClassRepository.FindAsync(request.Id);
                if (existingAccountClass == null)
                {
                    errorMessage = $"AccountClass with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingAccountClass.IsDeleted = true;

                _AccountClassRepository.Update(existingAccountClass);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting AccountClass: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }
}