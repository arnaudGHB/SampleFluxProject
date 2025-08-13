using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.OldLoanConfiguration.Queries;
using CBS.TransactionManagement.Data.Dto.OldLoanConfiguration;
using CBS.TransactionManagement.Repository.OldLoanConfiguration;

namespace CBS.TransactionManagement.OldLoanConfiguration.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific OldLoanAccountingMaping based on its unique identifier.
    /// </summary>
    public class GetOldLoanAccountingMapingQueryHandler : IRequestHandler<GetOldLoanAccountingMapingQuery, ServiceResponse<OldLoanAccountingMapingDto>>
    {
        private readonly IOldLoanAccountingMapingRepository _OldLoanAccountingMapingRepository; // Repository for accessing OldLoanAccountingMaping data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetOldLoanAccountingMapingQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetOldLoanAccountingMapingQueryHandler.
        /// </summary>
        /// <param name="OldLoanAccountingMapingRepository">Repository for OldLoanAccountingMaping data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetOldLoanAccountingMapingQueryHandler(
            IOldLoanAccountingMapingRepository OldLoanAccountingMapingRepository,
            IMapper mapper,
            ILogger<GetOldLoanAccountingMapingQueryHandler> logger)
        {
            _OldLoanAccountingMapingRepository = OldLoanAccountingMapingRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetOldLoanAccountingMapingQuery to retrieve a specific OldLoanAccountingMaping.
        /// </summary>
        /// <param name="request">The GetOldLoanAccountingMapingQuery containing OldLoanAccountingMaping ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<OldLoanAccountingMapingDto>> Handle(GetOldLoanAccountingMapingQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the OldLoanAccountingMaping entity with the specified ID from the repository
                var entity = await _OldLoanAccountingMapingRepository.FindAsync(request.Id);

                if (entity != null)
                {
                    // Map the OldLoanAccountingMaping entity to OldLoanAccountingMapingDto
                    var OldLoanAccountingMapingDto = _mapper.Map<OldLoanAccountingMapingDto>(entity);

                    // Log and audit success
                    string successMessage = $"Successfully retrieved OldLoanAccountingMaping with Id: {request.Id}.";
                    await BaseUtilities.LogAndAuditAsync(successMessage, entity, HttpStatusCodeEnum.OK, LogAction.Read, LogLevelInfo.Information);

                    // Return the result with 200 status
                    return ServiceResponse<OldLoanAccountingMapingDto>.ReturnResultWith200(OldLoanAccountingMapingDto);
                }
                else
                {
                    // Log the 404 error and return a 404 response
                    errorMessage = $"OldLoanAccountingMaping with Id: {request.Id} was not found.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.Read, LogLevelInfo.Warning);

                    return ServiceResponse<OldLoanAccountingMapingDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log the error and return a 500 Internal Server Error response
                errorMessage = $"Error occurred while retrieving OldLoanAccountingMaping with Id: {request.Id}. Error: {e.Message}";
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.Read, LogLevelInfo.Error);

                return ServiceResponse<OldLoanAccountingMapingDto>.Return500(errorMessage);
            }
        }
    }

}
