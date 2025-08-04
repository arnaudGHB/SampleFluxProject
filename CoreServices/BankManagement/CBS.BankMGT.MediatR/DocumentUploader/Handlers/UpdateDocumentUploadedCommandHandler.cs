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
    /// Handles the command to update a Division based on UpdateDivisionCommand.
    /// </summary>
    public class UpdateDivisionCommandHandler : IRequestHandler<UpdateDivisionCommand, ServiceResponse<DivisionDto>>
    {
        private readonly IDivisionRepository _DivisionRepository; // Repository for accessing Division data.
        private readonly ILogger<UpdateDivisionCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateDivisionCommandHandler.
        /// </summary>
        /// <param name="DivisionRepository">Repository for Division data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateDivisionCommandHandler(
            IDivisionRepository DivisionRepository,
            ILogger<UpdateDivisionCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<POSContext> uow = null)
        {
            _DivisionRepository = DivisionRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateDivisionCommand to update a Division.
        /// </summary>
        /// <param name="request">The UpdateDivisionCommand containing updated Division data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<DivisionDto>> Handle(UpdateDivisionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the Division entity to be updated from the repository
                var existingDivision = await _DivisionRepository.FindAsync(request.Id);

                // Check if the Division entity exists
                if (existingDivision != null)
                {
                    // Update Division entity properties with values from the request
                    existingDivision.Name = request.Name;
                    existingDivision.RegionId = request.RegionId;
                    // Use the repository to update the existing Division entity
                    _DivisionRepository.Update(existingDivision);
                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<DivisionDto>.Return500();
                    }
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<DivisionDto>.ReturnResultWith200(_mapper.Map<DivisionDto>(existingDivision));
                    _logger.LogInformation($"Division {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the Division entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<DivisionDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Division: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<DivisionDto>.Return500(e);
            }
        }
    }

}
