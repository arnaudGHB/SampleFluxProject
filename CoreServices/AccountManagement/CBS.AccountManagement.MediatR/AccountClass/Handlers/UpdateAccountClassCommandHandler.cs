using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to update a AccountClass based on UpdateAccountClassCommand.
    /// </summary>
    public class UpdateAccountClassCommandHandler : IRequestHandler<UpdateAccountClassCommand, ServiceResponse<AccountClassDto>>
    {
        private readonly IAccountClassRepository _AccountClassRepository; // Repository for accessing AccountClass data.
        private readonly ILogger<UpdateAccountClassCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateAccountClassCommandHandler.
        /// </summary>
        /// <param name="AccountClassRepository">Repository for AccountClass data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateAccountClassCommandHandler(
            IAccountClassRepository AccountClassRepository,
            ILogger<UpdateAccountClassCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<POSContext> uow = null)
        {
            _AccountClassRepository = AccountClassRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateAccountClassCommand to update a AccountClass.
        /// </summary>
        /// <param name="request">The UpdateAccountClassCommand containing updated AccountClass data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AccountClassDto>> Handle(UpdateAccountClassCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the AccountClass entity to be updated from the repository
                var existingAccountClass = await _AccountClassRepository.FindAsync(request.Id);

                // Check if the AccountClass entity exists
                if (existingAccountClass != null)
                {
                    // Update AccountClass entity properties with values from the request
                    existingAccountClass.AccountNumber = request.AccountNumber;
                    existingAccountClass.AccountCategoryId = request.AccountCategoryId;
                    //existingAccountClass.AccountCategoryId = request.AccountCategoryId;
                    // Use the repository to update the existing AccountClass entity
                    _AccountClassRepository.Update(existingAccountClass);
                    await _uow.SaveAsync();
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<AccountClassDto>.ReturnResultWith200(_mapper.Map<AccountClassDto>(existingAccountClass));
                    _logger.LogInformation($"AccountClass {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the AccountClass entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<AccountClassDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating AccountClass: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<AccountClassDto>.Return500(e);
            }
        }
    }
}