using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.LoanTermP;
using CBS.NLoan.Data.Entity.LoanTermP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanTermP.Commands;
using CBS.NLoan.Repository.LoanTermP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanTermP.Handlers
{

    /// <summary>
    /// Handles the command to update a Loan based on UpdateLoanCommand.
    /// </summary>
    public class UpdateLoanTermHandler : IRequestHandler<UpdateLoanTermCommand, ServiceResponse<LoanTermDto>>
    {
        private readonly ILoanTermRepository _LoanTermRepository;
        private readonly ILogger<UpdateLoanTermHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<LoanContext> _uow;

        public UpdateLoanTermHandler(
            ILoanTermRepository LoanTermRepository,
            ILogger<UpdateLoanTermHandler> logger,
            IMapper mapper,
            IUnitOfWork<LoanContext> uow = null)
        {
            _LoanTermRepository = LoanTermRepository ?? throw new ArgumentNullException(nameof(LoanTermRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _uow = uow;
        }

        public async Task<ServiceResponse<LoanTermDto>> Handle(UpdateLoanTermCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingLoanTerm = await _LoanTermRepository.FindAsync(request.Id);

                if (existingLoanTerm != null)
                {
                    _mapper.Map(request, existingLoanTerm);
                    _LoanTermRepository.Update(existingLoanTerm);
                    await _uow.SaveAsync();
                    var response = ServiceResponse<LoanTermDto>.ReturnResultWith200(_mapper.Map<LoanTermDto>(existingLoanTerm));
                    _logger.LogInformation($"LoanTerm {request.Name} was successfully updated.");
                    return response;
                }
                else
                {
                    string errorMessage = $"{request.Name} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<LoanTermDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                string errorMessage = $"Error occurred while updating LoanTerm: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanTermDto>.Return500(e);
            }
        }
    }

}
