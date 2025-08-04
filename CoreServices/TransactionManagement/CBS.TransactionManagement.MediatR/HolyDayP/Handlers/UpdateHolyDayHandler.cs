using AutoMapper;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data.Dto.HolyDayP;
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
    /// Handles the command to update a Loan based on UpdateLoanCommand.
    /// </summary>
    public class UpdateHolyDayHandler : IRequestHandler<UpdateHolyDayCommand, ServiceResponse<HolyDayDto>>
    {
        private readonly IHolyDayRepository _HolyDayRepository;
        private readonly ILogger<UpdateHolyDayHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<TransactionContext> _uow;

        public UpdateHolyDayHandler(
            IHolyDayRepository HolyDayRepository,
            ILogger<UpdateHolyDayHandler> logger,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null)
        {
            _HolyDayRepository = HolyDayRepository ?? throw new ArgumentNullException(nameof(HolyDayRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _uow = uow;
        }

        public async Task<ServiceResponse<HolyDayDto>> Handle(UpdateHolyDayCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingHolyDay = await _HolyDayRepository.FindAsync(request.Id);

                if (existingHolyDay != null)
                {
                    _mapper.Map(request, existingHolyDay);
                    _HolyDayRepository.Update(existingHolyDay);
                    await _uow.SaveAsync();
                    var response = ServiceResponse<HolyDayDto>.ReturnResultWith200(_mapper.Map<HolyDayDto>(existingHolyDay));
                    _logger.LogInformation($"HolyDay {request.EventName} was successfully updated.");
                    return response;
                }
                else
                {
                    string errorMessage = $"{request.EventName} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<HolyDayDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                string errorMessage = $"Error occurred while updating HolyDay: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<HolyDayDto>.Return500(e);
            }
        }
    }

}
