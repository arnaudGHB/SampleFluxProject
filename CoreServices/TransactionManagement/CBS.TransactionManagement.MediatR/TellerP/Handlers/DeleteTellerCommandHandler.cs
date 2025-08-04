using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.MediatR.TellerP.Commands;

namespace CBS.TransactionManagement.MediatR.TellerP.Handlers
{
    /// <summary>
    /// Handles the command to delete a Teller based on DeleteTellerCommand.
    /// </summary>
    public class DeleteTellerCommandHandler : IRequestHandler<DeleteTellerCommand, ServiceResponse<bool>>
    {
        private readonly ITellerRepository _TellerRepository; // Repository for accessing Teller data.
        private readonly ILogger<DeleteTellerCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly IAccountRepository _accountRepository;
        /// <summary>
        /// Constructor for initializing the DeleteTellerCommandHandler.
        /// </summary>
        /// <param name="TellerRepository">Repository for Teller data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteTellerCommandHandler(
            ITellerRepository TellerRepository, IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<DeleteTellerCommandHandler> logger
, IUnitOfWork<TransactionContext> uow, IAccountRepository accountRepository = null)
        {
            _TellerRepository = TellerRepository;
            _logger = logger;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _uow = uow;
            _accountRepository = accountRepository;
        }

        /// <summary>
        /// Handles the DeleteTellerCommand to delete a Teller.
        /// </summary>
        /// <param name="request">The DeleteTellerCommand containing Teller ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteTellerCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the Teller entity with the specified ID exists
                var existingTeller = await _TellerRepository.FindAsync(request.Id);
                if (existingTeller == null)
                {
                    errorMessage = $"Teller with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Delete.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }
                var tellerAccount = await _accountRepository.FindBy(x => x.TellerId == existingTeller.Id).FirstOrDefaultAsync();
                tellerAccount.IsDeleted = true;
                existingTeller.IsDeleted = true;
                _TellerRepository.Update(existingTeller);
                _accountRepository.Update(tellerAccount);
                await _uow.SaveAsync();
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting Teller: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
