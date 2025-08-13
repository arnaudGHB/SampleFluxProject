using AutoMapper;
using CBS.TransactionManagement.Data.Entity.SalaryFilesDto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.SalaryManagement.SalaryAnalysisResultP.Queries;
using CBS.TransactionManagement.MediatR.SalaryManagement.SalaryFiles.Queries;
using CBS.TransactionManagement.MediatR.SalaryManagement.StandingOrderP.Queries;
using CBS.TransactionManagement.Repository.SalaryManagement.SalaryFiles;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.MediatR.SalaryManagement.SalaryFiles.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetSalaryExtractQueryHandler : IRequestHandler<GetSalaryExtractQuery, ServiceResponse<SalaryExtractDto>>
    {
        private readonly ISalaryProcessingRepository _SalaryExtractRepository; // Repository for accessing SalaryExtract data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetSalaryExtractQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetSalaryExtractQueryHandler.
        /// </summary>
        /// <param name="SalaryExtractRepository">Repository for SalaryExtract data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetSalaryExtractQueryHandler(
            ISalaryProcessingRepository SalaryExtractRepository,
            IMapper mapper,
            ILogger<GetSalaryExtractQueryHandler> logger)
        {
            _SalaryExtractRepository = SalaryExtractRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetStandingOrderQuery to retrieve a specific SalaryExtract.
        /// </summary>
        /// <param name="request">The GetStandingOrderQuery containing SalaryExtract ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<SalaryExtractDto>> Handle(GetSalaryExtractQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the SalaryExtract entity with the specified ID from the repository
                var entity = await _SalaryExtractRepository.FindBy(x => x.Id == request.Id).Include(x => x.FileUpload).FirstOrDefaultAsync();
                if (entity != null)
                {
                    // Map the SalaryExtract entity to SalaryExtractDto and return it with a success response
                    var SalaryExtractDto = _mapper.Map<SalaryExtractDto>(entity);
                    return ServiceResponse<SalaryExtractDto>.ReturnResultWith200(SalaryExtractDto);
                }
                else
                {
                    // If the SalaryExtract entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("SalaryExtract not found.");
                    return ServiceResponse<SalaryExtractDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting SalaryExtract: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<SalaryExtractDto>.Return500(e);
            }
        }
    }

}
