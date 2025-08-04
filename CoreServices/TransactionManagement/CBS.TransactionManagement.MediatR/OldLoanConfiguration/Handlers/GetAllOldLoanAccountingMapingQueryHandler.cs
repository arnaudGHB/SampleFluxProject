using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using System.Linq;
using CBS.TransactionManagement.OldLoanConfiguration.Queries;
using CBS.TransactionManagement.Data.Dto.OldLoanConfiguration;
using CBS.TransactionManagement.Repository.OldLoanConfiguration;

namespace CBS.TransactionManagement.OldLoanConfiguration.Handlers
{
    /// <summary>
    /// Handles the retrieval of all OldLoanAccountingMaping based on the GetAllOldLoanAccountingMapingQuery.
    /// </summary>
    public class GetAllOldLoanAccountingMapingQueryHandler : IRequestHandler<GetAllOldLoanAccountingMapingQuery, ServiceResponse<List<OldLoanAccountingMapingDto>>>
    {
        private readonly IOldLoanAccountingMapingRepository _OldLoanAccountingMapingRepository; // Repository for accessing OldLoanAccountingMapings data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllOldLoanAccountingMapingQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllOldLoanAccountingMapingQueryHandler.
        /// </summary>
        /// <param name="OldLoanAccountingMapingRepository">Repository for OldLoanAccountingMapings data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllOldLoanAccountingMapingQueryHandler(
            IOldLoanAccountingMapingRepository OldLoanAccountingMapingRepository,
            IMapper mapper, ILogger<GetAllOldLoanAccountingMapingQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _OldLoanAccountingMapingRepository = OldLoanAccountingMapingRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllOldLoanAccountingMapingQuery to retrieve all OldLoanAccountingMapings.
        /// </summary>
        /// <param name="request">The GetAllOldLoanAccountingMapingQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<OldLoanAccountingMapingDto>>> Handle(GetAllOldLoanAccountingMapingQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve all OldLoanAccountingMapings entities from the repository where IsDeleted is false
                var entities = await _OldLoanAccountingMapingRepository.FindBy(x => x.IsDeleted == false).ToListAsync();

                // If no entities found, log and return 404
                if (entities == null || entities.Count == 0)
                {
                    errorMessage = "No OldLoanAccountingMaping records were found.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.Read, LogLevelInfo.Warning);
                    return ServiceResponse<List<OldLoanAccountingMapingDto>>.Return404(errorMessage);
                }

                // Log and audit success
                string successMessage = "Successfully retrieved all OldLoanAccountingMaping records.";
                await BaseUtilities.LogAndAuditAsync(successMessage, entities, HttpStatusCodeEnum.OK, LogAction.Read, LogLevelInfo.Information);

                // Return mapped result with 200 status code
                return ServiceResponse<List<OldLoanAccountingMapingDto>>.ReturnResultWith200(_mapper.Map<List<OldLoanAccountingMapingDto>>(entities));
            }
            catch (Exception e)
            {
                // Handle and log the exception
                errorMessage = $"Error occurred while retrieving OldLoanAccountingMaping records: {e.Message}";
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.Read, LogLevelInfo.Error);

                return ServiceResponse<List<OldLoanAccountingMapingDto>>.Return500(errorMessage);
            }
        }
    }
}
