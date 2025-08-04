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
    public class DeleteChartOfAccountManagementPositionCommandHandler : IRequestHandler<DeleteChartOfAccountManagementPositionCommand, ServiceResponse<bool>>
    {
        private readonly IChartOfAccountManagementPositionRepository _chartOfAccountManagementPositionRepository; // Repository for accessing AccountClass data.
        private readonly ILogger<DeleteChartOfAccountManagementPositionCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteChartOfAccountManagementPositionCommandHandler.
        /// </summary>
        /// <param name="AccountClassRepository">Repository for AccountClass data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteChartOfAccountManagementPositionCommandHandler(
            IChartOfAccountManagementPositionRepository AccountClassRepository, IMapper mapper,
            ILogger<DeleteChartOfAccountManagementPositionCommandHandler> logger
, IUnitOfWork<POSContext> uow)
        {
            _chartOfAccountManagementPositionRepository = AccountClassRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteChartOfAccountManagementPositionCommand  to delete a AccountClass.
        /// </summary>
        /// <param name="request">The DeleteChartOfAccountManagementPositionCommand containing AccountClass ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteChartOfAccountManagementPositionCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the AccountClass entity with the specified ID exists
                var existingAccountClass = await _chartOfAccountManagementPositionRepository.FindAsync(request.Id);
                if (existingAccountClass == null)
                {
                    errorMessage = $"DeleteChartOfAccountManagementPositionCommand with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingAccountClass.IsDeleted = true;

                _chartOfAccountManagementPositionRepository.Update(existingAccountClass);
                await _uow.SaveAsync();
             
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