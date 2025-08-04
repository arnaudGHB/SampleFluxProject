using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Commands;
using CBS.BankMGT.Repository;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Common.UnitOfWork;
using CBS.BankMGT.Domain;
using Microsoft.EntityFrameworkCore;

namespace CBS.BankMGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to delete a Bank based on DeleteBankCommand.
    /// </summary>
    public class DeleteThirdPartyBrancheCommandHandler : IRequestHandler<DeleteThirdPartyBrancheCommand, ServiceResponse<bool>>
    {
        private readonly IThirdPartyBrancheRepository _thirdPartyIRepository; // Repository for accessing Bank data.
        private readonly ILogger<DeleteThirdPartyBrancheCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteBankCommandHandler.
        /// </summary>
        /// <param name="BankRepository">Repository for Bank data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteThirdPartyBrancheCommandHandler(
            IThirdPartyBrancheRepository thirdPartyIRepository, IMapper mapper,
            ILogger<DeleteThirdPartyBrancheCommandHandler> logger
, IUnitOfWork<POSContext> uow)
        {
            _thirdPartyIRepository = thirdPartyIRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteBankCommand to delete a Bank.
        /// </summary>
        /// <param name="request">The DeleteBankCommand containing Bank ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteThirdPartyBrancheCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the Bank entity with the specified ID exists
                var existingBank = await _thirdPartyIRepository.FindAsync(request.Id);
                if (existingBank == null)
                {
                    errorMessage = $"Bank with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }
                existingBank.IsDeleted = true;
                _thirdPartyIRepository.Update(existingBank);
                await _uow.SaveAsync();
                
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting Bank: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
