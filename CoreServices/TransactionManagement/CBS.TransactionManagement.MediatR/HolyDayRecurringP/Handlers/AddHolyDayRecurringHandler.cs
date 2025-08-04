using AutoMapper;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data.Dto.HolyDayRecurringP;
using CBS.TransactionManagement.Data.Entity.HolyDayRecurringP;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.RecurringRecurringP.Commands;
using CBS.TransactionManagement.Repository.HolyDayRecurringP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.MediatR.HolyDayRecurringP.Handlers
{
    /// <summary>
    /// Handles the command to add a new Loan.
    /// </summary>
    public class AddHolyDayRecurringHandler : IRequestHandler<AddHolyDayRecurringCommand, ServiceResponse<HolyDayRecurringDto>>
    {
        private readonly IHolyDayRecurringRepository _HolyDayRecurringRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AddHolyDayRecurringHandler> _logger;
        private readonly IUnitOfWork<TransactionContext> _uow;

        public AddHolyDayRecurringHandler(
            IHolyDayRecurringRepository HolyDayRecurringRepository,
            IMapper mapper,
            ILogger<AddHolyDayRecurringHandler> logger,
            IUnitOfWork<TransactionContext> uow)
        {
            _HolyDayRecurringRepository = HolyDayRecurringRepository ?? throw new ArgumentNullException(nameof(HolyDayRecurringRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        }

        /// <summary>
        /// Handles the addition of a new recurring holiday based on the provided request.
        /// Validates if a similar recurring holiday already exists and saves the new entity if valid.
        /// </summary>
        /// <param name="request">The command containing details of the new recurring holiday.</param>
        /// <param name="cancellationToken">Token for canceling the asynchronous operation.</param>
        /// <returns>A ServiceResponse containing the newly created HolyDayRecurringDto or an error message.</returns>
        public async Task<ServiceResponse<HolyDayRecurringDto>> Handle(AddHolyDayRecurringCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Check for a recurring holiday with the same name in the specified branch
                var existingHolyDayRecurringName = await _HolyDayRecurringRepository
                    .FindBy(c => c.Name == request.Name && c.BranchId == request.BranchId && c.IsDeleted == false)
                    .FirstOrDefaultAsync();

                if (existingHolyDayRecurringName != null)
                {
                    // Log and return conflict message if a recurring holiday with the same name exists
                    var errorMessage = $"A holiday with the name '{request.Name}' already exists in the branch '{request.BranchId}'. " +
                                       "Please use a different name or verify the existing holiday.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<HolyDayRecurringDto>.Return409(errorMessage);
                }

                // Step 2: Check for a recurring holiday with the same type and recurrence pattern in the branch
                var existingHolyDayRecurring = await _HolyDayRecurringRepository
                    .FindBy(c => c.HolidayType == request.HolidayType &&
                                 c.RecurrencePattern == request.RecurrencePattern &&
                                 c.BranchId == request.BranchId &&
                                 c.IsDeleted == false &&
                                 c.Name == request.Name)
                    .FirstOrDefaultAsync();

                if (existingHolyDayRecurring != null)
                {
                    // Log and return conflict message if a holiday with the same type and pattern exists
                    var errorMessage = $"A '{request.HolidayType}' holiday with the recurrence pattern '{request.RecurrencePattern}' " +
                                       $"already exists in the branch '{request.BranchId}'. Please verify or update the existing holiday.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<HolyDayRecurringDto>.Return409(errorMessage);
                }

                // Step 3: Map the request to the entity, assign a unique ID, and save the new holiday
                var HolyDayRecurringEntity = _mapper.Map<HolyDayRecurring>(request);
                HolyDayRecurringEntity.Id = BaseUtilities.GenerateUniqueNumber(); // Assign a unique ID
                _HolyDayRecurringRepository.Add(HolyDayRecurringEntity); // Add the new entity to the repository
                await _uow.SaveAsync(); // Save changes to the database

                // Step 4: Map the saved entity to the DTO for response
                var HolyDayRecurringDto = _mapper.Map<HolyDayRecurringDto>(HolyDayRecurringEntity);

                // Return success response with the newly created DTO
                // Return success response with a detailed success message
                // Determine if the holiday is global or branch-specific
                string scopeMessage = HolyDayRecurringDto.IsGlobal
                    ? "globally for all branches"
                    : $"for the branch '{request.BranchId}'";

                // Return success response with a detailed success message
                return ServiceResponse<HolyDayRecurringDto>.ReturnResultWith200(
                    HolyDayRecurringDto,
                    $"The holiday '{HolyDayRecurringDto.Name}' of type '{HolyDayRecurringDto.HolidayType}' " +
                    $"with a recurrence pattern '{HolyDayRecurringDto.RecurrencePattern}' " +
                    $"was successfully created {scopeMessage}."
                );
            }
            catch (Exception e)
            {
                // Log and return a detailed error message in case of an exception
                var errorMessage = $"An error occurred while saving the recurring holiday: {e.Message}. " +
                                   "Please contact the system administrator if the issue persists.";
                _logger.LogError(errorMessage);
                return ServiceResponse<HolyDayRecurringDto>.Return500(e);
            }
        }
    }

}
