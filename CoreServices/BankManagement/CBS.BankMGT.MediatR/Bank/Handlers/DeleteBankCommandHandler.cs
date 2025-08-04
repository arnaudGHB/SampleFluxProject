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
    public class DeleteBankCommandHandler : IRequestHandler<DeleteBankCommand, ServiceResponse<bool>>
    {
        private readonly IBankRepository _BankRepository; // Repository for accessing Bank data.
        private readonly ILogger<DeleteBankCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteBankCommandHandler.
        /// </summary>
        /// <param name="BankRepository">Repository for Bank data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteBankCommandHandler(
            IBankRepository BankRepository, IMapper mapper,
            ILogger<DeleteBankCommandHandler> logger
, IUnitOfWork<POSContext> uow)
        {
            _BankRepository = BankRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteBankCommand to delete a Bank.
        /// </summary>
        /// <param name="request">The DeleteBankCommand containing Bank ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteBankCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the Bank entity with the specified ID exists
                var existingBank = await _BankRepository.FindAsync(request.Id);
                if (existingBank == null)
                {
                    errorMessage = $"Bank with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }
                existingBank.IsDeleted = true;
                _BankRepository.Update(existingBank);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
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
