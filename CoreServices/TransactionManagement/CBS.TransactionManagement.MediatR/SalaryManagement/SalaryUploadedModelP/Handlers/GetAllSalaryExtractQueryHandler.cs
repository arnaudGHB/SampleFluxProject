using AutoMapper;
using CBS.TransactionManagement.Data.Entity.SalaryFilesDto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.SalaryManagement.SalaryFiles.Queries;
using CBS.TransactionManagement.MediatR.SalaryManagement.SalaryUploadedModelP.Queries;
using CBS.TransactionManagement.MediatR.SalaryManagement.StandingOrderP.Queries;
using CBS.TransactionManagement.Repository.SalaryManagement.SalaryFiles;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.MediatR.SalaryManagement.SalaryUploadedModelP.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllSalaryExtractQueryHandler : IRequestHandler<GetAllSalaryExtractQuery, ServiceResponse<List<SalaryExtractDto>>>
    {
        private readonly ISalaryExecutedRepository _SalaryExtractRepository; // Repository for accessing SalaryExtracts data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllSalaryExtractQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllSalaryExtractQueryHandler.
        /// </summary>
        /// <param name="SalaryExtractRepository">Repository for SalaryExtracts data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllSalaryExtractQueryHandler(
            ISalaryExecutedRepository SalaryExtractRepository,
            IMapper mapper, ILogger<GetAllSalaryExtractQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _SalaryExtractRepository = SalaryExtractRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllSalaryExtractQuery to retrieve all SalaryExtracts.
        /// </summary>
        /// <param name="request">The GetAllSalaryExtractQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<SalaryExtractDto>>> Handle(GetAllSalaryExtractQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Build the query with conditional filtering
                var query = _SalaryExtractRepository.FindBy(x => !x.IsDeleted && x.FileUploadId == request.FileUploadId);

                // Add BranchId filtering if it's provided
                if (!string.IsNullOrEmpty(request.BranchId))
                {
                    query = query.Where(x => x.BranchId == request.BranchId);
                }

                // Retrieve the results and map to DTO
                var entities = await query.Include(x => x.FileUpload)
                                           .AsNoTracking()
                                           .ToListAsync();

                return ServiceResponse<List<SalaryExtractDto>>.ReturnResultWith200(_mapper.Map<List<SalaryExtractDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all SalaryExtracts: {e.Message}");
                return ServiceResponse<List<SalaryExtractDto>>.Return500(e, "Failed to get all SalaryExtracts");
            }
        }

    }
}
