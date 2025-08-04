using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Commands;
using CBS.BankMGT.Repository;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Common.UnitOfWork;
using CBS.BankMGT.Domain;

namespace CBS.BankMGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to delete a EconomicActivity based on DeleteEconomicActivityCommand.
    /// </summary>
    public class DeleteEconomicActivityCommandHandler : IRequestHandler<DeleteEconomicActivityCommand, ServiceResponse<bool>>
    {
        private readonly IEconomicActivityRepository _EconomicActivityRepository; // Repository for accessing EconomicActivity data.
        private readonly ILogger<DeleteEconomicActivityCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteEconomicActivityCommandHandler.
        /// </summary>
        /// <param name="EconomicActivityRepository">Repository for EconomicActivity data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteEconomicActivityCommandHandler(
            IEconomicActivityRepository EconomicActivityRepository, IMapper mapper,
            ILogger<DeleteEconomicActivityCommandHandler> logger
, IUnitOfWork<POSContext> uow)
        {
            _EconomicActivityRepository = EconomicActivityRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteEconomicActivityCommand to delete a EconomicActivity.
        /// </summary>
        /// <param name="request">The DeleteEconomicActivityCommand containing EconomicActivity ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteEconomicActivityCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the EconomicActivity entity with the specified ID exists
                var existingEconomicActivity = await _EconomicActivityRepository.FindAsync(request.Id);
                if (existingEconomicActivity == null)
                {
                    errorMessage = $"EconomicActivity with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }
                existingEconomicActivity.IsDeleted = true;
                _EconomicActivityRepository.Update(existingEconomicActivity);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting EconomicActivity: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
