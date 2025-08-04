using AutoMapper;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.HolyDayMediaR.HolyDayP.Commands;
using CBS.TransactionManagement.MediatR.HolyDayP.Commands;
using CBS.TransactionManagement.Repository.HolyDayP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.MediatR.HolyDayMediaR.HolyDayP.Handlers
{
    /// <summary>
    /// Handles the command to delete a Loan based on DeleteLoanCommand.
    /// </summary>
    public class DeleteHolyDayHandler : IRequestHandler<DeleteHolyDayCommand, ServiceResponse<bool>>
    {
        private readonly IHolyDayRepository _HolyDayRepository; // Repository for accessing HolyDay data.
        private readonly ILogger<DeleteHolyDayHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteHolyDayCommandHandler.
        /// </summary>
        /// <param name="HolyDayRepository">Repository for HolyDay data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteHolyDayHandler(
            IHolyDayRepository HolyDayRepository, IMapper mapper,
            ILogger<DeleteHolyDayHandler> logger, IUnitOfWork<TransactionContext> uow)
        {
            _HolyDayRepository = HolyDayRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteHolyDayCommand to delete a HolyDay.
        /// </summary>
        /// <param name="request">The DeleteHolyDayCommand containing HolyDay ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteHolyDayCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the HolyDay entity with the specified ID exists
                var existingHolyDay = await _HolyDayRepository.FindAsync(request.Id);
                if (existingHolyDay == null)
                {
                    errorMessage = $"HolyDay with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingHolyDay.IsDeleted = true;
                _HolyDayRepository.Update(existingHolyDay);
                await _uow.SaveAsync();
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting HolyDay: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
