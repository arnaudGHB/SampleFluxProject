using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.FeeP;
using CBS.NLoan.Data.Entity.FeeP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.FeeMediaR.FeeP.Commands;
using CBS.NLoan.Repository.FeeP.FeeP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.FeeMediaR.FeeP.Handlers
{
    /// <summary>
    /// Handles the command to add a new Loan.
    /// </summary>
    public class AddFeeHandler : IRequestHandler<AddFeeCommand, ServiceResponse<FeeDto>>
    {
        private readonly IFeeRepository _feeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AddFeeHandler> _logger;
        private readonly IUnitOfWork<LoanContext> _uow;

        public AddFeeHandler(
            IFeeRepository feeRepository,
            IMapper mapper,
            ILogger<AddFeeHandler> logger,
            IUnitOfWork<LoanContext> uow)
        {
            _feeRepository = feeRepository ?? throw new ArgumentNullException(nameof(feeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        }

        public async Task<ServiceResponse<FeeDto>> Handle(AddFeeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a fee with the same name already exists
                var existingFee = await _feeRepository.FindBy(c => c.Name == request.Name).FirstOrDefaultAsync();
                if (existingFee != null)
                {
                    var errorMessage = $"{request.Name} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<FeeDto>.Return409(errorMessage);
                }

                var feeEntity = _mapper.Map<Fee>(request);
                feeEntity.Id = BaseUtilities.GenerateUniqueNumber();
                _feeRepository.Add(feeEntity);
                await _uow.SaveAsync();
                var feeDto = _mapper.Map<FeeDto>(feeEntity);
                return ServiceResponse<FeeDto>.ReturnResultWith200(feeDto);
            }
            catch (Exception e)
            {
                var errorMessage = $"Error occurred while saving fee: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<FeeDto>.Return500(e);
            }
        }
    }

}
