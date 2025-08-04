using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Commands;

using CBS.BankMGT.Common.UnitOfWork;
using CBS.BankMGT.Domain;
using CBS.BankMGT.Repository;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Data.Dto;

namespace CBS.BankMGT.MediatR.Handlers
{

    /// <summary>
    /// Handles the command to update a EconomicActivity based on UpdateEconomicActivityCommand.
    /// </summary>
    public class UpdateEconomicActivityCommandHandler : IRequestHandler<UpdateEconomicActivityCommand, ServiceResponse<EconomicActivityDto>>
    {
        private readonly IEconomicActivityRepository _EconomicActivityRepository; // Repository for accessing EconomicActivity data.
        private readonly ILogger<UpdateEconomicActivityCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateEconomicActivityCommandHandler.
        /// </summary>
        /// <param name="EconomicActivityRepository">Repository for EconomicActivity data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateEconomicActivityCommandHandler(
            IEconomicActivityRepository EconomicActivityRepository,
            ILogger<UpdateEconomicActivityCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<POSContext> uow = null)
        {
            _EconomicActivityRepository = EconomicActivityRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateEconomicActivityCommand to update a EconomicActivity.
        /// </summary>
        /// <param name="request">The UpdateEconomicActivityCommand containing updated EconomicActivity data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<EconomicActivityDto>> Handle(UpdateEconomicActivityCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the EconomicActivity entity to be updated from the repository
                var existingEconomicActivity = await _EconomicActivityRepository.FindAsync(request.Id);

                // Check if the EconomicActivity entity exists
                if (existingEconomicActivity != null)
                {
                    // Update EconomicActivity entity properties with values from the request
                    existingEconomicActivity.Name = request.Name;
                    existingEconomicActivity.Description = request.Description;
                    // Use the repository to update the existing EconomicActivity entity
                    _EconomicActivityRepository.Update(existingEconomicActivity);
                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<EconomicActivityDto>.Return500();
                    }
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<EconomicActivityDto>.ReturnResultWith200(_mapper.Map<EconomicActivityDto>(existingEconomicActivity));
                    _logger.LogInformation($"EconomicActivity {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the EconomicActivity entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<EconomicActivityDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating EconomicActivity: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<EconomicActivityDto>.Return500(e);
            }
        }
    }

}
