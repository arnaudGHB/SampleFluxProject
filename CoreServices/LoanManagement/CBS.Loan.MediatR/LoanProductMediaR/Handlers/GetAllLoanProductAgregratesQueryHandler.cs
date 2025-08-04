using AutoMapper;
using CBS.NLoan.Data.Dto.DocumentP;
using CBS.NLoan.Data.Dto.FeeP;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanProductMediaR.Queries;
using CBS.NLoan.Repository.DocumentP;
using CBS.NLoan.Repository.FeeP.FeeP;
using CBS.NLoan.Repository.FundingLineP;
using CBS.NLoan.Repository.LoanProductP;
using CBS.NLoan.Repository.PenaltyP;
using CBS.NLoan.Repository.TaxP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanProductMediaR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllLoanProductAgregratesQueryHandler : IRequestHandler<GetAllLoanProductAgregatesQuery, ServiceResponse<LoanProductConfigurationAgregatesDto>>
    {
        private readonly ILoanProductRepository _LoanProductRepository;
        private readonly ITaxRepository _TaxRepository;
        private readonly IPenaltyRepository _PenaltyRepository;
        private readonly IFeeRepository _FeeRepository;
        private readonly IDocumentPackRepository _DocumentPackRepository;
        private readonly IFundingLineRepository _FundingLineRepository;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllLoanProductQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllLoanProductQueryHandler.
        /// </summary>
        /// <param name="LoanProductRepository">Repository for LoanProducts data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllLoanProductAgregratesQueryHandler(
            ILoanProductRepository LoanProductRepository,
            IMapper mapper, ILogger<GetAllLoanProductQueryHandler> logger,
            ITaxRepository taxRepository, IPenaltyRepository penaltyRepository, IFeeRepository feeRepository, IDocumentPackRepository documentPackRepository, IFundingLineRepository fundingLineRepository)
        {
            // Assign provided dependencies to local variables.
            _LoanProductRepository = LoanProductRepository;
            _mapper = mapper;
            _logger = logger;
            _TaxRepository = taxRepository;
            _PenaltyRepository = penaltyRepository;
            _FeeRepository = feeRepository;
            _DocumentPackRepository = documentPackRepository;
            _FundingLineRepository = fundingLineRepository;
        }

        /// <summary>
        /// Handles the GetAllLoanProductQuery to retrieve all LoanProducts.
        /// </summary>
        /// <param name="request">The GetAllLoanProductQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanProductConfigurationAgregatesDto>> Handle(GetAllLoanProductAgregatesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all LoanProducts entities from the repository
                LoanProductConfigurationAgregatesDto loanProductConfigurationAgregatesDto = new LoanProductConfigurationAgregatesDto();

                var taxes = await _TaxRepository.All.ToListAsync();
                var penalties = await _PenaltyRepository.All.ToListAsync();
                var fees = await _FeeRepository.All.ToListAsync();
                var feesDto = _mapper.Map<List<FeeDto>>(fees);
                var documents = await _DocumentPackRepository.All.ToListAsync();
                var documentsDto = _mapper.Map<List<DocumentPackDto>>(documents);
                var fundingLines = await _FundingLineRepository.All.ToListAsync();

                loanProductConfigurationAgregatesDto.Taxes = taxes;
                loanProductConfigurationAgregatesDto.Penalties = penalties;
                loanProductConfigurationAgregatesDto.Fees = feesDto;
                loanProductConfigurationAgregatesDto.DocumentPacks = documentsDto;
                loanProductConfigurationAgregatesDto.FundingLines = fundingLines;


                return ServiceResponse<LoanProductConfigurationAgregatesDto>.ReturnResultWith200(loanProductConfigurationAgregatesDto);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all LoanProducts: {e.Message}");
                return ServiceResponse<LoanProductConfigurationAgregatesDto>.Return500(e, "Failed to get all LoanProducts");
            }
        }
    }
}
