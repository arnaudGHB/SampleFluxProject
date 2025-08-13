using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Repository.ChargesWaivedP;
using CBS.TransactionManagement.Commands.ChargesWaivedP;

namespace CBS.TransactionManagement.Handlers.ChargesWaivedP
{
    /// <summary>
    /// Handles the command to delete a ChargesWaived based on DeleteChargesWaivedCommand.
    /// </summary>
    public class DeleteChargesWaivedCommandHandler : IRequestHandler<DeleteChargesWaivedCommand, ServiceResponse<bool>>
    {
        private readonly IChargesWaivedRepository _ChargesWaivedRepository; // Repository for accessing ChargesWaived data.
        private readonly ILogger<DeleteChargesWaivedCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteChargesWaivedCommandHandler.
        /// </summary>
        /// <param name="ChargesWaivedRepository">Repository for ChargesWaived data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteChargesWaivedCommandHandler(
            IChargesWaivedRepository ChargesWaivedRepository, IMapper mapper,
            ILogger<DeleteChargesWaivedCommandHandler> logger
, IUnitOfWork<TransactionContext> uow)
        {
            _ChargesWaivedRepository = ChargesWaivedRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteChargesWaivedCommand to delete a ChargesWaived.
        /// </summary>
        /// <param name="request">The DeleteChargesWaivedCommand containing ChargesWaived ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteChargesWaivedCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the ChargesWaived entity with the specified ID exists
                var existingChargesWaived = await _ChargesWaivedRepository.FindAsync(request.Id);
                if (existingChargesWaived == null)
                {
                    errorMessage = $"ChargesWaived with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingChargesWaived.IsDeleted = true;
                _ChargesWaivedRepository.Update(existingChargesWaived);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting ChargesWaived: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
