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
    /// Handles the request to retrieve all Standing Orders for a specific member.
    /// </summary>
    public class GetMemberStandingOrdersQueryHandler : IRequestHandler<GetStandingOrderByMemberIdQuery, ServiceResponse<List<StandingOrderDto>>>
    {
        private readonly IStandingOrderRepository _standingOrderRepository; // Repository for accessing Standing Order data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetMemberStandingOrdersQueryHandler> _logger; // Audi Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetMemberStandingOrdersQueryHandler.
        /// </summary>
        /// <param name="standingOrderRepository">Repository for Standing Order data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Audi Logger for logging handler actions and errors.</param>
        public GetMemberStandingOrdersQueryHandler(
            IStandingOrderRepository standingOrderRepository,
            IMapper mapper,
            ILogger<GetMemberStandingOrdersQueryHandler> logger)
        {
            _standingOrderRepository = standingOrderRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetStandingOrderByMemberIdQuery to retrieve all Standing Orders for a specific member.
        /// </summary>
        /// <param name="request">The GetStandingOrderByMemberIdQuery containing the member ID.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<StandingOrderDto>>> Handle(GetStandingOrderByMemberIdQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            string successMessage = "Member's Standing Orders retrieved successfully.";

            try
            {
                // Retrieve all Standing Order entities for the specified member from the repository
                var entities = await _standingOrderRepository.FindBy(x => x.MemberId == request.MemberId && x.IsDeleted==false).ToListAsync();
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
                    _logger.LogInformation("No Standing Orders found for the member.");
                    return ServiceResponse<List<StandingOrderDto>>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while retrieving Member's Standing Orders: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.StandingOrder, LogLevelInfo.Error);
                return ServiceResponse<List<StandingOrderDto>>.Return500(e);
            }
        }
    }
}
