using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanApplicationMediaR.Validations;
using CBS.NLoan.MediatR.LoanProductMediaR.Commands;
using CBS.NLoan.Repository.LoanApplicationP.LoanCommentryP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanProductMediaR.Handlers
{

    public class AddLoanProductRepaymentCycleCommandHandler : IRequestHandler<AddLoanProductRepaymentCycleCommand, ServiceResponse<bool>>
    {
        private readonly ILoanProductRepaymentCycleRepository _LoanProductRepaymentCycleRepository; // Repository for accessing LoanProductRepaymentCycle data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddLoanProductRepaymentCycleCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the AddLoanProductRepaymentCycleCommandHandler.
        /// </summary>
        /// <param name="LoanProductRepaymentCycleRepository">Repository for LoanProductRepaymentCycle data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddLoanProductRepaymentCycleCommandHandler(
            ILoanProductRepaymentCycleRepository LoanProductRepaymentCycleRepository,
            IMapper mapper,
            ILogger<AddLoanProductRepaymentCycleCommandHandler> logger,
            IUnitOfWork<LoanContext> uow)
        {
            _LoanProductRepaymentCycleRepository = LoanProductRepaymentCycleRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddLoanProductRepaymentCycleCommand to add a new LoanProductRepaymentCycle.
        /// </summary>
        /// <param name="request">The AddLoanProductRepaymentCycleCommand containing LoanProductRepaymentCycle data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(AddLoanProductRepaymentCycleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var loanProductRepaymentCycles = await _LoanProductRepaymentCycleRepository.FindBy(x => x.LoanProductId == request.LoanProductId).ToListAsync();
                _LoanProductRepaymentCycleRepository.RemoveRange(loanProductRepaymentCycles);

                // Check if a LoanProductRepaymentCycle with the same name already exists (case-insensitive)
                int i = 0;
                foreach (var a in request.RepaymentCycles)
                {
                    i++;
                    var existingLoanProductRepaymentCycle = new LoanProductRepaymentCycle
                    {
                        Id = BaseUtilities.GenerateUniqueNumber(),
                        LoanProductId = request.LoanProductId,
                        RepaymentCycle = a
                    };
                    _LoanProductRepaymentCycleRepository.Add(existingLoanProductRepaymentCycle);
                }
                //if (request.RepaymentCycles.Any())
                //{
                //    await _uow.SaveAsync();
                //}
                // Map the LoanProductRepaymentCycle entity to LoanProductRepaymentCycleDto and return it with a success response
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving LoanProductRepaymentCycle: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e, errorMessage);
            }
        }
    }

}
