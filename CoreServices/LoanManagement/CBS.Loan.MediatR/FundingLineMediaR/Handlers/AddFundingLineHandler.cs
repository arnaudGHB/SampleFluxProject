using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.FundingLineP;
using CBS.NLoan.Data.Entity.FundingLineP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.FundingLineMediaR.Commands;
using CBS.NLoan.Repository.FundingLineP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.FundingLineMediaR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Loan.
    /// </summary>
    public class AddFundingLineHandler : IRequestHandler<AddFundingLineCommand, ServiceResponse<FundingLineDto>>
    {
        private readonly IFundingLineRepository _FundingLineRepository; // Repository for accessing FundingLine data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddFundingLineHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<LoanContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddFundingLineCommandHandler.
        /// </summary>
        /// <param name="FundingLineRepository">Repository for FundingLine data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddFundingLineHandler(
            IFundingLineRepository FundingLineRepository,
            IMapper mapper,
            ILogger<AddFundingLineHandler> logger,
            IUnitOfWork<LoanContext> uow)
        {
            _FundingLineRepository = FundingLineRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddFundingLineCommand to add a new FundingLine.
        /// </summary>
        /// <param name="request">The AddFundingLineCommand containing FundingLine data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<FundingLineDto>> Handle(AddFundingLineCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a FundingLine with the same name already exists (case-insensitive)
                //var existingFundingLine = await _FundingLineRepository.FindBy(c => c.Id == request.Id).FirstOrDefaultAsync();

                // If a FundingLine with the same name already exists, return a conflict response
                //if (existingFundingLine != null)
                //{
                //    var errorMessage = $"FundingLine {request.Id} already exists.";
                //    _logger.LogError(errorMessage);
                //    return ServiceResponse<FundingLineDto>.Return409(errorMessage);
                //}


                // Map the AddFundingLineCommand to a FundingLine entity
                var FundingLineEntity = _mapper.Map<FundingLine>(request);
                // Convert UTC to local time and set it in the entity
                FundingLineEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
                FundingLineEntity.Id = BaseUtilities.GenerateUniqueNumber();

                // Add the new FundingLine entity to the repository
                _FundingLineRepository.Add(FundingLineEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<FundingLineDto>.Return500();
                }

                // Map the FundingLine entity to FundingLineDto and return it with a success response
                var FundingLineDto = _mapper.Map<FundingLineDto>(FundingLineEntity);
                return ServiceResponse<FundingLineDto>.ReturnResultWith200(FundingLineDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving FundingLine: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<FundingLineDto>.Return500(e);
            }
        }
    }

}
