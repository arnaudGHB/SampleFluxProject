using AutoMapper;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data.Dto.FeeP;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.FeeMediaR.FeeP.Commands;
using CBS.TransactionManagement.Repository.FeeP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.MediatR.FeeMediaR.FeeP.Handlers
{

    /// <summary>
    /// Handles the command to update a Loan based on UpdateLoanCommand.
    /// </summary>
    public class UpdateFeeHandler : IRequestHandler<UpdateFeeCommand, ServiceResponse<FeeDto>>
    {
        private readonly IFeeRepository _feeRepository;
        private readonly ILogger<UpdateFeeHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<TransactionContext> _uow;

        public UpdateFeeHandler(
            IFeeRepository feeRepository,
            ILogger<UpdateFeeHandler> logger,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null)
        {
            _feeRepository = feeRepository ?? throw new ArgumentNullException(nameof(feeRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _uow = uow;
        }

        public async Task<ServiceResponse<FeeDto>> Handle(UpdateFeeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingFee = await _feeRepository.FindAsync(request.Id);

                if (existingFee != null)
                {
                    _mapper.Map(request, existingFee);
                    _feeRepository.Update(existingFee);
                    await _uow.SaveAsync();
                    var response = ServiceResponse<FeeDto>.ReturnResultWith200(_mapper.Map<FeeDto>(existingFee));
                    _logger.LogInformation($"Fee {request.Name} was successfully updated.");
                    return response;
                }
                else
                {
                    string errorMessage = $"{request.Name} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<FeeDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                string errorMessage = $"Error occurred while updating Fee: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<FeeDto>.Return500(e);
            }
        }
    }

}
