using AutoMapper;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
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
    /// Handles the command to delete a ChartOfAccount based on DeleteChartOfAccountCommand.
    /// </summary>
    public class DeleteChartOfAccountCommandHandler : IRequestHandler<DeleteChartOfAccountCommand, ServiceResponse<bool>>
    {
        private readonly IChartOfAccountRepository _ChartOfAccountRepository; // Repository for accessing Account data.
        private readonly ILogger<DeleteChartOfAccountCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteChartOfAccountCommandHandler.
        /// </summary>
        /// <param name="ChartOfAccountRepository">Repository for Account data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteChartOfAccountCommandHandler(
            IChartOfAccountRepository ChartOfAccountRepository, IMapper mapper,
            ILogger<DeleteChartOfAccountCommandHandler> logger
, IUnitOfWork<POSContext> uow)
        {
            _ChartOfAccountRepository = ChartOfAccountRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteChartOfAccountCommand to delete a Account.
        /// </summary>
        /// <param name="request">The DeleteChartOfAccountCommand containing Account ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteChartOfAccountCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the Account entity with the specified ID exists
                var existingAccount = await _ChartOfAccountRepository.FindAsync(request.Id);
                if (existingAccount == null)
                {
                    errorMessage = $"Account with Id {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingAccount.IsDeleted = true;

                _ChartOfAccountRepository.Update(existingAccount);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting Account: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }
}