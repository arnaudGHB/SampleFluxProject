using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using Microsoft.EntityFrameworkCore;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific DepositLimit based on its unique identifier.
    /// </summary>
    public class GetDepositLimitQueryHandler : IRequestHandler<GetDepositLimitQuery, ServiceResponse<CashDepositParameterDto>>
    {
        private readonly IDepositLimitRepository _DepositLimitRepository; // Repository for accessing DepositLimit data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetDepositLimitQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetDepositLimitQueryHandler.
        /// </summary>
        /// <param name="DepositLimitRepository">Repository for DepositLimit data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetDepositLimitQueryHandler(
            IDepositLimitRepository DepositLimitRepository,
            IMapper mapper,
            ILogger<GetDepositLimitQueryHandler> logger)
        {
            _DepositLimitRepository = DepositLimitRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetDepositLimitQuery to retrieve a specific DepositLimit.
        /// </summary>
        /// <param name="request">The GetDepositLimitQuery containing DepositLimit ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
       public async Task<ServiceResponse<CashDepositParameterDto>> Handle(GetDepositLimitQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                //request.Id = "017085183837349";
                // Retrieve the DepositLimit entity with the specified ID from the repository
                var entity = await _DepositLimitRepository.FindBy(x=>x.Id==request.Id).Include(x=>x.Product).FirstOrDefaultAsync();
                if (entity != null)
                {
                    // Map the DepositLimit entity to DepositLimitDto and return it with a success response
                    var DepositLimitDto = _mapper.Map<CashDepositParameterDto>(entity);
                    return ServiceResponse<CashDepositParameterDto>.ReturnResultWith200(DepositLimitDto);
                }
                else
                {
                    // If the DepositLimit entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("DepositLimit not found.");
                    return ServiceResponse<CashDepositParameterDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting DepositLimit: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<CashDepositParameterDto>.Return500(e);
            }
        }

    }

}
