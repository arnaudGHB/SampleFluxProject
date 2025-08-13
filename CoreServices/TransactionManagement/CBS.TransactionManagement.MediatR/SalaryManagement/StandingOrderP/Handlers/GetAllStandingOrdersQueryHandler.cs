using AutoMapper;
using CBS.TransactionManagement.Data.Dto.SalaryManagement.StandingOrderP;
using CBS.TransactionManagement.Data.Entity.SalaryFilesDto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.SalaryManagement.SalaryFiles.Queries;
using CBS.TransactionManagement.MediatR.SalaryManagement.StandingOrderP.Queries;
using CBS.TransactionManagement.Repository.SalaryManagement.SalaryFiles;
using CBS.TransactionManagement.Repository.SalaryManagement.StandingOrderP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.MediatR.SalaryManagement.StandingOrderP.Handlers
{
    /// <summary>
    /// Handles the request to retrieve all Standing Orders, optionally filtered by Branch ID.
    /// </summary>
    public class GetAllStandingOrdersQueryHandler : IRequestHandler<GetAllStandingOrdersQuery, ServiceResponse<List<StandingOrderDto>>>
    {
        private readonly IStandingOrderRepository _standingOrderRepository; // Repository for accessing Standing Order data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllStandingOrdersQueryHandler> _logger; // Audi Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllStandingOrdersQueryHandler.
        /// </summary>
        /// <param name="standingOrderRepository">Repository for Standing Order data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Audi Logger for logging handler actions and errors.</param>
        public GetAllStandingOrdersQueryHandler(
            IStandingOrderRepository standingOrderRepository,
            IMapper mapper,
            ILogger<GetAllStandingOrdersQueryHandler> logger)
        {
            _standingOrderRepository = standingOrderRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllStandingOrdersQuery to retrieve all Standing Orders.
        /// </summary>
        /// <param name="request">The GetAllStandingOrdersQuery containing optional Branch ID.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<StandingOrderDto>>> Handle(GetAllStandingOrdersQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            string successMessage = "Standing Orders retrieved successfully.";

            try
            {
                // Retrieve all Standing Order entities where IsDelete is false
                var entities = await _standingOrderRepository.All.Where(x => !x.IsDeleted).ToListAsync(cancellationToken);

                // Optionally filter by Branch ID
                if (!string.IsNullOrEmpty(request.BranchId))
                {
                    entities = entities.Where(x => x.BranchId == request.BranchId).ToList();
                }

                if (entities.Any())
                {
                    // Map the Standing Order entities to a list of StandingOrderDto and return it with a success response
                    var standingOrderDtos = _mapper.Map<List<StandingOrderDto>>(entities);

                    // Audit the success
                    await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.StandingOrder, LogLevelInfo.Information);

                    return ServiceResponse<List<StandingOrderDto>>.ReturnResultWith200(standingOrderDtos);
                }
                else
                {
                    // If no Standing Orders were found, log the information and return a 404 Not Found response
                    _logger.LogInformation("No Standing Orders found.");
                    return ServiceResponse<List<StandingOrderDto>>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while retrieving Standing Orders: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.StandingOrder, LogLevelInfo.Error);
                return ServiceResponse<List<StandingOrderDto>>.Return500(e);
            }
        }
    }
}
