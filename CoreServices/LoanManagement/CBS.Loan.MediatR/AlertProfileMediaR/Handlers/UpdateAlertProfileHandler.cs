using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.AlertProfileP;
using CBS.NLoan.Data.Entity.AlertProfileP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.AlertProfileMediaR.Commands;
using CBS.NLoan.Repository.AlertProfileP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.AlertProfileMediaR.Handlers
{

    /// <summary>
    /// Handles the command to update a Loan based on UpdateLoanCommand.
    /// </summary>
    public class UpdateAlertProfileHandler : IRequestHandler<UpdateAlertProfileCommand, ServiceResponse<AlertProfileDto>>
    {
        private readonly IAlertProfileRepository _AlertProfileRepository; // Repository for accessing AlertProfile data.
        private readonly ILogger<UpdateAlertProfileHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateAlertProfileCommandHandler.
        /// </summary>
        /// <param name="AlertProfileRepository">Repository for AlertProfile data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateAlertProfileHandler(
            IAlertProfileRepository AlertProfileRepository,
            ILogger<UpdateAlertProfileHandler> logger,
            IMapper mapper,
            IUnitOfWork<LoanContext> uow = null)
        {
            _AlertProfileRepository = AlertProfileRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateAlertProfileCommand to update a AlertProfile.
        /// </summary>
        /// <param name="request">The UpdateAlertProfileCommand containing updated AlertProfile data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AlertProfileDto>> Handle(UpdateAlertProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the AlertProfile entity to be updated from the repository
                var existingAlertProfile = await _AlertProfileRepository.FindAsync(request.Id);

                // Check if the AlertProfile entity exists
                if (existingAlertProfile != null)
                {
                    // Update AlertProfile entity properties with values from the request
                    var AlertProfileToUpdate = _mapper.Map<AlertProfile>(request);
                    // Use the repository to update the existing AlertProfile entity
                    _AlertProfileRepository.Update(AlertProfileToUpdate);
                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<AlertProfileDto>.Return500();
                    }
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<AlertProfileDto>.ReturnResultWith200(_mapper.Map<AlertProfileDto>(existingAlertProfile));
                    _logger.LogInformation($"AlertProfile {request.Name} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the AlertProfile entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Name} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<AlertProfileDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating AlertProfile: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<AlertProfileDto>.Return500(e);
            }
        }
    }

}
