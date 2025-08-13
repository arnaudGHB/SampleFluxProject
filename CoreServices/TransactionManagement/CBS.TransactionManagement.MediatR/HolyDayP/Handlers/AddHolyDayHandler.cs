using AutoMapper;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data.Dto.HolyDayP;
using CBS.TransactionManagement.Data.Entity.HolyDayP;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.HolyDayMediaR.HolyDayP.Commands;
using CBS.TransactionManagement.Repository.HolyDayP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.MediatR.HolyDayP.Handlers
{
    /// <summary>
    /// Handles the command to add a new Loan.
    /// </summary>
    public class AddHolyDayHandler : IRequestHandler<AddHolyDayCommand, ServiceResponse<HolyDayDto>>
    {
        private readonly IHolyDayRepository _HolyDayRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AddHolyDayHandler> _logger;
        private readonly IUnitOfWork<TransactionContext> _uow;

        public AddHolyDayHandler(
            IHolyDayRepository HolyDayRepository,
            IMapper mapper,
            ILogger<AddHolyDayHandler> logger,
            IUnitOfWork<TransactionContext> uow)
        {
            _HolyDayRepository = HolyDayRepository ?? throw new ArgumentNullException(nameof(HolyDayRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        }

        public async Task<ServiceResponse<HolyDayDto>> Handle(AddHolyDayCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a HolyDay with the same name already exists
                var existingHolyDay = await _HolyDayRepository.FindBy(c => c.EventName == request.EventName && c.BranchId == request.BranchId && c.IsDeleted == false).FirstOrDefaultAsync();
                if (existingHolyDay != null)
                {
                    var errorMessage = $"{request.EventName} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<HolyDayDto>.Return409(errorMessage);
                }

                // Map the command to the HolyDay entity
                var HolyDayEntity = _mapper.Map<HolyDay>(request);
                HolyDayEntity.Id = BaseUtilities.GenerateUniqueNumber();

                // Add to repository and save changes
                _HolyDayRepository.Add(HolyDayEntity);
                await _uow.SaveAsync();

                // Map to DTO for response
                var HolyDayDto = _mapper.Map<HolyDayDto>(HolyDayEntity);

                // Determine if the configuration is centralized or by branch
                var configType = HolyDayEntity.IsCentralisedConfiguration ? "centralized" : $"specific to branch {HolyDayEntity.BranchId}";

                // Enhanced success message with configuration type
                var successMessage = $"Holiday '{HolyDayEntity.EventName}' has been successfully created. " +
                                     $"It is {configType} with ID {HolyDayEntity.Id}.";

                return ServiceResponse<HolyDayDto>.ReturnResultWith200(HolyDayDto, successMessage);
            }
            catch (Exception e)
            {
                var errorMessage = $"Error occurred while saving HolyDay: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<HolyDayDto>.Return500(e);
            }
        }
    }

}
