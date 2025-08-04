using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Commands;
using CBS.BankMGT.Repository;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Common.UnitOfWork;
using CBS.BankMGT.Domain;
using Microsoft.EntityFrameworkCore;

namespace CBS.BankMGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to delete a Town based on DeleteTownCommand.
    /// </summary>
    public class DeleteTownCommandHandler : IRequestHandler<DeleteTownCommand, ServiceResponse<bool>>
    {
        private readonly ITownRepository _TownRepository; // Repository for accessing Town data.
        private readonly ILogger<DeleteTownCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteTownCommandHandler.
        /// </summary>
        /// <param name="TownRepository">Repository for Town data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteTownCommandHandler(
            ITownRepository TownRepository, IMapper mapper,
            ILogger<DeleteTownCommandHandler> logger
, IUnitOfWork<POSContext> uow)
        {
            _TownRepository = TownRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteTownCommand to delete a Town.
        /// </summary>
        /// <param name="request">The DeleteTownCommand containing Town ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteTownCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the Town entity with the specified ID exists
                var existingTown = await _TownRepository.AllIncluding(c => c.Subdivision).FirstOrDefaultAsync(c => c.Id == request.Id);
                if (existingTown == null)
                {
                    errorMessage = $"Town with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }
                existingTown.IsDeleted = true;
                _TownRepository.Update(existingTown);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting Town: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
