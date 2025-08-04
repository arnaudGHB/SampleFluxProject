using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to update a ChartOfAccount based on UpdateChartOfAccountCommand.
    /// </summary>
    public class UpdateChartOfAccountCommandHandler : IRequestHandler<UpdateChartOfAccountCommand, ServiceResponse<ChartOfAccountDto>>
    {
        private readonly IChartOfAccountRepository _ChartOfAccountRepository; // Repository for accessing Account data.
        private readonly ILogger<UpdateChartOfAccountCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the UpdateChartOfAccountCommandHandler.
        /// </summary>
        /// <param name="ChartOfAccountRepository">Repository for Account data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateChartOfAccountCommandHandler(
            IChartOfAccountRepository ChartOfAccountRepository,
            ILogger<UpdateChartOfAccountCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<POSContext> uow ,UserInfoToken userInfoToken)
        {
            _ChartOfAccountRepository = ChartOfAccountRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the UpdateChartOfAccountCommand to update a ChartOfAccount.
        /// </summary>
        /// <param name="request">The UpdateChartOfAccountCommand containing updated Account data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<ChartOfAccountDto>> Handle(UpdateChartOfAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the Account entity to be updated from the repository
                var existingAccount = await _ChartOfAccountRepository.FindAsync(request.Id);

                // Check if the Account entity exists
                if (existingAccount != null)
                {
                       // Use the repository to update the existing Account entity
                    existingAccount = _mapper.Map(request,existingAccount);
                    _ChartOfAccountRepository.Update(existingAccount);
                    await _uow.SaveAsync();
                     
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<ChartOfAccountDto>.ReturnResultWith200(_mapper.Map<ChartOfAccountDto>(existingAccount));
                    _logger.LogInformation($"Account {request.AccountNumber} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the Account entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.AccountNumber} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<ChartOfAccountDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Account: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<ChartOfAccountDto>.Return500(e);
            }
        }
    }
}