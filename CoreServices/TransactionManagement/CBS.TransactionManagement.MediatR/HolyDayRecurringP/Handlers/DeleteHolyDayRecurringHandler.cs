using AutoMapper;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.HolyDayRecurringP.Commands;
using CBS.TransactionManagement.Repository.HolyDayRecurringP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.MediatR.HolyDayRecurringMediaR.HolyDayRecurringP.Handlers
{
    /// <summary>
    /// Handles the command to delete a Loan based on DeleteLoanCommand.
    /// </summary>
    public class DeleteHolyDayRecurringHandler : IRequestHandler<DeleteHolyDayRecurringCommand, ServiceResponse<bool>>
    {
        private readonly IHolyDayRecurringRepository _HolyDayRecurringRepository; // Repository for accessing HolyDayRecurring data.
        private readonly ILogger<DeleteHolyDayRecurringHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteHolyDayRecurringCommandHandler.
        /// </summary>
        /// <param name="HolyDayRecurringRepository">Repository for HolyDayRecurring data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteHolyDayRecurringHandler(
            IHolyDayRecurringRepository HolyDayRecurringRepository, IMapper mapper,
            ILogger<DeleteHolyDayRecurringHandler> logger, IUnitOfWork<TransactionContext> uow)
        {
            _HolyDayRecurringRepository = HolyDayRecurringRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteHolyDayRecurringCommand to delete a HolyDayRecurring.
        /// </summary>
        /// <param name="request">The DeleteHolyDayRecurringCommand containing HolyDayRecurring ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteHolyDayRecurringCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the HolyDayRecurring entity with the specified ID exists
                var existingHolyDayRecurring = await _HolyDayRecurringRepository.FindAsync(request.Id);
                if (existingHolyDayRecurring == null)
                {
                    errorMessage = $"HolyDayRecurring with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingHolyDayRecurring.IsDeleted = true;
                _HolyDayRecurringRepository.Update(existingHolyDayRecurring);
                await _uow.SaveAsync();
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting HolyDayRecurring: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
