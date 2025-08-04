using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.LoanTermP;
using CBS.NLoan.Data.Entity.LoanTermP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanTermP.Commands;
using CBS.NLoan.Repository.LoanTermP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanTermP.Handlers
{
    /// <summary>
    /// Handles the command to add a new Loan.
    /// </summary>
    public class AddLoanTermHandler : IRequestHandler<AddLoanTermCommand, ServiceResponse<LoanTermDto>>
    {
        private readonly ILoanTermRepository _LoanTermRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AddLoanTermHandler> _logger;
        private readonly IUnitOfWork<LoanContext> _uow;

        public AddLoanTermHandler(
            ILoanTermRepository LoanTermRepository,
            IMapper mapper,
            ILogger<AddLoanTermHandler> logger,
            IUnitOfWork<LoanContext> uow)
        {
            _LoanTermRepository = LoanTermRepository ?? throw new ArgumentNullException(nameof(LoanTermRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        }

        public async Task<ServiceResponse<LoanTermDto>> Handle(AddLoanTermCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a LoanTerm with the same name already exists
                var existingLoanTerm = await _LoanTermRepository.FindBy(c => c.Name == request.Name).FirstOrDefaultAsync();
                if (existingLoanTerm != null)
                {
                    var errorMessage = $"{request.Name} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<LoanTermDto>.Return409(errorMessage);
                }

                var LoanTermEntity = _mapper.Map<LoanTerm>(request);
                LoanTermEntity.Id = BaseUtilities.GenerateUniqueNumber();
                _LoanTermRepository.Add(LoanTermEntity);
                await _uow.SaveAsync();
                var LoanTermDto = _mapper.Map<LoanTermDto>(LoanTermEntity);
                return ServiceResponse<LoanTermDto>.ReturnResultWith200(LoanTermDto);
            }
            catch (Exception e)
            {
                var errorMessage = $"Error occurred while saving LoanTerm: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanTermDto>.Return500(e);
            }
        }
    }

}
