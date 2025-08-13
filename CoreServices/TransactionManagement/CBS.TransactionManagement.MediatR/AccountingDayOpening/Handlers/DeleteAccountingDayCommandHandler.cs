using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Repository.AccountingDayOpening;
using CBS.TransactionManagement.Commands.AccountingDayOpening;

namespace CBS.TransactionManagement.Handlers.AccountingDayOpening
{
  
    public class DeleteAccountingDayCommandHandler : IRequestHandler<DeleteAccountingDayCommand, ServiceResponse<bool>>
    {
        private readonly IAccountingDayRepository _accountingDayRepository; // Repository for accessing AccountingDay data.
        private readonly ILogger<DeleteAccountingDayCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the DeleteAccountingDayCommandHandler.
        /// </summary>
        public DeleteAccountingDayCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<DeleteAccountingDayCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            IAccountingDayRepository accountingDayRepository)
        {
            _userInfoToken = userInfoToken;
            _logger = logger;
            _uow = uow;
            _accountingDayRepository = accountingDayRepository;
        }

        /// <summary>
        /// Handles the DeleteAccountingDayCommand to delete an AccountingDay.
        /// </summary>
        public async Task<ServiceResponse<bool>> Handle(DeleteAccountingDayCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                if (request.DelateByDate)
                {

                    // Delete by date
                    await _accountingDayRepository.DeleteAccountingDay(request.Date);
                    errorMessage = $"AccountingDay for {request.Date} were all deleted successfully by {_userInfoToken.FullName}";
                    _logger.LogInformation(errorMessage);

                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Delete.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                    return ServiceResponse<bool>.ReturnResultWith200(true);
                }
                else
                {
                    // Delete by ID
                    var existingAccountingDay = await _accountingDayRepository.FindAsync(request.Id);
                    if (existingAccountingDay == null)
                    {
                        errorMessage = $"AccountingDay with ID {request.Id} not found.";
                        _logger.LogError(errorMessage);
                        await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Delete.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                        return ServiceResponse<bool>.Return404(errorMessage);
                    }

                    existingAccountingDay.IsDeleted = true; // Soft delete approach
                    _accountingDayRepository.Update(existingAccountingDay);

                    if (await _uow.SaveAsync() <= 0)
                    {
                        errorMessage = "An error occurred while saving changes to the database.";
                        await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Delete.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                        return ServiceResponse<bool>.Return500(errorMessage);
                    }
                    errorMessage = $"AccountingDay for {request.Date} with branch id {existingAccountingDay.BranchId} was deleted successfully by {_userInfoToken.FullName}";
                    _logger.LogInformation(errorMessage);

                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Delete.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                    return ServiceResponse<bool>.ReturnResultWith200(true);
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting AccountingDay: {e.Message}";
                _logger.LogError(e, errorMessage); // Log the exception with stack trace
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Delete.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
