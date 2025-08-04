using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.FeeP;
using CBS.NLoan.Data.Entity.FeeP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.FeeMediaR.FeeP.Commands;
using CBS.NLoan.Repository.FeeP.FeeP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.FeeMediaR.FeeP.Handlers
{

    /// <summary>
    /// Handles the command to update a Loan based on UpdateLoanCommand.
    /// </summary>
    public class UpdateFeeHandler : IRequestHandler<UpdateFeeCommand, ServiceResponse<FeeDto>>
    {
        private readonly IFeeRepository _feeRepository;
        private readonly ILogger<UpdateFeeHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<LoanContext> _uow;

        public UpdateFeeHandler(
            IFeeRepository feeRepository,
            ILogger<UpdateFeeHandler> logger,
            IMapper mapper,
            IUnitOfWork<LoanContext> uow = null)
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
