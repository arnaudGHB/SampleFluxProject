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
    /// Handles the command to update a Town based on UpdateTownCommand.
    /// </summary>
    public class UpdateTownCommandHandler : IRequestHandler<UpdateTownCommand, ServiceResponse<TownDto>>
    {
        private readonly ITownRepository _TownRepository; // Repository for accessing Town data.
        private readonly ILogger<UpdateTownCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateTownCommandHandler.
        /// </summary>
        /// <param name="TownRepository">Repository for Town data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateTownCommandHandler(
            ITownRepository TownRepository,
            ILogger<UpdateTownCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<POSContext> uow = null)
        {
            _TownRepository = TownRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateTownCommand to update a Town.
        /// </summary>
        /// <param name="request">The UpdateTownCommand containing updated Town data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<TownDto>> Handle(UpdateTownCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the Town entity to be updated from the repository
                var existingTown = await _TownRepository.FindAsync(request.Id);

                // Check if the Town entity exists
                if (existingTown != null)
                {
                    // Update Town entity properties with values from the request
                    existingTown.Name = request.Name;
                    existingTown.SubdivisionId = request.SubdivisionId;
                    // Use the repository to update the existing Town entity
                    _TownRepository.Update(existingTown);
                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<TownDto>.Return500();
                    }
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<TownDto>.ReturnResultWith200(_mapper.Map<TownDto>(existingTown));
                    _logger.LogInformation($"Town {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the Town entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<TownDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Town: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<TownDto>.Return500(e);
            }
        }
    }

}
