using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.AlertProfileMediaR.Commands;
using CBS.NLoan.Repository.AlertProfileP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.AlertProfileMediaR.Handlers
{
    /// <summary>
    /// Handles the command to delete a Loan based on DeleteLoanCommand.
    /// </summary>
    public class DeleteAlertProfileHandler : IRequestHandler<DeleteAlertProfileCommand, ServiceResponse<bool>>
    {
        private readonly IAlertProfileRepository _AlertProfileRepository; // Repository for accessing AlertProfile data.
        private readonly ILogger<DeleteAlertProfileHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteAlertProfileCommandHandler.
        /// </summary>
        /// <param name="AlertProfileRepository">Repository for AlertProfile data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteAlertProfileHandler(
            IAlertProfileRepository AlertProfileRepository, IMapper mapper,
            ILogger<DeleteAlertProfileHandler> logger, IUnitOfWork<LoanContext> uow)
        {
            _AlertProfileRepository = AlertProfileRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteAlertProfileCommand to delete a AlertProfile.
        /// </summary>
        /// <param name="request">The DeleteAlertProfileCommand containing AlertProfile ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteAlertProfileCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the AlertProfile entity with the specified ID exists
                var existingAlertProfile = await _AlertProfileRepository.FindAsync(request.Id);
                if (existingAlertProfile == null)
                {
                    errorMessage = $"AlertProfile with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingAlertProfile.IsDeleted = true;
                _AlertProfileRepository.Update(existingAlertProfile);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting AlertProfile: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
