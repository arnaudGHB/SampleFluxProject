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
    /// Handles the request to retrieve a specific Standing Order based on its unique identifier.
    /// </summary>
    public class GetStandingOrderQueryHandler : IRequestHandler<GetStandingOrderQuery, ServiceResponse<StandingOrderDto>>
    {
        private readonly IStandingOrderRepository _standingOrderRepository; // Repository for accessing Standing Order data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetStandingOrderQueryHandler> _logger; // Audi Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetStandingOrderQueryHandler.
        /// </summary>
        /// <param name="standingOrderRepository">Repository for Standing Order data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Audi Logger for logging handler actions and errors.</param>
        public GetStandingOrderQueryHandler(
            IStandingOrderRepository standingOrderRepository,
            IMapper mapper,
            ILogger<GetStandingOrderQueryHandler> logger)
        {
            _standingOrderRepository = standingOrderRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetStandingOrderQuery to retrieve a specific Standing Order.
        /// </summary>
        /// <param name="request">The GetStandingOrderQuery containing Standing Order ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<StandingOrderDto>> Handle(GetStandingOrderQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            string successMessage = "Standing Order retrieved successfully.";

            try
            {
                // Retrieve the Standing Order entity with the specified ID from the repository
                var entity = await _standingOrderRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    // Map the Standing Order entity to StandingOrderDto and return it with a success response
                    var standingOrderDto = _mapper.Map<StandingOrderDto>(entity);

                    // Audit the success
                    await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.StandingOrder, LogLevelInfo.Information);

                    return ServiceResponse<StandingOrderDto>.ReturnResultWith200(standingOrderDto);
                }
                else
                {
                    // If the Standing Order entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Standing Order not found.");
                    return ServiceResponse<StandingOrderDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Standing Order: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.StandingOrder, LogLevelInfo.Error);
                return ServiceResponse<StandingOrderDto>.Return500(e);
            }
        }
    }

}
