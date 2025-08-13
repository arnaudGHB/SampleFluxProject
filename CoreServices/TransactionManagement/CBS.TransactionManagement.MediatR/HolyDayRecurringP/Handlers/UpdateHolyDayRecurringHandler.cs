using AutoMapper;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data.Dto.HolyDayRecurringP;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.HolyDayRecurringP.Commands;
using CBS.TransactionManagement.Repository.HolyDayRecurringP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.MediatR.HolyDayRecurringMediaR.HolyDayRecurringP.Handlers
{

    /// <summary>
    /// Handles the command to update a Loan based on UpdateLoanCommand.
    /// </summary>
    public class UpdateHolyDayRecurringHandler : IRequestHandler<UpdateHolyDayRecurringCommand, ServiceResponse<HolyDayRecurringDto>>
    {
        private readonly IHolyDayRecurringRepository _HolyDayRecurringRepository;
        private readonly ILogger<UpdateHolyDayRecurringHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<TransactionContext> _uow;

        public UpdateHolyDayRecurringHandler(
            IHolyDayRecurringRepository HolyDayRecurringRepository,
            ILogger<UpdateHolyDayRecurringHandler> logger,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null)
        {
            _HolyDayRecurringRepository = HolyDayRecurringRepository ?? throw new ArgumentNullException(nameof(HolyDayRecurringRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _uow = uow;
        }

        /// <summary>
        /// Handles the update of a recurring holiday configuration.
        /// </summary>
        /// <param name="request">The update command containing the updated details of the recurring holiday.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>Service response indicating the result of the update operation.</returns>
        public async Task<ServiceResponse<HolyDayRecurringDto>> Handle(UpdateHolyDayRecurringCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the existing holiday configuration by ID
                var existingHolyDayRecurring = await _HolyDayRecurringRepository.FindAsync(request.Id);

                if (existingHolyDayRecurring != null)
                {
                    // Update the existing entity with the new details
                    _mapper.Map(request, existingHolyDayRecurring);

                    // Save the changes
                    _HolyDayRecurringRepository.Update(existingHolyDayRecurring);
                    await _uow.SaveAsync();

                    // Map the updated entity to the DTO for the response
                    var updatedDto = _mapper.Map<HolyDayRecurringDto>(existingHolyDayRecurring);

                    // Construct a dynamic success message
                    string scopeMessage = existingHolyDayRecurring.IsGlobal
                        ? "globally for all branches"
                        : $"for the branch '{existingHolyDayRecurring.BranchId}'";

                    string successMessage = $"The holiday '{updatedDto.Name}' of type '{updatedDto.HolidayType}' " +
                                            $"with a recurrence pattern '{updatedDto.RecurrencePattern}' " +
                                            $"was successfully updated {scopeMessage}.";

                    // Log the success and return the response
                    _logger.LogInformation(successMessage);
                    return ServiceResponse<HolyDayRecurringDto>.ReturnResultWith200(updatedDto, successMessage);
                }
                else
                {
                    // Log and return a not-found error if the entity doesn't exist
                    string errorMessage = $"The holiday with ID '{request.Id}' could not be found for update.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<HolyDayRecurringDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log the exception and return a server error response
                string errorMessage = $"An error occurred while updating the holiday: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<HolyDayRecurringDto>.Return500(e);
            }
        }
    }

}
